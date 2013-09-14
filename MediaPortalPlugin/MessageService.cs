using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;
using MediaPortal.GUI.Library;
using System.Threading;

namespace MediaPortalPlugin
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MessageService : IMessageCallback
    {
        #region Singleton Implementation

        private static MessageService instance;

        private MessageService()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(MessageService));
        }

        public static MessageService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageService();
                }
                return instance;
            }
        }

        public static void InitializeMessageService(ConnectionSettings settings)
        {
            Instance.InitializeConnection(settings);
        }

        #endregion

        #region Fields

        private APIConnection _connection;
        private MessageClient _messageClient;
        private EndpointAddress _serverEndpoint;
        private NetTcpBinding _serverBinding;
        private ConnectionSettings _settings;
        private MPDisplay.Common.Log.Log Log;
        private System.Threading.Timer _keepAlive;
        private bool _isDisconnecting = false; 

        #endregion

        #region Properties

        public bool IsConnected { get; set; }
        public bool IsMPDisplayConnected { get; set; } 

        #endregion

        #region Connection

        public void InitializeConnection(ConnectionSettings settings)
        {
            try
            {
                _settings = settings;
                string connectionString = string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port);
                Log.Message(LogLevel.Info, "[Initialize] - Initializing server connection. Connection: {0}", connectionString);
                _serverEndpoint = new EndpointAddress(connectionString);
                _serverBinding = new NetTcpBinding();

                // Security (lol)
                _serverBinding.Security.Mode = SecurityMode.None;
                _serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                _serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                _serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

                // Connection
                _serverBinding.Name = "NetTcpBinding_IMessage";
                _serverBinding.CloseTimeout = new TimeSpan(0, 0, 5);
                _serverBinding.OpenTimeout = new TimeSpan(0, 0, 5);
                _serverBinding.ReceiveTimeout = new TimeSpan(0, 0, 30);
                _serverBinding.SendTimeout = new TimeSpan(0, 0, 30);
                _serverBinding.TransferMode = TransferMode.Buffered;
                _serverBinding.ListenBacklog = 100;
                _serverBinding.MaxConnections = 100;
                _serverBinding.MaxReceivedMessageSize = int.MaxValue;
                _serverBinding.MaxBufferSize = int.MaxValue;
                _serverBinding.MaxBufferPoolSize = int.MaxValue;

                // Message
                _serverBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                _serverBinding.ReaderQuotas.MaxDepth = 32;
                _serverBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                _serverBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                _serverBinding.ReliableSession.Enabled = true;
                _serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(0, 3, 0);

                InstanceContext site = new InstanceContext(this);
                if (_messageClient != null)
                {
                    _messageClient.InnerChannel.Faulted -= ConnectionFaulted;
                    _messageClient.ConnectCompleted -= ConnectCompleted;
                    _messageClient = null;
                }

                _messageClient = new MessageClient(site, _serverBinding, _serverEndpoint);
                _messageClient.InnerChannel.Faulted += ConnectionFaulted;
                _messageClient.ConnectCompleted += ConnectCompleted;

                _connection = new APIConnection("MediaPortalPlugin");
                ConnectToService();
                StartKeepAlive();
            }
            catch (Exception ex)
            {
                Log.Exception("[Initialize] - An exception occured initializing server connection.", ex);
            }
        }

        private void ConnectCompleted(object sender, ConnectCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Message(LogLevel.Error, "[Connect] - Connection to server failed. Error: {0}", e.Error.Message);
            }
            else
            {
                Log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                foreach (var connection in e.Result.Where(c => !c.ConnectionName.Equals("MediaPortalPlugin")))
                {
                    SessionConnected(connection);
                }
            }
        }

        private void StartKeepAlive()
        {
            StopKeepAlive();
            if (_keepAlive == null)
            {
                Log.Message(LogLevel.Info, "[KeepAlive] - Staring KeepAlive thread.");
                _keepAlive = new System.Threading.Timer(SendKeepAlive, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            }
        }

        private void StopKeepAlive()
        {
            if (_keepAlive != null)
            {
                Log.Message(LogLevel.Info, "[KeepAlive] - Stopping KeepAlive thread.");
                _keepAlive.Change(Timeout.Infinite, Timeout.Infinite);
                _keepAlive = null;
            }
        }

        private void SendKeepAlive(object state)
        {
            if (!IsConnected)
            {
                Reconnect();
                return;
            }
            Log.Message(LogLevel.Info, "[KeepAlive] - Sending connection KeepAlive message.");
            SendDataMessage(new APIDataMessage { DataType = APIDataMessageType.KeepAlive });
        }

        public void ConnectToService()
        {
            IsConnected = false;
            IsMPDisplayConnected = false;
            if (_messageClient != null)
            {
                Log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
                _messageClient.ConnectAsync(_connection);
            }
        }

        public void Shutdown()
        {
            StopKeepAlive();
            Log.Message(LogLevel.Info, "[Shutdown] - Shuting down connection instance.");
            _isDisconnecting = true;
            Disconnect();
        }

        public void Reconnect()
        {
            if (!_isDisconnecting)
            {
                Log.Message(LogLevel.Info, "[Reconnect] - Reconnecting to server.");
                Disconnect();
                InitializeConnection(_settings);
            }
        }

        public void Disconnect()
        {
            IsConnected = false;
            IsMPDisplayConnected = false;
            if (_messageClient != null)
            {
                try
                {
                    Log.Message(LogLevel.Info, "[Disconnect] - Disconnecting from server.");
                    _messageClient.Disconnect();
                }
                catch { }
            }
        }

        public void SessionConnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    IsConnected = true;
                }
                else if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - TVServerPlugin connected to network.");
                }
                else
                {
                    Log.Message(LogLevel.Info, "[Session] - MPDisplay instance connected to network. ConnectionName: {0}", connection.ConnectionName);
                    IsMPDisplayConnected = true;
                    WindowManager.Instance.SendFullUpdate();
                }
            }
        }

        public void SessionDisconnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    IsConnected = false;
                    IsMPDisplayConnected = false;
                    Reconnect();
                }
                else if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - TVServerPlugin disconnected from network.");
                }
                else
                {
                    Log.Message(LogLevel.Info, "[Session] - MPDisplay instance disconnected from network. ConnectionName: {0}", connection.ConnectionName);
                    IsMPDisplayConnected = false;
                }
            }
        }

        private void ConnectionFaulted(object sender, EventArgs e)
        {
            Log.Message(LogLevel.Error, "[Faulted] - Server connection has faulted");
            IsConnected = false;
            IsMPDisplayConnected = false;
        } 

        #endregion  

        #region Send/Receive

        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
        {
            Log.Message(LogLevel.Verbose, "[Receive] - Message received, MessageType: {0}.", message.MessageType);
            WindowManager.Instance.OnMediaPortalMessageReceived(message);
        }

        /// <summary>
        /// Sends the MP property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void SendPropertyMessage(APIPropertyMessage property)
        {
            try
            {
                if (IsConnected && IsMPDisplayConnected)
                {
                    if (_messageClient != null && property != null)
                    {
                        Log.Message(LogLevel.Verbose, "[Send] - Sending property message, Property: {0}, Type: {1}.", property.SkinTag, property.PropertyType);
                        _messageClient.SendPropertyMessageAsync(property);
                    }
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
        public void SendListMessage(APIListMessage listMessage)
        {
            try
            {
                if (IsConnected && IsMPDisplayConnected)
                {
                    if (_messageClient != null && listMessage != null)
                    {
                        Log.Message(LogLevel.Verbose, "[Send] - Sending list message, MessageType: {0}.", listMessage.MessageType);
                        _messageClient.SendListMessageAsync(listMessage);
                    }
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
        public void SendInfoMessage(APIInfoMessage infoMessage)
        {
            try
            {
                if (IsConnected && IsMPDisplayConnected)
                {
                    if (_messageClient != null && infoMessage != null)
                    {
                        Log.Message(LogLevel.Verbose, "[Send] - Sending info message, MessageType: {0}.", infoMessage.MessageType);
                        _messageClient.SendInfoMessageAsync(infoMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendInfoMessage] - An exception occured sending message.", ex);
            }
        }

        public void SendDataMessage(APIDataMessage dataMessage)
        {
            try
            {
                if (IsConnected && IsMPDisplayConnected)
                {
                    if (_messageClient != null && dataMessage != null)
                    {
                        Log.Message(LogLevel.Verbose, "[Send] - Sending data message, MessageType: {0}.", dataMessage.DataType);
                        _messageClient.SendDataMessage(dataMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendDataMessage] - An Exception Occured Processing Message", ex);
            }
        }

        public void ReceiveTVServerMessage(APITVServerMessage message) { }
        public void ReceiveAPIPropertyMessage(APIPropertyMessage message) { }
        public void ReceiveAPIListMessage(APIListMessage message) { }
        public void ReceiveAPIInfoMessage(APIInfoMessage message) { }
        public void ReceiveAPIDataMessage(APIDataMessage message) { } 

        #endregion
    }
}
