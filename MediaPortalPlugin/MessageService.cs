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
            _settings = settings;
            _serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port));
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
            _serverBinding.ListenBacklog = 1;
            _serverBinding.MaxConnections = 10;
            _serverBinding.MaxReceivedMessageSize = 1000000;
            _serverBinding.MaxBufferSize = 1000000;
            _serverBinding.MaxBufferPoolSize = 1000000;

            // Message
            _serverBinding.ReaderQuotas.MaxArrayLength = 1000000;
            _serverBinding.ReaderQuotas.MaxDepth = 32;
            _serverBinding.ReaderQuotas.MaxStringContentLength = 1000000;
            _serverBinding.ReaderQuotas.MaxBytesPerRead = 1000000;
            _serverBinding.ReliableSession.Enabled = true;
            _serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)

            InstanceContext site = new InstanceContext(this);
            _messageClient = new MessageClient(site, _serverBinding, _serverEndpoint);

            //Remove any old events, otherwise when we reconnect we're creating extra events
            _messageClient.InnerChannel.Faulted -= Channel_Faulted;
            _messageClient.InnerChannel.Faulted += new EventHandler(Channel_Faulted);

            _connection = new APIConnection("MediaPortalPlugin");
            ConnectToService();
        }

        public void ConnectToService()
        {
            IsConnected = false;
            IsMPDisplayConnected = false;
            if (_messageClient != null)
            {
                _messageClient.ConnectAsync(_connection);
            }
        }

        public void Reconnect()
        {
            Disconnect();
            InitializeConnection(_settings);
            ConnectToService();
        }

        public void Disconnect()
        {
            IsConnected = false;
            IsMPDisplayConnected = false;
            if (_messageClient != null)
            {
                try
                {
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
                else if (!connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    IsMPDisplayConnected = true;
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
                }
                else if (!connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    IsMPDisplayConnected = false;
                }
            }
        }


        private void Channel_Faulted(object sender, EventArgs e)
        {
            Log.Message(LogLevel.Error, "[Channel_Faulted] - Server connection has faulted");
            IsConnected = false;
            IsMPDisplayConnected = false;
        }


        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
        {
            if (message != null)
            {
                Log.Message(LogLevel.Verbose, "[ReceiveMediaPortalMessage] - MediaPortalMessage received from MPDisplay, MessageType: {0}", message.MessageType);
                if (message.MessageType == APIMediaPortalMessageType.WindowInfoMessage)
                {
                    if (message.WindowMessage != null)
                    {
                        WindowManager.Instance.OnWindowMessageReceived(message.WindowMessage);
                    }
                }
                else if (message.MessageType == APIMediaPortalMessageType.DialogInfoMessage)
                {
                    if (message.WindowMessage != null)
                    {
                        WindowManager.Instance.OnDialogMessageReceived(message.WindowMessage);
                    }
                }

                if (message.MessageType == APIMediaPortalMessageType.ActionMessage)
                {
                    if (message.ActionMessage != null)
                    {
                        Log.Message(LogLevel.Verbose, "[ReceiveMediaPortalMessage] - Processing ActionMessage, ActionType: {0}", message.ActionMessage.ActionType);
                        if (message.ActionMessage.ActionType == APIActionMessageType.ListAction)
                        {
                            ListManager.Instance.OnActionMessageReceived(message.ActionMessage);
                        }
                        else if (message.ActionMessage.ActionType == APIActionMessageType.MediaPortalAction)
                        {
                            WindowManager.Instance.OnActionMessageReceived(message.ActionMessage);
                        }
                    }
                }
            }
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
