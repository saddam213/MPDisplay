using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common;
using SkinEditor.Dialogs;
using MPDisplay.Common.Controls;
using Common.Helpers;
using MPDisplay.Common.ExtensionMethods;
using GUIFramework;
using MessageFramework.DataObjects;
using System.ServiceModel;
using GUISkinFramework.Editor.PropertyEditors.PropertyEditor;
using GUISkinFramework.PropertyEditors;
using GUISkinFramework.Property;
using SkinEditor.Views;


namespace SkinEditor.ConnectionHelpers
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ConnectionHelper : UserControl, IMessageCallback 
    {

        private MessageClient _messageBroker;
        private DateTime _lastKeepAlive = DateTime.MinValue;
        private APIConnection _connection;
        private EndpointAddress serverEndpoint;
        private NetTcpBinding serverBinding;
        private DispatcherTimer _secondTimer;

        private bool _isConnected;
        private bool _isMediaPortalConnected;

        private ObservableCollection<SkinPropertyItem> _propertyData = new ObservableCollection<SkinPropertyItem>();
        private ObservableCollection<SkinPropertyItem> _listItemData = new ObservableCollection<SkinPropertyItem>();
        private int _windowId;
        private int _dialogId;
        private int _focusedControlId;
        private List<string> _propertyTagCache;

        private InfoEditorViewSettings _settings;

        private EditorViewModel _baseclass = null;

        public EditorViewModel baseclass
        {
            set { _baseclass = value; }
            get { return _baseclass; }
        }

        public InfoEditorViewSettings settings
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

        public ObservableCollection<SkinPropertyItem> ListItemData
        {
            get { return _listItemData; }
            set { _listItemData = value; }
        }

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
                if (_propertyTagCache == null)
                {
                    if (_baseclass != null)
                    {
                        _propertyTagCache = _baseclass.SkinInfo.Properties.SelectMany(x => x.MediaPortalTags).Select(m => m.Tag).ToList();
                    }
                }
                return _propertyTagCache;
            }
            set { _propertyTagCache = value; }
        }
    
         // call notifyer of all registered baseclasses uning this instance
        private void NotifyPropertyChangedAll(string _property)
        {
                if ( _baseclass != null ) _baseclass.NotifyPropertyChanged(_property);
         }

        public async Task InitializeServerConnection()
        {

            serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", _settings.IpAddress, _settings.Port));
            // Log.Message(LogLevel.Info, "[Initialize] - Initializing server connection. Connection: {0}", serverEndpoint);
            serverBinding = ConnectHelper.getServerBinding();

            InstanceContext site = new InstanceContext(this);
            if (_messageBroker != null)
            {
                _messageBroker.InnerChannel.Faulted -= Channel_Faulted;
                _messageBroker = null;
            }
            _messageBroker = new MessageClient(site, serverBinding, serverEndpoint);
            _messageBroker.InnerChannel.Faulted += new EventHandler(Channel_Faulted);

            _connection = new APIConnection("SkinEditor");

            await ConnectToService();
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            // Log.Message(LogLevel.Error, "[Faulted] - Server connection has faulted");
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

                    // Log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
                    var result = await _messageBroker.ConnectAsync(_connection);
                    if (result != null && result.Any())
                    {
                        // Log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                        IsConnected = true;
                        IsMediaPortalConnected = result.Any(x => x.ConnectionName.Equals("MediaPortalPlugin"));
                        _lastKeepAlive = DateTime.Now;
                    }
                }
                catch
                {
                    // Log.Message(LogLevel.Error, "[Connect] - Connection to server failed. Error: {0}", ex.Message);
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
            if (_messageBroker != null)
            {
                try
                {
                    //  Log.Message(LogLevel.Info, "[Disconnect] - Disconnecting from server.");
                    return Task.WhenAny(_messageBroker.DisconnectAsync(), Task.Delay(5000));
                }
                catch { }
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Sessions the connected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionConnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals("SkinEditor"))
                {
                    IsConnected = true;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    // Log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin connected to network.");
                    IsMediaPortalConnected = true;
                }
            }
        }

        /// <summary>
        /// Sessions the disconnected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void SessionDisconnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals("SkinEditor"))
                {
                    Disconnect();
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    // Log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin disconnected from network.");
                    IsMediaPortalConnected = false;
                }
            }
        }
        /// <summary>
        /// Starts the second timer.
        /// </summary>
        public void StartSecondTimer()
        {
            if (_secondTimer == null)
            {
                _secondTimer = new DispatcherTimer(DispatcherPriority.Background);
                _secondTimer.Interval = TimeSpan.FromSeconds(1);
                _secondTimer.Tick += SecondTimer_Tick;
                _secondTimer.Start();
            }
        }

        /// <summary>
        /// Stops the second timer.
        /// </summary>
        public void StopSecondTimer()
        {
            if (_secondTimer != null)
            {
                _secondTimer.Tick -= SecondTimer_Tick;
                _secondTimer.Stop();
                _secondTimer = null;
            }
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
                    //  Log.Message(LogLevel.Debug, "[KeepAlive] - Sending KeepAlive message.");
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
            catch (Exception)
            {

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

                if (message.DataType == APIDataMessageType.SkinEditorInfo)
                {
                    var skineditorData = message.SkinEditorData;
                    if (skineditorData.DataType == APISkinEditorDataType.Property)
                    {
                        var property = skineditorData.PropertyData;
                        if (property != null && property.Count() == 2)
                        {

                            var existing = _propertyData.FirstOrDefault(p => p.Tag == property[0]);
                            if (existing != null)
                            {
                                existing.Value = property[1];
                                return;
                            }
                            PropertyData.Add(new SkinPropertyItem { Tag = property[0], Value = property[1], IsDefined = PropertyTagCache.Contains(property[0]) });
                        }
                    }

                    if (skineditorData.DataType == APISkinEditorDataType.ListItem)
                    {
                        ListItemData.Clear();
                        foreach (var item in skineditorData.ListItemData.Where(x => x != null && x.Count() == 2).ToArray())
                        {
                            ListItemData.Add(new SkinPropertyItem { Tag = item[0], Value = item[1] });
                        }
                    }

                    if (skineditorData.DataType == APISkinEditorDataType.WindowId)
                    {
                        WindowId = skineditorData.IntValue;
                    }

                    if (skineditorData.DataType == APISkinEditorDataType.DialogId)
                    {
                        DialogId = skineditorData.IntValue;
                    }

                    if (skineditorData.DataType == APISkinEditorDataType.FocusedControlId)
                    {
                        FocusedControlId = skineditorData.IntValue;
                    }
                }
            });
        }


        public void OpenPropertyEditor(SkinPropertyItem item)
        {
            if (item != null && _baseclass != null)
            {
                var propEditor = new PropertyEditor(_baseclass.SkinInfo);

                if (item.IsDefined)
                {
                    var selection = _baseclass.SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == item.Tag && x.MediaPortalTags.Select(m => m.Tag).Contains(item.Tag))
                                 ?? _baseclass.SkinInfo.Properties.FirstOrDefault(x => x.MediaPortalTags.Select(m => m.Tag).Contains(item.Tag));
                    propEditor.SelectedProperty = selection;
                }
                else
                {
                    var newProp = new XmlProperty { SkinTag = item.Tag, MediaPortalTags = new ObservableCollection<XmlMediaPortalTag> { new XmlMediaPortalTag { Tag = item.Tag } } };
                    _baseclass.SkinInfo.Properties.Add(newProp);
                    propEditor.SelectedProperty = newProp;
                }
                new EditorDialog(propEditor, false).ShowDialog();
                PropertyTagCache = null;

                foreach (var property in PropertyData)
                {
                    property.IsDefined = PropertyTagCache.Contains(property.Tag);
                }
            }
        }

        public void ReceiveMediaPortalMessage(APIMediaPortalMessage message) { }
        public void ReceiveTVServerMessage(APITVServerMessage message) { }
        public void ReceiveAPIPropertyMessage(APIPropertyMessage message) { }
        public void ReceiveAPIListMessage(APIListMessage message) { }
        public void ReceiveAPIInfoMessage(APIInfoMessage message) { }
    }
}
