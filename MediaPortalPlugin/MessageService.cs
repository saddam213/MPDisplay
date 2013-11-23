using MediaPortal.GUI.Library;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using Common.Logging;
using Common.Settings;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Microsoft.Win32;

namespace MediaPortalPlugin
{
    /// <summary>
    /// MessageService for sending and receiving messages from MPDisplay
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MessageService : IMessageCallback
    {
        #region Singleton Implementation

        private static MessageService instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="MessageService"/> class from being created.
        /// </summary>
        private MessageService()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(MessageService));
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
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

        /// <summary>
        /// Initializes the message service.
        /// </summary>
        /// <param name="settings">The settings.</param>
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
        private Common.Logging.Log Log;
        private System.Threading.Timer _keepAlive;
        private bool _isDisconnecting = false;
        private DateTime _lastKeepAlive = DateTime.Now.AddMinutes(2); 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [is connected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is connected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is mp display connected].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is mp display connected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsMPDisplayConnected { get; set; } 

        #endregion

        #region Connection

        /// <summary>
        /// Initializes the connection.
        /// </summary>
        /// <param name="settings">The settings.</param>
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
                _serverBinding.ReceiveTimeout = new TimeSpan(0, 1, 0);
                _serverBinding.SendTimeout = new TimeSpan(0, 1, 0);
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
                    _messageClient.InnerChannel.Faulted -= OnConnectionFaulted;
                    _messageClient.ConnectCompleted -= OnConnectCompleted;
                    _messageClient.SendListMessageCompleted -= _messageClient_SendMessageCompleted;
                    _messageClient.SendInfoMessageCompleted -= _messageClient_SendMessageCompleted;
                    _messageClient.SendDataMessageCompleted -= _messageClient_SendMessageCompleted;
                    _messageClient.SendPropertyMessageCompleted -= _messageClient_SendMessageCompleted;
                    _messageClient = null;
                }

                _messageClient = new MessageClient(site, _serverBinding, _serverEndpoint);
                _messageClient.InnerChannel.Faulted += OnConnectionFaulted;
                _messageClient.ConnectCompleted += OnConnectCompleted;
                _messageClient.SendListMessageCompleted += _messageClient_SendMessageCompleted;
                _messageClient.SendInfoMessageCompleted += _messageClient_SendMessageCompleted;
                _messageClient.SendDataMessageCompleted += _messageClient_SendMessageCompleted;
                _messageClient.SendPropertyMessageCompleted += _messageClient_SendMessageCompleted;
            
                _connection = new APIConnection("MediaPortalPlugin");
                ConnectToService();
            }
            catch (Exception ex)
            {
                Log.Exception("[Initialize] - An exception occured initializing server connection.", ex);
            }
        }

        void _messageClient_SendMessageCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Exception("An error occured sending message to MPDisplay.", e.Error);
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public void Shutdown()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            Log.Message(LogLevel.Info, "[Shutdown] - Shuting down connection instance.");
            _isDisconnecting = true;
            Disconnect();
        }

     

        /// <summary>
        /// Starts the keep alive.
        /// </summary>
        private void StartKeepAlive()
        {
            StopKeepAlive();
            if (_keepAlive == null)
            {
                Log.Message(LogLevel.Debug, "[KeepAlive] - Starting KeepAlive thread.");
                _keepAlive = new System.Threading.Timer(SendKeepAlive, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            }
        }

        /// <summary>
        /// Stops the keep alive.
        /// </summary>
        private void StopKeepAlive()
        {
            if (_keepAlive != null)
            {
                Log.Message(LogLevel.Debug, "[KeepAlive] - Stopping KeepAlive thread.");
                _keepAlive.Change(Timeout.Infinite, Timeout.Infinite);
                _keepAlive = null;
            }
        }

        /// <summary>
        /// Sends the keep alive.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SendKeepAlive(object state)
        {
            if (!IsConnected || DateTime.Now > _lastKeepAlive.AddSeconds(35))
            {
                Reconnect();
                return;
            }
            SendKeepAliveMessage();
        }

        /// <summary>
        /// Connects to service.
        /// </summary>
        public void ConnectToService()
        {
            if (_messageClient != null)
            {
                Log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
                _messageClient.ConnectAsync(_connection);
            }
        }

        /// <summary>
        /// Called when connect completes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ConnectCompletedEventArgs"/> instance containing the event data.</param>
        private void OnConnectCompleted(object sender, ConnectCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Message(LogLevel.Error, "[Connect] - Connection to server failed. Error: {0}", e.Error.Message);
            }
            else
            {
                _lastKeepAlive = DateTime.Now;
                Log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                IsConnected = true;
                IsMPDisplayConnected = e.Result.Where(x => !x.ConnectionName.Equals("MediaPortalPlugin") && !x.ConnectionName.Equals("TVServerPlugin")).Any();
                if (IsMPDisplayConnected)
                {
                    WindowManager.Instance.SendFullUpdate();
                }
            }
            StartKeepAlive();
        }
     

        /// <summary>
        /// Reconnects this session.
        /// </summary>
        public void Reconnect()
        {
            if (!_isDisconnecting)
            {
                Log.Message(LogLevel.Info, "[Reconnect] - Reconnecting to server.");
                Disconnect();
                InitializeConnection(_settings);
            }
        }

        /// <summary>
        /// Disconnects this session.
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            IsMPDisplayConnected = false;
            if (_messageClient != null)
            {
                try
                {
                    StopKeepAlive();
                    Log.Message(LogLevel.Info, "[Disconnect] - Disconnecting from server.");
                    _messageClient.Disconnect();
                }
                catch { }
            }
        }

        /// <summary>
        /// Called when a Session is connected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionConnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - TVServerPlugin connected to network.");
                }
                else if (!connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - MPDisplay instance connected to network. ConnectionName: {0}", connection.ConnectionName);
                    IsMPDisplayConnected = true;
                    WindowManager.Instance.SendFullUpdate();
                }
            }
        }

        /// <summary>
        /// Called when a Session is disconnected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionDisconnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
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

        /// <summary>
        /// Called when connection faulted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnConnectionFaulted(object sender, EventArgs e)
        {
            Log.Message(LogLevel.Error, "[Faulted] - Server connection has faulted");
            Reconnect();
        } 

        #endregion  

        #region Send/Receive

        /// <summary>
        /// Receives the mediaportal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
        {
            Log.Message(LogLevel.Debug, "[Receive] - Message received, MessageType: {0}.", message.MessageType);
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
                      //  Log.Message(LogLevel.Verbose, "[Send] - Sending property message, Property: {0}, Type: {1}.", property.SkinTag, property.PropertyType);
                        _messageClient.SendPropertyMessageAsync(property);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, "[SendPropertyMessage] - An exception occured sending message.", ex.Message);
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
                        if (listMessage.MessageType == APIListMessageType.List && listMessage.List.BatchCount != -1)
                        {
                            Log.Message(LogLevel.Verbose, "[SendListMessage] - Sending list batch, BatchId: {0}, BatchNumber: {1}, BatchCount: {2}", listMessage.List.BatchId, listMessage.List.BatchNumber, listMessage.List.BatchCount);
                        }
                        _messageClient.SendListMessageAsync(listMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, "[SendListMessage] - An exception occured sending message.", ex.Message);
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
                Log.Message(LogLevel.Error, "[SendInfoMessage] - An exception occured sending message.", ex.Message);
            }
        }

        /// <summary>
        /// Sends the data message.
        /// </summary>
        /// <param name="dataMessage">The data message.</param>
        public void SendDataMessage(APIDataMessage dataMessage)
        {
            try
            {
                if (IsConnected && IsMPDisplayConnected)
                {
                    if (_messageClient != null && dataMessage != null)
                    {
                        if (dataMessage.DataType != APIDataMessageType.EQData)
                        {
                            Log.Message(LogLevel.Verbose, "[Send] - Sending data message, MessageType: {0}.", dataMessage.DataType);
                        }
                        _messageClient.SendDataMessageAsync(dataMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Error, "[SendDataMessage] - An Exception Occured Processing Message", ex.Message);
            }
        }

        /// <summary>
        /// Sends the keep alive message.
        /// </summary>
        public void SendKeepAliveMessage()
        {
            try
            {
                if (IsConnected)
                {
                    if (_messageClient != null)
                    {
                        Log.Message(LogLevel.Debug, "[KeepAlive] - Sending KeepAlive message.");
                        _messageClient.SendDataMessageAsync(new APIDataMessage {  DataType = APIDataMessageType.KeepAlive});
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[KeepAlive] - An Exception Occured sending KeepAlive message", ex);
            }
        }

        /// <summary>
        /// Receives the API data message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ReceiveAPIDataMessage(APIDataMessage message)
        {
            if (message != null)
            {
                if (message.DataType == APIDataMessageType.KeepAlive)
                {
                    _lastKeepAlive = DateTime.Now;
                    return;
                }
            }
        } 

        public void ReceiveTVServerMessage(APITVServerMessage message) { }
        public void ReceiveAPIPropertyMessage(APIPropertyMessage message) { }
        public void ReceiveAPIListMessage(APIListMessage message) { }
        public void ReceiveAPIInfoMessage(APIInfoMessage message) { }

        #endregion

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Thread.Sleep(_settings.ResumeDelay);
                    InitializeConnection(_settings);
                });
            }

            if (e.Mode == PowerModes.Suspend)
            {
                Disconnect();
            }
        }
    }
}
