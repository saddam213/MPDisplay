using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Common.Log;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

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
        /// <param name="message">The message.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveAPIPropertyMessage(APIPropertyMessage message);

        /// <summary>
        /// Receives the MPGUI list message.
        /// </summary>
        /// <param name="message">The message.</param>
        [OperationContract(IsOneWay = true)]
        Task ReceiveAPIListMessage(APIListMessage message);

        /// <summary>
        /// Receives the MP multi message.
        /// </summary>
        /// <param name="message">The message.</param>
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
        /// <param name="connection">The connection.</param>
        [OperationContract(IsOneWay = true)]
        Task SessionConnected(APIConnection connection);

        /// <summary>
        /// Users the disconneted.
        /// </summary>
        /// <param name="connection">The connection.</param>
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

    public class MessageEventArgs
    {
        public MessageEventArgs(MessageType messageType, APIConnection connection, object data = null)
        {
            MessageType = messageType;
            Connection = connection;
            Data = data;
        }

        public MessageType MessageType { get; set; }
        public APIConnection Connection { get; set; }
        public object Data { get; set; }
    }

    #endregion

    #region MessageService


    public static class ConnectionManager
    {
        private static Log _log = LoggingManager.GetLog(typeof(ConnectionManager));
        private static Dictionary<APIConnection, Action<MessageEventArgs>> _activeConnections = new Dictionary<APIConnection, Action<MessageEventArgs>>();
        public static Dictionary<APIConnection, Action<MessageEventArgs>> ActiveConnections
        {
            get { return _activeConnections; }
        }
        private static object _syncObj = new object();

        public static void AddConnection(APIConnection connection, Action<MessageEventArgs> callback)
        {
            RemoveConnection(connection);
            if (connection != null && callback != null)
            {
                _log.Message(LogLevel.Info, "Adding new connection, Connection: {0}", connection.ConnectionName);
                lock (_syncObj)
                {
                    ActiveConnections.Add(connection, callback);
                }
            }
        }

        public static void RemoveConnection(APIConnection connection)
        {

            if (connection != null)
            {
                var existing = ActiveConnections.Keys.FirstOrDefault(x => x.ConnectionName == connection.ConnectionName);
                if (existing != null)
                {
                    _log.Message(LogLevel.Info, "Removing existing connection, Connection: {0}", connection.ConnectionName);
                    lock (_syncObj)
                    {
                        ActiveConnections.Remove(existing);
                    }
                }
            }
        }

        public static List<APIConnection> GetConnections()
        {
            return ActiveConnections.Keys.ToList();
        }

        public static IEnumerable<Action<MessageEventArgs>> GetAllCallbacksExcept(APIConnection exception)
        {
            foreach (var connection in _activeConnections)
            {
                if (exception == null || connection.Key.ConnectionName != exception.ConnectionName)
                {
                    yield return connection.Value;
                }
            }
        }

        public static Task ToTask(this Action<MessageEventArgs> callback, MessageEventArgs message)
        {
            return Task.Factory.StartNew(() => callback(message));
        }

        public static Action<MessageEventArgs> GetCallbackForConnection(string connectionName)
        {
            return _activeConnections.FirstOrDefault(kv => kv.Key.ConnectionName == connectionName).Value;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MessageService : IMessage
    {
        #region Vars

    
        private Log _log = LoggingManager.GetLog(typeof(MessageService));
        private IMessageCallback _messageCallback;
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
                    _log.Message(LogLevel.Info, "[Connect] - New connection request received, ConnectionName: {0}", connection.ConnectionName);
                    ConnectionManager.RemoveConnection(connection);

                    _apiConnection = connection;
                    _messageCallback = OperationContext.Current.GetCallbackChannel<IMessageCallback>();
                    ConnectionManager.AddConnection(connection, CallbackEventHandler);
             
                    await BroadcastMessage(new MessageEventArgs(MessageType.SessionConnected, _apiConnection));
                 
                    _log.Message(LogLevel.Info, "[Connect] - Successfully established connection, ConnectionName: {0}", connection.ConnectionName);
                    return ConnectionManager.GetConnections();
                }
                else
                {
                    _log.Message(LogLevel.Error, "[Connect] - Failed to create new connection.");
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[Connect] - An exception occured while connecting.", ex);
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
                _log.Message(LogLevel.Info, "[Disconnect] - Disconnection request received, ConnectionName: {0}", _apiConnection.ConnectionName);

                ConnectionManager.RemoveConnection(_apiConnection);

                await BroadcastMessage(new MessageEventArgs(MessageType.SessionDisconnected, _apiConnection));

                _log.Message(LogLevel.Info, "[Disconnect] - Successfully removed connection instance, ConnectionName: {0}", _apiConnection.ConnectionName);
                _apiConnection = null;
                _messageCallback = null;
            }
            catch (Exception ex)
            {
                _log.Exception("[Disconnect] - An exception occured while disconnecting.", ex);
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
                    await BroadcastMessage(new MessageEventArgs(MessageType.ReceiveAPIPropertyMessage, _apiConnection, property));
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[SendPropertyMessage] - An exception occured sending message.", ex);
            }
        }

        /// <summary>
        /// Sends the MPGUI list.
        /// </summary>
        /// <param name="listMessage">The list message.</param>
        public async Task SendListMessage(APIListMessage listMessage)
        {
            try
            {
                if (_apiConnection != null && listMessage != null)
                {
                    _log.Message(LogLevel.Debug, "[SendListMessage] - Sending List message to MPDisplay, Sender: {0}, MessageType: {1}"
                        , _apiConnection.ConnectionName, listMessage.MessageType);

                    await BroadcastMessage(new MessageEventArgs(MessageType.ReceiveAPIListMessage, _apiConnection, listMessage));
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[SendListMessage] - An exception occured sending message.", ex);
            }
        }

        /// <summary>
        /// Sends the MP multi message.
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        public async Task SendInfoMessage(APIInfoMessage infoMessage)
        {
            try
            {
                if (_apiConnection != null && infoMessage != null)
                {
                    _log.Message(LogLevel.Debug, "[SendInfoMessage] - Sending Info message to MPDisplay, Sender: {0}, InfoType: {1}"
                        , _apiConnection.ConnectionName, infoMessage.MessageType);
                    await BroadcastMessage(new MessageEventArgs(MessageType.ReceiveAPIInfoMessage, _apiConnection, infoMessage));
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[SendInfoMessage] - An exception occured sending message.", ex);
            }
        }

        public async Task SendDataMessage(APIDataMessage dataMessage)
        {
            try
            {
                if (_apiConnection != null && dataMessage != null)
                {
                    var message = new MessageEventArgs(MessageType.ReceiveAPIDataMessage, _apiConnection, dataMessage);
                    if (dataMessage.DataType == APIDataMessageType.KeepAlive)
                    {
                        _log.Message(LogLevel.Debug, "[KeepAlive] - KeepAlive received., Sender: {0}", _apiConnection.ConnectionName);
                        CallbackEventHandler(message);
                        return;
                    }
                    if (dataMessage.DataType != APIDataMessageType.EQData)
                    {
                        _log.Message(LogLevel.Debug, "[SendDataMessage] - Sending message to MPDisplay, Sender: {0}, DataType: {1}"
                            , _apiConnection.ConnectionName, dataMessage.DataType);
                    }
                    await BroadcastMessage(message);
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[SendDataMessage] - An Exception Occured Processing Message", ex);
            }
        }

        /// <summary>
        /// Sends to MediaPortal.
        /// </summary>
        /// <param name="mediaPortalMessage">The MediaPortal message.</param>
        public async Task SendMediaPortalMessage(APIMediaPortalMessage mediaPortalMessage)
        {
            try
            {
                if (_apiConnection != null && mediaPortalMessage != null)
                {
                    _log.Message(LogLevel.Debug, "[SendMediaPortalMessage] - Sending message to MediaPortal plugin, Sender: {0}, MessageType: {1}"
                        , _apiConnection.ConnectionName, mediaPortalMessage.MessageType);
                    await BroadcastMessage("MediaPortalPlugin", new MessageEventArgs(MessageType.ReceiveMediaPortalMessage, _apiConnection, mediaPortalMessage));
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[SendMediaPortalMessage] - An exception occured sending message.", ex);
            }
        }

        /// <summary>
        /// Sends to MediaPortal.
        /// </summary>
        /// <param name="tvServerMessage">The TVServer message.</param>
        public async Task SendTVServerMessage(APITVServerMessage tvServerMessage)
        {
            try
            {
                if (_apiConnection != null && tvServerMessage != null)
                {
                    _log.Message(LogLevel.Debug, "[SendTVServerMessage] - Sending message to TVServer plugin, Sender: {0}", _apiConnection.ConnectionName);
                    await BroadcastMessage("TVServerPlugin", new MessageEventArgs(MessageType.ReceiveTVServerMessage, _apiConnection, tvServerMessage));
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[SendTVServerMessage] - An exception occured sending message.", ex);
            }
        }

   

        #endregion

        #region private methods

        /// <summary>
        /// Callbacks the event handler.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private void CallbackEventHandler(MessageEventArgs e)
        {
            try
            {
                if (_messageCallback != null)
                {
                    switch (e.MessageType)
                    {
                        case MessageType.ReceiveAPIPropertyMessage:
                            _messageCallback.ReceiveAPIPropertyMessage(e.Data as APIPropertyMessage);
                            break;
                        case MessageType.ReceiveAPIListMessage:
                            _messageCallback.ReceiveAPIListMessage(e.Data as APIListMessage);
                            break;
                        case MessageType.ReceiveAPIInfoMessage:
                            _messageCallback.ReceiveAPIInfoMessage(e.Data as APIInfoMessage);
                            break;
                        case MessageType.ReceiveAPIDataMessage:
                            _messageCallback.ReceiveAPIDataMessage(e.Data as APIDataMessage);
                            break;
                        case MessageType.ReceiveMediaPortalMessage:
                            _messageCallback.ReceiveMediaPortalMessage(e.Data as APIMediaPortalMessage);
                            break;
                        case MessageType.ReceiveTVServerMessage:
                            _messageCallback.ReceiveTVServerMessage(e.Data as APITVServerMessage);
                            break;
                        case MessageType.SessionConnected:
                            _messageCallback.SessionConnected(e.Connection);
                            break;
                        case MessageType.SessionDisconnected:
                            _messageCallback.SessionDisconnected(e.Connection);
                            break;
                    }
                }
                return;
            }
            catch (ObjectDisposedException ex)
            {
                _log.Exception("[CallbackHandler] - Unable to send message to destination, Destination does not exist", ex);
            }
            catch (Exception ex)
            {
                _log.Exception("[CallbackHandler] - An exception occured while invoking callback.{0}", ex);
            }
            Disconnect().RunSynchronously();
        }

        /// <summary>
        /// Broadcasts the message.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        private async Task BroadcastMessage(MessageEventArgs e)
        {
            try
            {
                await Task.WhenAll(ConnectionManager.GetAllCallbacksExcept(_apiConnection).Select(x => x.ToTask(e)));
            }
            catch (Exception ex)
            {
                _log.Exception("[BroadcastMessage] - An exception occured broardcasting message", ex);
            }
        }

        private async Task BroadcastMessage(string connectionName, MessageEventArgs e)
        {
            try
            {
                var callback = ConnectionManager.GetCallbackForConnection(connectionName);
                if (callback != null)
                {
                    await callback.ToTask(e);
                }
                else
                {
                    _log.Message(LogLevel.Warn, "[BroadcastMessage] - Message cannot be sent because the connection '{0}' does not exist or cannot be found.", connectionName);
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[BroadcastMessage] - An exception occured broardcasting message to " + connectionName, ex);
            }
        }

        #endregion
    }

    #endregion

}

