﻿using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
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

        private static MessageService _instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="MessageService"/> class from being created.
        /// </summary>
        private MessageService()
        {
            _log = LoggingManager.GetLog(typeof(MessageService));
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static MessageService Instance
        {
            get { return _instance ?? (_instance = new MessageService()); }
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
        private Log _log;
        private Timer _keepAlive;
        private bool _isDisconnecting;
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
        public bool IsMpDisplayConnected { get; set; }


        public bool IsSkinEditorConnected { get; set; }


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
                var connectionString = string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port);
                _log.Message(LogLevel.Info, "[Initialize] - Initializing server connection. Connection: {0}", connectionString);
                _serverEndpoint = new EndpointAddress(connectionString);
                _serverBinding = ConnectHelper.GetServerBinding();

                var site = new InstanceContext(this);
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
                _log.Exception("[Initialize] - An exception occured initializing server connection.", ex);
            }
        }

        void _messageClient_SendMessageCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _log.Exception("An error occured sending message to MPDisplay.", e.Error);
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public void Shutdown()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            _log.Message(LogLevel.Info, "[Shutdown] - Shuting down connection instance.");
            _isDisconnecting = true;
            Disconnect();
        }

     

        /// <summary>
        /// Starts the keep alive.
        /// </summary>
        private void StartKeepAlive()
        {
            StopKeepAlive();
            if (_keepAlive != null) return;

            _log.Message(LogLevel.Debug, "[KeepAlive] - Starting KeepAlive thread.");
            _keepAlive = new Timer(SendKeepAlive, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Stops the keep alive.
        /// </summary>
        private void StopKeepAlive()
        {
            if (_keepAlive == null) return;

            _log.Message(LogLevel.Debug, "[KeepAlive] - Stopping KeepAlive thread.");
            _keepAlive.Change(Timeout.Infinite, Timeout.Infinite);
            _keepAlive = null;
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
            if (_messageClient == null) return;

            _log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
            _messageClient.ConnectAsync(_connection);
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
                _log.Message(LogLevel.Error, "[Connect] - Connection to server failed. Error: {0}", e.Error.Message);
            }
            else
            {
                _lastKeepAlive = DateTime.Now;
                _log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                IsConnected = true;
                IsMpDisplayConnected = e.Result.Any(x => !x.ConnectionName.Equals("MediaPortalPlugin") && !x.ConnectionName.Equals("SkinEditor"));
                IsSkinEditorConnected = e.Result.Any(x => x.ConnectionName.Equals("SkinEditor"));
                if (IsMpDisplayConnected)
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
            if (_isDisconnecting) return;

            _log.Message(LogLevel.Info, "[Reconnect] - Reconnecting to server.");
            Disconnect();
            InitializeConnection(_settings);
        }

        /// <summary>
        /// Disconnects this session.
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            IsMpDisplayConnected = false;
            IsSkinEditorConnected = false;
            if (_messageClient == null) return;

            try
            {
                StopKeepAlive();
                _log.Message(LogLevel.Info, "[Disconnect] - Disconnecting from server.");
                _messageClient.Disconnect();
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Called when a Session is connected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionConnected(APIConnection connection)
        {
            if (connection == null) return;

            if (connection.ConnectionName.Equals("SkinEditor"))
            {
                _log.Message(LogLevel.Info, "[Session] - SkinEditor connected to network.");
                IsSkinEditorConnected = true;
            }
            else if (!connection.ConnectionName.Equals("MediaPortalPlugin"))
            {
                _log.Message(LogLevel.Info, "[Session] - MPDisplay instance connected to network. ConnectionName: {0}", connection.ConnectionName);
                IsMpDisplayConnected = true;
                WindowManager.Instance.SendFullUpdate();
            }
        }

        /// <summary>
        /// Called when a Session is disconnected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionDisconnected(APIConnection connection)
        {
            if (connection == null) return;

            if (connection.ConnectionName.Equals("MediaPortalPlugin"))
            {
                Reconnect();
            }
            else if (connection.ConnectionName.Equals("SkinEditor"))
            {
                _log.Message(LogLevel.Info, "[Session] - SkinEditor disconnected from network.");
                IsSkinEditorConnected = false;
            }
            else
            {
                _log.Message(LogLevel.Info, "[Session] - MPDisplay instance disconnected from network. ConnectionName: {0}", connection.ConnectionName);
                IsMpDisplayConnected = false;
            }
        }

        /// <summary>
        /// Called when connection faulted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnConnectionFaulted(object sender, EventArgs e)
        {
            _log.Message(LogLevel.Error, "[Faulted] - Server connection has faulted");
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
            _log.Message(LogLevel.Debug, "[Receive] - Message received, MessageType: {0}.", message.MessageType);
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
                if (!IsConnected) return;

                if (!IsMpDisplayConnected) return;

                if (_messageClient == null || property == null) return;
                _log.Message(LogLevel.Verbose, "[Send] - Sending property message, Property: {0}, Type: {1}, Label: {2}.", property.SkinTag, property.PropertyType, property.Label); //Test
                _messageClient.SendPropertyMessageAsync(property);
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendPropertyMessage] - An exception occured sending message.", ex.Message);
            }
        }

        /// <summary>
        /// Sends the MPGUI list.
        /// </summary>
        /// <param name="listMessage">The message.</param>
        public void SendListMessage(APIListMessage listMessage)
        {
            try
            {
                if (!IsConnected || !IsMpDisplayConnected) return;

                if (_messageClient == null || listMessage == null) return;

                if (listMessage.MessageType == APIListMessageType.TVGuide)
                {
                    _log.Message(LogLevel.Verbose, "[Send] - Sending list message, MessageType: {0} TVGuideType: {1}.", listMessage.MessageType, listMessage.TvGuide.MessageType);
                }
                else
                {
                    _log.Message(LogLevel.Verbose, "[Send] - Sending list message, MessageType: {0}.", listMessage.MessageType);
                }
                if (listMessage.MessageType == APIListMessageType.List && listMessage.List.BatchCount != -1)
                {
                    _log.Message(LogLevel.Verbose, "[SendListMessage] - Sending list batch, BatchId: {0}, BatchNumber: {1}, BatchCount: {2}", listMessage.List.BatchId, listMessage.List.BatchNumber, listMessage.List.BatchCount);
                }
                _messageClient.SendListMessageAsync(listMessage);
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendListMessage] - An exception occured sending message.", ex.Message);
            }
        }

        /// <summary>
        /// Sends the MP multi message.
        /// </summary>
        /// <param name="infoMessage">The message.</param>
        public void SendInfoMessage(APIInfoMessage infoMessage)
        {
            try
            {
                if (!IsConnected || !IsMpDisplayConnected) return;

                if (_messageClient == null || infoMessage == null) return;

                _log.Message(LogLevel.Verbose, "[Send] - Sending info message, MessageType: {0}.", infoMessage.MessageType);
                _messageClient.SendInfoMessageAsync(infoMessage);
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendInfoMessage] - An exception occured sending message.", ex.Message);
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
                if (!IsConnected || !IsMpDisplayConnected) return;

                if (_messageClient == null || dataMessage == null) return;

                if (dataMessage.DataType != APIDataMessageType.EQData)
                {
                    if (dataMessage.IntArray != null)
                    {
                        _log.Message(LogLevel.Verbose, "[Send] - Sending data message, MessageType: {0}, IntValue: {1}, ArraySize {2}.", dataMessage.DataType, dataMessage.IntValue, dataMessage.IntArray.Count());
                    }
                    else
                    {
                        _log.Message(LogLevel.Verbose, "[Send] - Sending data message, MessageType: {0}, IntValue: {1}.", dataMessage.DataType, dataMessage.IntValue);
                    }
                }
                _messageClient.SendDataMessageAsync(dataMessage);
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendDataMessage] - An Exception Occured Processing Message", ex.Message);
            }
        }

        /// <summary>
        /// Sends the keep alive message.
        /// </summary>
        public void SendKeepAliveMessage()
        {
            try
            {
                if (!IsConnected) return;

                if (_messageClient == null) return;

                _log.Message(LogLevel.Debug, "[KeepAlive] - Sending KeepAlive message.");
                _messageClient.SendDataMessageAsync(new APIDataMessage {  DataType = APIDataMessageType.KeepAlive});
            }
            catch (Exception ex)
            {
                _log.Exception("[KeepAlive] - An Exception Occured sending KeepAlive message", ex);
            }
        }

        /// <summary>
        /// Receives the API data message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ReceiveAPIDataMessage(APIDataMessage message)
        {
            if (message == null) return;

            if (message.DataType == APIDataMessageType.KeepAlive)
            {
                _lastKeepAlive = DateTime.Now;
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
                ThreadPool.QueueUserWorkItem(o =>
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


        public void SendSkinEditorDataMessage(APISkinEditorData message)
        {   
            try
            {
                if (!IsConnected || !IsSkinEditorConnected) return;

                if (_messageClient == null || message == null) return;

                var dataMessage = new APIDataMessage
                {
                    DataType = APIDataMessageType.SkinEditorInfo,
                    SkinEditorData = message
                };
                _messageClient.SendDataMessageAsync(dataMessage);
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendSkinEditorDataMessage] - An Exception Occured Processing Message", ex.Message);
            }
        }
      
    }
}
