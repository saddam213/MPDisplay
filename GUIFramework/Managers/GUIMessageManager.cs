using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using GUISkinFramework.Property;
using MessageFramework;
using MessageFramework.DataObjects;
using MPDisplay.Common.Settings;

namespace GUIFramework.Managers
{

    //public static class GUIMessageManager
    //{

    //    public static MessageClient MessageBroker;

    //    public static async Task SendMediaPortalMessage(APIMediaPortalMessage message)
    //    {
    //        if (MessageBroker != null)
    //        {
    //            await MessageBroker.SendMediaPortalMessageAsync(message);
    //        }
    //    }

    //    public static async Task SendTVServerMessage(APITVServerMessage message)
    //    {
    //        if (MessageBroker != null)
    //        {
    //            await MessageBroker.SendTVServerMessageAsync(message);
    //        }
    //    }


    //    public static async Task SendRegisterInfoMessage(APIWindowInfoMessage message)
    //    {
    //        if (MessageBroker != null && message != null)
    //        {
    //            await SendMediaPortalMessage(new APIMediaPortalMessage
    //            {
    //                MessageType = APIMediaPortalMessageType.WindowInfoMessage,
    //                WindowMessage = message
    //            });
    //        }
    //    }

    //}

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class GUIMessageService : IMessageCallback
    {
        private static GUIMessageService instance;

        private GUIMessageService() { }

        public static GUIMessageService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GUIMessageService();
                }
                return instance;
            }
        }

        private APIConnection _connection;
        private EndpointAddress serverEndpoint;
        private NetTcpBinding serverBinding;
        private ConnectionSettings _settings;
        private MessageClient _messageBroker;
        private bool _isMediaPortalConnected = false;
        private bool _isTVServerConnected = false;
        private bool _isMPDisplayConnected = false;

        public bool IsMPDisplayConnected
        {
            get { return _isMPDisplayConnected; }
            private set
            {
                if (_isMPDisplayConnected != value)
                {
                    _isMPDisplayConnected = value;
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        public bool IsMediaPortalConnected
        {
            get { return _isMediaPortalConnected; }
            private set
            {
                if (_isMediaPortalConnected != value)
                {
                    _isMediaPortalConnected = value;
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        public bool IsTVServerConnected
        {
            get { return _isTVServerConnected; }
            private set
            {
                if (_isTVServerConnected != value)
                {
                    _isTVServerConnected = value;
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }
      
        public void InitializeServerConnection(ConnectionSettings settings)
        {
            _settings = settings;
            serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port));
            serverBinding = new NetTcpBinding();

            // Security (lol)
            serverBinding.Security.Mode = SecurityMode.None;
            serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

            // Connection
            serverBinding.Name = "NetTcpBinding_IMessage";
            serverBinding.CloseTimeout = new TimeSpan(0, 10, 0);
            serverBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            serverBinding.ReceiveTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)
            serverBinding.SendTimeout = new TimeSpan(0, 10, 0);
            serverBinding.TransferMode = TransferMode.Buffered;
            serverBinding.ListenBacklog = 1;
            serverBinding.MaxConnections = 10;
            serverBinding.MaxReceivedMessageSize = 1000000;
            serverBinding.MaxBufferSize = 1000000;
            serverBinding.MaxBufferPoolSize = 1000000;

            // Message
            serverBinding.ReaderQuotas.MaxArrayLength = 1000000;
            serverBinding.ReaderQuotas.MaxDepth = 32;
            serverBinding.ReaderQuotas.MaxStringContentLength = 1000000;
            serverBinding.ReaderQuotas.MaxBytesPerRead = 1000000;
            serverBinding.ReliableSession.Enabled = true;
            serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)

            InstanceContext site = new InstanceContext(this);
            _messageBroker = new MessageClient(site, serverBinding, serverEndpoint);

            //Remove any old events, otherwise when we reconnect we're creating extra events
            _messageBroker.InnerChannel.Faulted -= Channel_Faulted;
            _messageBroker.InnerChannel.Faulted += new EventHandler(Channel_Faulted);

            _connection = new APIConnection(settings.ConnectionName);
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            IsMPDisplayConnected = false;
            IsMediaPortalConnected = false;
            IsTVServerConnected = false;
        }

        public async Task ConnectToService()
        {
            if (_messageBroker != null)
            {
                await Disconnect();
                var task = _messageBroker.ConnectAsync(_connection);
                if (await Task.WhenAny(task, Task.Delay(5000)) == task)
                {
                    foreach (var connection in task.Result)
                    {
                        if (connection.ConnectionName.Equals(_settings.ConnectionName))
                        {
                            IsMPDisplayConnected = true;
                        }

                        if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                        {
                            IsMediaPortalConnected = true;
                        }

                        if (connection.ConnectionName.Equals("TVServerPlugin"))
                        {
                            IsTVServerConnected = true;
                        }
                    }
                }
            }
        }

        public async Task Reconnect()
        {
            await Disconnect();
            InitializeServerConnection(_settings);
            await ConnectToService();
        }

        public async Task Disconnect()
        {
            if (_messageBroker != null)
            {
                try
                {
                    await Task.WhenAny(_messageBroker.DisconnectAsync(), Task.Delay(5000));
                }
                catch { }
            }
        }

        public void SessionConnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals(_settings.ConnectionName) && !_isMPDisplayConnected)
                {
                    IsMPDisplayConnected = true;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin") && !_isMediaPortalConnected)
                {
                    IsMediaPortalConnected = true;
                }

                if (connection.ConnectionName.Equals("TVServerPlugin") && !_isTVServerConnected)
                {
                    IsTVServerConnected = true;
                }
            }
        }

        public void SessionDisconnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals(_settings.ConnectionName) && _isMPDisplayConnected)
                {
                    IsMPDisplayConnected = false;
                    IsMediaPortalConnected = false;
                    IsTVServerConnected = false;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin") && _isMediaPortalConnected)
                {
                    IsMediaPortalConnected = false;
                }

                if (connection.ConnectionName.Equals("TVServerPlugin") && _isTVServerConnected)
                {
                    IsTVServerConnected = false;
                }
            }
        }

        public async Task SendMediaPortalMessage(APIMediaPortalMessage message)
        {
            if (_messageBroker != null)
            {
                await _messageBroker.SendMediaPortalMessageAsync(message);
            }
        }


        #region Receive

        public async void ReceiveAPIPropertyMessage(APIPropertyMessage message)
        {
            await GUIDataRepository.PropertyManager.AddProperty(message);
        }

        public async void ReceiveAPIListMessage(APIListMessage message)
        {
            await GUIDataRepository.ListManager.AddListData(message);
        }

        public async void ReceiveAPIInfoMessage(APIInfoMessage message)
        {
            await GUIDataRepository.InfoManager.AddInfo(message);
        }

        public async void ReceiveAPIDataMessage(APIDataMessage message)
        {
           // await Repository<APIDataMessage>.Instance.Add(message);
        }

        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
        {
            //  Repository<APIMediaPortalMessage>.Instance.Add(message);
        }

        public void ReceiveTVServerMessage(APITVServerMessage message)
        {
            // Repository<APITVServerMessage>.Instance.Add(message);
        }

        #endregion


    }

}
