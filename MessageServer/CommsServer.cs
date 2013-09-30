using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading.Tasks;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using System.Linq;

namespace MessageServer
{

    #region IMessage interface
  
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMessageCallback))]
    interface IMessage
    {
        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        Task SendPropertyMessage(APIPropertyMessage property);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        Task SendListMessage(APIListMessage listData);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        Task SendInfoMessage(APIInfoMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        Task SendDataMessage(APIDataMessage msg);



        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        Task SendMediaPortalMessage(APIMediaPortalMessage msg);

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        Task SendTVServerMessage(APITVServerMessage msg);


        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        Task<List<APIConnection>> Connect(APIConnection name);
        
        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        Task Disconnect();
    }

    #endregion

    #region IMessageCallback interface

    interface IMessageCallback
    {
        /// <summary>
        /// Receives the MP property message.
        /// </summary>
        /// <param name="property">The property.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveAPIPropertyMessage(APIPropertyMessage message);

        /// <summary>
        /// Receives the MPGUI list message.
        /// </summary>
        /// <param name="property">The property.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveAPIListMessage(APIListMessage message);

        /// <summary>
        /// Receives the MP multi message.
        /// </summary>
        /// <param name="property">The property.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveAPIInfoMessage(APIInfoMessage message);


        /// <summary>
        /// Receives the API data message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        Task ReceiveAPIDataMessage(APIDataMessage message);

        /// <summary>
        /// Receives the MP display message.
        /// </summary>
        /// <param name="message">The message.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveMediaPortalMessage(APIMediaPortalMessage message);

        /// <summary>
        /// Receives the MP display message.
        /// </summary>
        /// <param name="message">The message.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveTVServerMessage(APITVServerMessage message);

        /// <summary>
        /// Users the connected.
        /// </summary>
        /// <param name="user">The user.</param>
        [OperationContract(IsOneWay = true)]
        Task SessionConnected(APIConnection connection);

        /// <summary>
        /// Users the disconneted.
        /// </summary>
        /// <param name="user">The user.</param>
        [OperationContract(IsOneWay = true)]
        Task SessionDisconnected(APIConnection connection);

    }

    #endregion

    #region Public enums/event args

    public enum MessageType
    { 
        ReceiveAPIPropertyMessage,
        ReceiveAPIListMessage,
        ReceiveAPIInfoMessage,
        ReceiveAPIDataMessage,

        ReceiveMediaPortalMessage,
        ReceiveTVServerMessage,

        SessionConnected,
        SessionDisconnected 
    };

    public class MessageEventArgs : EventArgs
    {
        public MessageType MessageType;
        public APIConnection Connection;
        public APIListMessage ListMessage;
        public APIPropertyMessage PropertyMessage;
        public APIInfoMessage InfoMessage;
        public APIVisibleMessage VisibleMessage;
        public APIDataMessage DataMessage;
        public APITVServerMessage TVServerMessage;
        public APIMediaPortalMessage MediaPortalMessage;
    }

    #endregion

    #region MessageService

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MessageService : IMessage
    {
        #region Vars

        public static Dictionary<APIConnection, MessageEventHandler> MPDisplayConnectionList = new Dictionary<APIConnection, MessageEventHandler>();
        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public static event MessageEventHandler MessageEvent;
        private static Log Log = LoggingManager.GetLog(typeof(MessageService));

        private static object _syncObj = new object();
        private IMessageCallback _messageCallback = null;
        private MessageEventHandler _callbackEventHandler = null;
        private APIConnection _apiConnection;
       

        #endregion

        #region IMessage implementation

        /// <summary>
        /// Connects the specified user.
        /// </summary>
        /// <param name="connection">The user.</param>
        /// <returns></returns>
        public async Task<List<APIConnection>> Connect(APIConnection connection)
        {
            try
            {
                if (connection != null)
                {
                    Log.Message(LogLevel.Info, "[Connect] - New connection request received, ConnectionName: {0}", connection.ConnectionName);
                    if (CheckIfConnectionExists(connection.ConnectionName))
                    {
                        Log.Message(LogLevel.Info, "[Connect] - Connection '{0}' already exists, removing existing instance", connection.ConnectionName);
                        var handlerToRemove = GetConnectionHandler(connection.ConnectionName);
                        if (handlerToRemove != null)
                        {
                            MessageEvent -= handlerToRemove;
                        }
                        var connectionToRemove = GetConnection(connection.ConnectionName);
                        if (connectionToRemove != null)
                        {
                            MPDisplayConnectionList.Remove(connectionToRemove);
                        }
                    }
                   
                    _messageCallback = OperationContext.Current.GetCallbackChannel<IMessageCallback>();
                    _callbackEventHandler = new MessageEventHandler(CallbackEventHandler);
                    _apiConnection = connection;

                    MPDisplayConnectionList.Add(connection, CallbackEventHandler);
                    MessageEvent += _callbackEventHandler;
                    await BroadcastMessage(new MessageEventArgs
                    {
                        MessageType = MessageType.SessionConnected,
                        Connection = _apiConnection
                    });

                    var activeConnections = new List<APIConnection>();
                    lock (_syncObj)
                    {
                        activeConnections = new List<APIConnection>(MPDisplayConnectionList.Keys);
                    }
                    Log.Message(LogLevel.Info, "[Connect] - Successfully established connection, ConnectionName: {0}", connection.ConnectionName);
                    return activeConnections;
                }
                else
                {
                    Log.Message(LogLevel.Error, "[Connect] - Failed to create new connection.");
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[Connect] - An exception occured while connecting.", ex);
            }
            return new List<APIConnection>();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public async Task Disconnect()
        {
            if (_apiConnection == null)
                return;
            try
            {
                Log.Message(LogLevel.Info, "[Disconnect] - Disconnection request received, ConnectionName: {0}" , _apiConnection.ConnectionName);
                var connectionToRemove = GetConnectionHandler(_apiConnection.ConnectionName);
                lock (_syncObj)
                {
                    MPDisplayConnectionList.Remove(_apiConnection);
                }
                if (connectionToRemove != null)
                {
                    MessageEvent -= connectionToRemove;
                    MessageEvent -= _callbackEventHandler;
                    var e = new MessageEventArgs();
                    e.MessageType = MessageType.SessionDisconnected;
                    e.Connection = _apiConnection;
                    Log.Message(LogLevel.Info, "[Disconnect] - Successfully removed connection instance, ConnectionName: {0}", _apiConnection.ConnectionName);
                    _apiConnection = null;
                    await BroadcastMessage(e);
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[Disconnect] - An exception occured while disconnecting.", ex);
            }
        }

    


        /// <summary>
        /// Sends the MP property.
        /// </summary>
        /// <param name="property">The property.</param>
        public async Task SendPropertyMessage(APIPropertyMessage property)
        {
            try
            {
                if (_apiConnection != null && property != null)
                {
                    Log.Message(LogLevel.Verbose, "[SendPropertyMessage] - Sending Property message to MPDisplay, Sender: {0}, PropertyType: {1}, SkinTag: {2}"
                        , _apiConnection.ConnectionName, property.PropertyType, property.SkinTag);
                    await BroadcastMessage(new MessageEventArgs
                    {
                        MessageType = MessageType.ReceiveAPIPropertyMessage,
                        Connection = _apiConnection,
                        PropertyMessage = property
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendPropertyMessage] - An exception occured sending message.", ex);
            }
        }

        /// <summary>
        /// Sends the MPGUI list.
        /// </summary>
        /// <param name="listdata">The listdata.</param>
        public async Task SendListMessage(APIListMessage listMessage)
        {
            try
            {
                if (_apiConnection != null && listMessage != null)
                {
                    Log.Message(LogLevel.Verbose, "[SendListMessage] - Sending List message to MPDisplay, Sender: {0}, MessageType: {1}"
                        , _apiConnection.ConnectionName, listMessage.MessageType);

                    await BroadcastMessage(new MessageEventArgs
                    {
                        MessageType = MessageType.ReceiveAPIListMessage,
                        Connection = _apiConnection,
                        ListMessage = listMessage
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendListMessage] - An exception occured sending message.", ex);
            }
        }

        /// <summary>
        /// Sends the MP multi message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendInfoMessage(APIInfoMessage infoMessage)
        {
            try
            {
                if (_apiConnection != null && infoMessage != null)
                {
                    Log.Message(LogLevel.Verbose, "[SendInfoMessage] - Sending Info message to MPDisplay, Sender: {0}, InfoType: {1}"
                        , _apiConnection.ConnectionName, infoMessage.MessageType);

                    await BroadcastMessage(new MessageEventArgs
                    {
                        MessageType = MessageType.ReceiveAPIInfoMessage,
                        Connection = _apiConnection,
                        InfoMessage = infoMessage
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendInfoMessage] - An exception occured sending message.", ex);
            }
        }

        public async Task SendDataMessage(APIDataMessage dataMessage)
        {
            try
            {
                if (_apiConnection != null && dataMessage != null)
                {
                    if (dataMessage.DataType == APIDataMessageType.KeepAlive)
                    {
                        Log.Message(LogLevel.Info, "[KeepAlive] - KeepAlive received., Sender: {0}", _apiConnection.ConnectionName);
                    }
                    else
                    {
                        Log.Message(LogLevel.Verbose, "[SendDataMessage] - Sending message to MPDisplay, Sender: {0}, DataType: {1}", _apiConnection.ConnectionName, dataMessage.DataType);
                    }
                  
                    await BroadcastMessage(new MessageEventArgs
                    {
                        MessageType = MessageType.ReceiveAPIDataMessage,
                        Connection = _apiConnection,
                        DataMessage = dataMessage
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendDataMessage] - An Exception Occured Processing Message", ex);
            }
        }

        /// <summary>
        /// Sends to MediaPortal.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public async Task SendMediaPortalMessage(APIMediaPortalMessage mediaPortalMessage)
        {
            try
            {
                if (_apiConnection != null && mediaPortalMessage != null)
                {
                    Log.Message(LogLevel.Verbose, "[SendMediaPortalMessage] - Sending message to MediaPortal plugin, Sender: {0}, MessageType: {1}", _apiConnection.ConnectionName, mediaPortalMessage.MessageType);
                   
                    var connectionTo = GetConnectionHandler("MediaPortalPlugin");
                    if (connectionTo == null)
                    {
                        Log.Message(LogLevel.Error, "[SendMediaPortalMessage] - Message cannot be sent because the MediaPortal plugin connection does not exist or cannot be found.");
                    }
                    else
                    {
                        await Task.Factory.FromAsync(connectionTo.BeginInvoke(this, new MessageEventArgs
                        {
                            MessageType = MessageType.ReceiveMediaPortalMessage,
                            Connection = _apiConnection,
                            MediaPortalMessage = mediaPortalMessage
                        }, null, null), connectionTo.EndInvoke);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendMediaPortalMessage] - An exception occured sending message.", ex);
            }
        }

        /// <summary>
        /// Sends to MediaPortal.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public async Task SendTVServerMessage(APITVServerMessage msg)
        {
            try
            {
                if (_apiConnection != null && msg != null)
                {
                    Log.Message(LogLevel.Verbose, "[SendMediaPortalMessage] - Sending message to TVServer plugin, Sender: {0}", _apiConnection.ConnectionName);

                    var connectionTo = GetConnectionHandler("TVServerPlugin");
                    if (connectionTo == null)
                    {
                        Log.Message(LogLevel.Error, "[SendTVServerMessage] - Message cannot be sent because the TVServer plugin connection does not exist or cannot be found.");
                    }
                    else
                    {
                        await Task.Factory.FromAsync(connectionTo.BeginInvoke(this, new MessageEventArgs
                        {
                            MessageType = MessageType.ReceiveTVServerMessage,
                            Connection = _apiConnection,
                            TVServerMessage = msg
                        }, null, null), connectionTo.EndInvoke);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Exception("[SendTVServerMessage] - An exception occured sending message.", ex);
            }
        }

   

        #endregion

        #region private methods

        /// <summary>
        /// Callbacks the event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MPDisplayServer.MessageEventArgs"/> instance containing the event data.</param>
        private async void CallbackEventHandler(object sender, MessageEventArgs e)
        {
            try
            {
                if (_messageCallback != null)
                {
                    switch (e.MessageType)
                    {
                        case MessageType.ReceiveAPIPropertyMessage:
                            await _messageCallback.ReceiveAPIPropertyMessage(e.PropertyMessage);
                            break;
                        case MessageType.ReceiveAPIListMessage:
                            await _messageCallback.ReceiveAPIListMessage(e.ListMessage);
                            break;
                        case MessageType.ReceiveAPIInfoMessage:
                            await _messageCallback.ReceiveAPIInfoMessage(e.InfoMessage);
                            break;
                        case MessageType.ReceiveAPIDataMessage:
                            await _messageCallback.ReceiveAPIDataMessage(e.DataMessage);
                            break;
                        case MessageType.ReceiveMediaPortalMessage:
                            await _messageCallback.ReceiveMediaPortalMessage(e.MediaPortalMessage);
                            break;
                        case MessageType.ReceiveTVServerMessage:
                            await _messageCallback.ReceiveTVServerMessage(e.TVServerMessage);
                            break;
                        case MessageType.SessionConnected:
                            await _messageCallback.SessionConnected(e.Connection);
                            break;
                        case MessageType.SessionDisconnected:
                            await _messageCallback.SessionDisconnected(e.Connection);
                            break;
                        default:
                            break;
                    }
                }
                return;
            }
            catch (ObjectDisposedException ex)
            {
                Log.Exception("[CallbackHandler] - Unable to send message to destination, Destination does not exist", ex);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, "[CallbackHandler] - An exception occured while invoking callback.{0}", ex.Message);
            }
            await Disconnect();
        }

        /// <summary>
        /// Broadcasts the message.
        /// </summary>
        /// <param name="e">The <see cref="MPDisplayServer.MessageEventArgs"/> instance containing the event data.</param>
        private async Task BroadcastMessage(MessageEventArgs e)
        {
            try
            {
                MessageEventHandler temp = MessageEvent;
                if (temp != null)
                {
                    foreach (MessageEventHandler handler in temp.GetInvocationList())
                    {
                        await Task.Factory.FromAsync(handler.BeginInvoke(this, e, null, null), handler.EndInvoke);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[BroadcastMessage] - An exception occured broardcasting message", ex);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks if connection exists.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        private bool CheckIfConnectionExists(string connectionName)
        {
            return GetConnection(connectionName) != null;
        }

        /// <summary>
        /// Gets the connection handler.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        private MessageEventHandler GetConnectionHandler(string connectionName)
        {
            MessageEventHandler connectionHandler = null;
            var connection = GetConnection(connectionName);
            if (connection != null && MPDisplayConnectionList.TryGetValue(connection, out connectionHandler))
            {
                return connectionHandler;
            }
            return null;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        private APIConnection GetConnection(string connectionName)
        {
            return MPDisplayConnectionList.Keys.FirstOrDefault(con => con.ConnectionName.Equals(connectionName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }

    #endregion

}

