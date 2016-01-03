using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Common.Helpers;
using Common.Log;
using GUIFramework;
using GUISkinFramework.Editors;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using SkinEditor.Themes;
using SkinEditor.Views;

namespace SkinEditor.Helpers
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ConnectionHelper : UserControl, IMessageCallback 
    {

        private MessageClient _messageBroker;
        private DateTime _lastKeepAlive = DateTime.MinValue;
        private APIConnection _connection;
        private EndpointAddress _serverEndpoint;
        private NetTcpBinding _serverBinding;
        private DispatcherTimer _secondTimer;

        private bool _isConnected;
        private bool _isMediaPortalConnected;

        private ObservableCollection<SkinPropertyItem> _propertyData = new ObservableCollection<SkinPropertyItem>();
        private int _windowId;
        private int _dialogId;
        private int _focusedControlId;
        private List<string> _propertyTagCache;

        private InfoEditorViewSettings _settings;

        private readonly Log _log = LoggingManager.GetLog(typeof(ConnectionHelper));

        public ConnectionHelper()
        {
            Baseclass = null;
        }

        public EditorViewModel Baseclass { set; get; }

        public InfoEditorViewSettings Settings
        {
            set { _settings = value; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; NotifyPropertyChangedAll("IsConnected"); }
        }

        public bool IsMediaPortalConnected
        {
            get { return _isMediaPortalConnected; }
            set { _isMediaPortalConnected = value; NotifyPropertyChangedAll("IsMediaPortalConnected"); }
        }

        public ObservableCollection<SkinPropertyItem> PropertyData
        {
            get { return _propertyData; }
            set { _propertyData = value; }
        }

        public ObservableCollection<SkinPropertyItem> ListItemData { get; set; } = new ObservableCollection<SkinPropertyItem>();

        public int WindowId
        {
            get { return _windowId; }
            set { _windowId = value; NotifyPropertyChangedAll("WindowId"); }
        }

        public int DialogId
        {
            get { return _dialogId; }
            set { _dialogId = value; NotifyPropertyChangedAll("DialogId"); }
        }

        public int FocusedControlId
        {
            get { return _focusedControlId; }
            set { _focusedControlId = value; NotifyPropertyChangedAll("FocusedControlId"); }
        }

        public List<string> PropertyTagCache
        {
            get
            {
                if (_propertyTagCache != null) return _propertyTagCache;

                if (Baseclass != null)
                {
                    _propertyTagCache = Baseclass.SkinInfo.Properties.SelectMany(x => x.MediaPortalTags).Select(m => m.Tag).ToList();
                }
                return _propertyTagCache;
            }
            set { _propertyTagCache = value; }
        }
    
         // call notifyer of all registered baseclasses uning this instance
        private void NotifyPropertyChangedAll(string property)
        {
            Baseclass?.NotifyPropertyChanged(property);
        }

        public async Task InitializeServerConnection()
        {

            _serverEndpoint = new EndpointAddress($"net.tcp://{_settings.IpAddress}:{_settings.Port}/MPDisplayService");
            _log.Message(LogLevel.Info, "[Initialize] - Initializing server connection. Connection: {0}", _serverEndpoint);
            _serverBinding = ConnectHelper.GetServerBinding();

            var site = new InstanceContext(this);
            if (_messageBroker != null)
            {
                _messageBroker.InnerChannel.Faulted -= Channel_Faulted;
                _messageBroker = null;
            }
            _messageBroker = new MessageClient(site, _serverBinding, _serverEndpoint);
            _messageBroker.InnerChannel.Faulted += Channel_Faulted;

           _connection = new APIConnection(ConnectionType.SkinEditor);

            await ConnectToService();
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            _log.Message(LogLevel.Error, "[Faulted] - Server connection has faulted");
            Disconnect();
        }

        /// <summary>
        /// Connects to service.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectToService()
        {
            if (_messageBroker != null)
            {
                try
                {

                    _log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
                    var result = await _messageBroker.ConnectAsync(_connection);
                    if (result != null && result.Any())
                    {
                        _log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                        IsConnected = true;
                        IsMediaPortalConnected = result.Any(x => x.ConnectionType.Equals(ConnectionType.MediaPortalPlugin));
                        _lastKeepAlive = DateTime.Now;
                    }
                }
                catch( Exception ex)
                {
                    _log.Message(LogLevel.Error, "[Connect] - Connection to server failed. Error: {0}", ex.Message);
                }
            }
        }


        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns></returns>
        public Task Disconnect()
        {
            IsConnected = false;
            if (_messageBroker == null) return Task.FromResult<object>(null);

            try
            {
                _log.Message(LogLevel.Info, "[Disconnect] - Disconnecting from server.");
                return Task.WhenAny(_messageBroker.DisconnectAsync(), Task.Delay(5000));
            }
            catch( Exception ex)
            {
                _log.Message(LogLevel.Error, "[Disconnect] - An exception occured when disconnecting. Exception: {0}", ex);
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Sessions the connected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionConnected(APIConnection connection)
        {
            if (connection == null) return;

            if (connection.ConnectionType.Equals(ConnectionType.SkinEditor))
            {
                IsConnected = true;
            }

            if (!connection.ConnectionType.Equals(ConnectionType.MediaPortalPlugin)) return;

            _log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin connected to network.");
            IsMediaPortalConnected = true;
        }

        /// <summary>
        /// Sessions the disconnected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionDisconnected(APIConnection connection)
        {
            if (connection == null) return;

            if (connection.ConnectionType.Equals(ConnectionType.SkinEditor))
            {
                Disconnect();
            }

            if (!connection.ConnectionType.Equals(ConnectionType.MediaPortalPlugin)) return;

            _log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin disconnected from network.");
            IsMediaPortalConnected = false;
        }
        /// <summary>
        /// Starts the second timer.
        /// </summary>
        public void StartSecondTimer()
        {
            if (_secondTimer != null) return;

            _secondTimer = new DispatcherTimer(DispatcherPriority.Background) {Interval = TimeSpan.FromSeconds(1)};
            _secondTimer.Tick += SecondTimer_Tick;
            _secondTimer.Start();
        }

        /// <summary>
        /// Stops the second timer.
        /// </summary>
        public void StopSecondTimer()
        {
            if (_secondTimer == null) return;

            _secondTimer.Tick -= SecondTimer_Tick;
            _secondTimer.Stop();
            _secondTimer = null;
        }

        /// <summary>
        /// Handles the Tick event of the SecondTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void SecondTimer_Tick(object sender, EventArgs e)
        {
            await SendKeepAlive();
        }

        /// <summary>
        /// Sends the keep alive.
        /// </summary>
        /// <returns></returns>
        public async Task SendKeepAlive()
        {
            if (IsConnected)
            {
                if (DateTime.Now > _lastKeepAlive.AddSeconds(45))
                {
                    await Disconnect();
                }

                if (DateTime.Now > _lastKeepAlive.AddSeconds(30))
                {
                    _log.Message(LogLevel.Verbose, "[KeepAlive] - Sending KeepAlive message.");
                    await SendKeepAliveMessage();
                }
            }
        }

        /// <summary>
        /// Sends the keep alive message.
        /// </summary>
        /// <returns></returns>
        public Task SendKeepAliveMessage()
        {
            try
            {
                if (_messageBroker != null)
                {
                    return _messageBroker.SendDataMessageAsync(new APIDataMessage { DataType = APIDataMessageType.KeepAlive });
                }
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendKeepAliveMessage] - An exception occured sending keep alive message. Exception: {0}", ex);
            }
            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// Receives the API data message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ReceiveAPIDataMessage(APIDataMessage message)
        {
            Dispatcher.Invoke(() =>
            {
                if (message != null && message.DataType == APIDataMessageType.KeepAlive)
                {
                    _lastKeepAlive = DateTime.Now;
                    return;
                }

                if (message == null || message.DataType != APIDataMessageType.SkinEditorInfo) return;

                var skineditorData = message.SkinEditorData;
                if (skineditorData.DataType == APISkinEditorDataType.Property)
                {
                    var property = skineditorData.PropertyData;
                    if (property != null && property.Length == 2)
                    {

                        var existing = _propertyData.FirstOrDefault(p => p.Tag == property[0]);
                        if (existing != null)
                        {
                            existing.Value = property[1];
                            return;
                        }
                        PropertyData.Add(new SkinPropertyItem { Tag = property[0], Value = property[1], IsDefined = PropertyTagCache.Contains(property[0]) });
                        _log.Message(LogLevel.Verbose, "[SkinEditor Message Received] - Property Tag <{0}> Value <{1}>.", property[0], property[1]);

                    }
                }

                if (skineditorData.DataType == APISkinEditorDataType.ListItem)
                {
                    ListItemData.Clear();
                    foreach (var item in skineditorData.ListItemData.Where(x => x != null && x.Length == 2).ToArray())
                    {
                        ListItemData.Add(new SkinPropertyItem { Tag = item[0], Value = item[1] });
                        _log.Message(LogLevel.Verbose, "[SkinEditor Message Received] - List Item Tag <{0}> Value <{1}>.", item[0], item[1]);
                    }
                }

                if (skineditorData.DataType == APISkinEditorDataType.WindowId)
                {
                    WindowId = skineditorData.IntValue;
                    _log.Message(LogLevel.Verbose, "[SkinEditor Message Received] - Window ID <{0}>.", skineditorData.IntValue);
                }

                if (skineditorData.DataType == APISkinEditorDataType.DialogId)
                {
                    DialogId = skineditorData.IntValue;
                    _log.Message(LogLevel.Verbose, "[SkinEditor Message Received] - Dialog ID <{0}>.", skineditorData.IntValue);
                }

                if (skineditorData.DataType != APISkinEditorDataType.FocusedControlId) return;

                FocusedControlId = skineditorData.IntValue;
                _log.Message(LogLevel.Verbose, "[SkinEditor Message Received] - Focussed Control ID <{0}>.", skineditorData.IntValue);
            });
        }


        public void OpenPropertyEditor(SkinPropertyItem item)
        {
            if (item == null || Baseclass == null) return;

            var propEditor = new PropertyEditor(Baseclass.SkinInfo);

            if (item.IsDefined)
            {
                var selection = Baseclass.SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == item.Tag && x.MediaPortalTags.Select(m => m.Tag).Contains(item.Tag))
                                ?? Baseclass.SkinInfo.Properties.FirstOrDefault(x => x.MediaPortalTags.Select(m => m.Tag).Contains(item.Tag));
                propEditor.SelectedProperty = selection;
            }
            else
            {
                var newProp = new XmlProperty { SkinTag = item.Tag, MediaPortalTags = new ObservableCollection<XmlMediaPortalTag> { new XmlMediaPortalTag { Tag = item.Tag } } };
                Baseclass.SkinInfo.Properties.Add(newProp);
                propEditor.SelectedProperty = newProp;
            }
            new EditorDialog(propEditor, false).ShowDialog();

            foreach (var property in PropertyData.Where(property => PropertyTagCache != null))
            {
                property.IsDefined = PropertyTagCache.Contains(property.Tag);
            }
        }

        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message) { }
        public void ReceiveAPIPropertyMessage(APIPropertyMessage message) { }
        public void ReceiveAPIListMessage(APIListMessage message) { }
        public void ReceiveAPIInfoMessage(APIInfoMessage message) { }
    }
}
