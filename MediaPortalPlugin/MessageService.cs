using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MediaPortalPlugin.InfoManagers;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

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

        #endregion


        private APIConnection _connection;
        private MessageClient _messageClient;
        private EndpointAddress _serverEndpoint;
        private NetTcpBinding _serverBinding;
        private ConnectionSettings _settings;
        private MPDisplay.Common.Log.Log Log;

        public bool IsConnected { get; set; }
        public bool IsMPDisplayConnected { get; set; }

        public void InitializeConnection(ConnectionSettings settings)
        {
            try
            {
                _settings = settings;
                string connectionString = string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port);
                Log.Message(LogLevel.Info, "[InitializeConnection] - Initializing server connection. Connection: {0}", connectionString);
                _serverEndpoint = new EndpointAddress(connectionString);
                _serverBinding = new NetTcpBinding();

                // Security (lol)
                _serverBinding.Security.Mode = SecurityMode.None;
                _serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                _serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                _serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

                // Connection
                _serverBinding.Name = "NetTcpBinding_IMessage";
                _serverBinding.CloseTimeout = new TimeSpan(0, 10, 0);
                _serverBinding.OpenTimeout = new TimeSpan(0, 10, 0);
                _serverBinding.ReceiveTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)
                _serverBinding.SendTimeout = new TimeSpan(0, 10, 0);
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
                _serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)

                InstanceContext site = new InstanceContext(this);
                _messageClient = new MessageClient(site, _serverBinding, _serverEndpoint);

                //Remove any old events, otherwise when we reconnect we're creating extra events
                _messageClient.InnerChannel.Faulted -= Channel_Faulted;
                _messageClient.InnerChannel.Faulted += new EventHandler(Channel_Faulted);
                _messageClient.ConnectCompleted += _messageClient_ConnectCompleted;

                _connection = new APIConnection("MediaPortalPlugin");
                ConnectToService();
            }
            catch (Exception ex)
            {
                Log.Exception("[InitializeConnection] - An exception occured initializing server connection.", ex);
            }
        }

        void _messageClient_ConnectCompleted(object sender, ConnectCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Message(LogLevel.Error, "[ConnectToService] - Connection to server failed. Error: {0}", e.Error.Message);
                _retryCount++;
                if (_retryCount < 5)
                {

                    Reconnect();
                }
            }
            else
            {
                Log.Message(LogLevel.Info, "[ConnectToService] - Connection to server successful.");
                WindowManager.Instance.SendFullUpdate();
            }
        }



        public void ConnectToService()
        {
            IsConnected = false;
            IsMPDisplayConnected = false;
            if (_messageClient != null)
            {
                Log.Message(LogLevel.Info, "[ConnectToService] - Connecting to server.");
                _messageClient.ConnectAsync(_connection);
            }
        }

        public void Shutdown()
        {
            Log.Message(LogLevel.Info, "[Shutdown] - Shuting down connection instance.");
            _isDisconnecting = true;
            Disconnect();
        }

        private int _retryCount = 0;

        public void Reconnect()
        {
            if (!_isDisconnecting)
            {
                Log.Message(LogLevel.Info, "[Reconnect] - Reconnecting to server.");
                Disconnect();
                InitializeConnection(_settings);
                ConnectToService();
            }
        }

        private bool _isDisconnecting = false;

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
                    _retryCount = 0;
                    IsConnected = true;
                }
                else if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[SessionConnected] - TVServerPlugin connected to network.");
                }
                else
                {
                    Log.Message(LogLevel.Info, "[SessionConnected] - MPDisplay instabce connected to network. ConnectionName: {0}", connection.ConnectionName);
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
                    Reconnect();
                }
                else if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[SessionConnected] - TVServerPlugin disconnected from network.");
                }
                else
                {
                    Log.Message(LogLevel.Info, "[SessionDisconnected] - MPDisplay instance disconnected from network. ConnectionName: {0}", connection.ConnectionName);
                    IsMPDisplayConnected = false;
                }
            }
        }


        private void Channel_Faulted(object sender, EventArgs e)
        {
            Log.Message(LogLevel.Error, "[Channel_Faulted] - Server connection has faulted");
            Reconnect();
        }


        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
        {
            WindowManager.Instance.OnMediaPortalMessageReceived(message);
        }

        public void ReceiveTVServerMessage(APITVServerMessage message) { }
        public void ReceiveAPIPropertyMessage(APIPropertyMessage message) { }
        public void ReceiveAPIListMessage(APIListMessage message) { }
        public void ReceiveAPIInfoMessage(APIInfoMessage message) { }
        public void ReceiveAPIDataMessage(APIDataMessage message) { }

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
                        _messageClient.SendDataMessage(dataMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[SendDataMessage] - An Exception Occured Processing Message", ex);
            }
        }
    }
}
