using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUIFramework.GUI;
using GUIFramework.GUI.Controls;
using GUIFramework.GUI.Windows;
using GUIFramework.Managers;
using GUISkinFramework;
using GUISkinFramework.Common;
using GUISkinFramework.Windows;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;
using System.Windows.Threading;
using GUISkinFramework.Editor.PropertyEditors;

namespace GUIFramework
{
    /// <summary>
    /// Interaction logic for GUISurface.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class GUISurface : UserControl, IMessageCallback, INotifyPropertyChanged
    {
        private Log Log = LoggingManager.GetLog(typeof(GUISurface));
        private XmlSkinInfo _currentSkin;
        private GUISettings _settings;
        private ObservableCollection<IControlHost> _surfaceElements = new ObservableCollection<IControlHost>();
        private GUIWindow _currentWindow;
        private GUIMPDialog _currentMediaPortalDialog;
        private List<GUIMPDDialog> _currentMPDisplayDialogs = new List<GUIMPDDialog>();
        private bool _currentWindowIsLocked;
        private Queue<int> _previousMPDisplayWindows = new Queue<int>();
        private DispatcherTimer _secondTimer;

        public GUISurface()
        {
            InitializeComponent();
            StartSecondTimer();
        }

    

        public GUISettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }
        
        public XmlSkinInfo CurrentSkin
        {
            get { return _currentSkin; }
            set { _currentSkin = value; }
        }

        public ObservableCollection<IControlHost> SurfaceElements
        {
            get { return _surfaceElements; }
            set { _surfaceElements = value; }
        }

        public IEnumerable<GUIMPDWindow> MPDisplayWindows
        {
            get { return _surfaceElements.OfType<GUIMPDWindow>(); }
        }

        public IEnumerable<GUIMPWindow> MediaPortalWindows
        {
            get { return _surfaceElements.OfType<GUIMPWindow>(); }
        }

        public IEnumerable<GUIPlayerWindow> PlayerWindows
        {
            get { return _surfaceElements.OfType<GUIPlayerWindow>(); }
        }

        public IEnumerable<GUIMPDDialog> MPDisplayDialogs
        {
            get { return _surfaceElements.OfType<GUIMPDDialog>(); }
        }

        public IEnumerable<GUIMPDialog> MediaPortalDialogs
        {
            get { return _surfaceElements.OfType<GUIMPDialog>(); }
        }


        #region Load Skin

        public async Task LoadSkin(GUISettings settings)
        {
            if (settings != null)
            {
                IsSplashScreenVisible = true;
                XmlSkinInfo skinInfo = XmlManager.Deserialize<XmlSkinInfo>(settings.SkinInfoXml);
                if (skinInfo != null)
                {
                    Settings = settings;
                    skinInfo.SkinFolderPath = System.IO.Path.GetDirectoryName(settings.SkinInfoXml);
                    await LoadSkin(skinInfo);
                }
            }
        }

        public async Task LoadSkin(XmlSkinInfo skinInfo, GUISettings settings)
        {
            if (settings != null && skinInfo != null)
            {
                Settings = settings;
                await LoadSkin(skinInfo);
            }
        }

        public async Task LoadSkin(XmlSkinInfo skinInfo)
        {
            if (skinInfo != null)
            {
               
                CurrentSkin = skinInfo;
                Width = CurrentSkin.SkinWidth;
                Height = CurrentSkin.SkinHeight;
                ShowSplashScreen();
                SurfaceElements.Clear();

                await SetSplashScreenText("Loading Skin Files...");
                await Dispatcher.InvokeAsync(() =>
                {
                    CurrentSkin.LoadXmlSkin();
                    GUIImageManager.Init(CurrentSkin);
                    InfoRepository.Instance.Initialize(Settings, CurrentSkin);
                    PropertyRepository.Instance.Initialize(Settings, CurrentSkin);
                    ListRepository.Instance.Initialize(Settings, CurrentSkin);
                    GenericDataRepository.Instance.Initialize(Settings, CurrentSkin);
                });


                await SetSplashScreenText("Loading Windows...");
                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var window in CurrentSkin.Windows)
                    {
                        SurfaceElements.Add(GUIElementFactory.CreateWindow(window));
                    }
                });


                await SetSplashScreenText("Loading Dialogs...");
                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var dialog in CurrentSkin.Dialogs)
                    {
                        SurfaceElements.Add(GUIElementFactory.CreateDialog(dialog));
                    }
                });



                await SetSplashScreenText("Connecting to service...");
                await InitializeServerConnection(Settings.ConnectionSettings);
              

                await SetSplashScreenText("Starting MPDisplay++...");
                await OpenMediaPortalWindow();
             
                if (_currentWindow != null)
                {
                    HideSplashScreen();
                    RegisterCallbacks();
                    return;
                }
                ShowSplashScreenError("No Default windows found!");
                return;
            }
            ShowSplashScreenError("Failed to load skin");
        }

     
      

        #endregion


        private async Task OpenMPDisplayWindow(int windowId, bool isPrevious)
        {
            if (_currentWindowIsLocked)
            {
                return;
            }

            var newWindow = MPDisplayWindows.GetOrDefault(windowId);
            if (newWindow != null && newWindow != _currentWindow)
            {
                foreach (var dialog in _currentMPDisplayDialogs.Where(d => d.CloseOnWindowChanged))
                {
                    await dialog.DialogClose();
                }

                if (_currentWindow != null)
                {
                    if (!isPrevious)
                    {
                        _previousMPDisplayWindows.Enqueue(_currentWindow.Id);
                    }
                }

                await ChangeWindow(newWindow);
            }

        }

        private async Task CloseMPDisplayWindow()
        {
            if (_currentWindow is GUIMPDWindow)
            {
                if (_currentWindowIsLocked)
                {
                    LockMPDisplayWindow();
                }
            
                if (GUIVisibilityManager.IsMediaPortalConnected())
                {
                    await OpenMediaPortalWindow();
                    return;
                }

                await OpenMPDisplayWindow(-1, false);
            }
        }

        private async Task OpenMPDisplayDialog(int dialogId)
        {
            var existing = MPDisplayDialogs.FirstOrDefault(w => w.Id == dialogId);
            if (_currentMPDisplayDialogs.Contains(existing))
            {
                if (existing != null)
                {
                    await existing.DialogClose();
                    _currentMPDisplayDialogs.Remove(existing);
                    return;
                }
            }

            var newDialog = MPDisplayDialogs.FirstOrDefault(w => w.Id == dialogId);
            if (newDialog != null)
            {
                _currentMPDisplayDialogs.Add(newDialog);
                await newDialog.DialogOpen();
            }
        }

        private async Task OpenPreviousMPDisplayWindow()
        {
            if (_currentWindowIsLocked)
            {
                return;
            }

            if (_previousMPDisplayWindows.Any())
            {
                await OpenMPDisplayWindow(_previousMPDisplayWindows.Dequeue(), true);
            }
        }


        private void LockMPDisplayWindow()
        {
            if (_currentWindow is GUIMPDWindow)
            {
                if (_currentWindowIsLocked)
                {
                    _currentWindowIsLocked = false;
                    return;
                }
                _currentWindowIsLocked = true;
            }
        }



        private async Task OpenMediaPortalWindow()
        {
            if (_currentWindowIsLocked)
            {
                return;
            }

            if (InfoRepository.Instance.IsMediaPortalConnected)
            {
                if (InfoRepository.Instance.PlaybackType != APIPlaybackType.None)
                {
                    var playerWindow = PlayerWindows.GetOrDefault();
                    if (playerWindow != null)
                    {
                        if (InfoRepository.Instance.PlaybackType.IsMusic() && _isUserInteracting)
                        {
                            return;
                        }
                        await ChangeWindow(playerWindow);
                        return;
                    }
                }

                var newWindow = MediaPortalWindows.GetOrDefault();
                if (newWindow != null)
                {
                    await ChangeWindow(newWindow);
                }
            }
            else
            {
                await OpenMPDisplayWindow(-1, false);
            }
        }

        private async Task OpenMediaPortalDialog(int dialogId)
        {
            var newDialog = MediaPortalDialogs.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible());
            if (newDialog != null && newDialog != _currentMediaPortalDialog)
            {
                if (_currentMediaPortalDialog != null)
                {
                    await _currentMediaPortalDialog.DialogClose();
                }

                _currentMediaPortalDialog = newDialog;
                await _currentMediaPortalDialog.DialogOpen();
            }
        }



        private async Task ChangeWindow(GUIWindow newWindow)
        {
            if (newWindow != _currentWindow)
            {
                if (_currentWindow != null)
                {
                    await _currentWindow.WindowClose();
                }
                await RegisterWindowData(newWindow);
                _currentWindow = newWindow;
                await _currentWindow.WindowOpen();
            }
        }

        private bool _isUserInteracting = false;
        private DateTime _lastUserInteraction = DateTime.MinValue;


        private async Task CheckUserInteraction()
        {
            if (_isUserInteracting)
            {
                if (_lastUserInteraction != DateTime.MaxValue && DateTime.Now > _lastUserInteraction.AddSeconds(10))
                {
                    _isUserInteracting = false;
                    await OpenMediaPortalWindow();
                }
            }
        }

        private void SetUserInteraction(bool isInteracting)
        {
            _isUserInteracting = isInteracting;
            _lastUserInteraction = _isUserInteracting
                ? DateTime.Now
                : DateTime.MaxValue;
        }
    

     

     




        private void RegisterCallbacks()
        {
            GUIActionManager.RegisterAction(XmlActionType.CloseWindow, async action => await CloseMPDisplayWindow());
            GUIActionManager.RegisterAction(XmlActionType.OpenWindow, async action => await OpenMPDisplayWindow(action.GetParam1As<int>(-1), false));
            GUIActionManager.RegisterAction(XmlActionType.OpenDialog, async action => await OpenMPDisplayDialog(action.GetParam1As<int>(-1)));
            GUIActionManager.RegisterAction(XmlActionType.PreviousWindow, async action => await OpenPreviousMPDisplayWindow());
            GUIActionManager.RegisterAction(XmlActionType.Connect, async action => await Reconnect());
            GUIActionManager.RegisterAction(XmlActionType.LockWindow, action => LockMPDisplayWindow());
            GUIActionManager.RegisterAction(XmlActionType.MediaPortalWindow, async action => await SendMediaPortalAction(action));
            GUIActionManager.RegisterAction(XmlActionType.MediaPortalAction, async action => await SendMediaPortalAction(action));
            GUIActionManager.RegisterAction(XmlActionType.SwitchTheme, action => CurrentSkin.CycleTheme());
            GUIActionManager.RegisterAction(XmlActionType.Exit, action => CloseDown(true));
            GUIActionManager.RegisterAction(XmlActionType.NowPlaying, action => SetUserInteraction(false));

            InfoRepository.RegisterMessage<int>(InfoMessageType.WindowId, async windowId => await OpenMediaPortalWindow());
            InfoRepository.RegisterMessage<int>(InfoMessageType.PlaybackState, async windowId => await OpenMediaPortalWindow());
            InfoRepository.RegisterMessage<int>(InfoMessageType.PlaybackType, async windowId => await OpenMediaPortalWindow());
            InfoRepository.RegisterMessage<int>(InfoMessageType.PlayerType, async windowId => await OpenMediaPortalWindow());

            ListRepository.RegisterMessage<APIListAction>(ListServiceMessage.SendItem, async item => await SendListAction(item));

            GenericDataRepository.RegisterMessage(GenericDataMessageType.ResetIteraction, () => SetUserInteraction(true));
          
        }

 


        public Task RegisterWindowData(GUIWindow window)
        {
            return SendMediaPortalMessage(new APIMediaPortalMessage
            {
                MessageType = APIMediaPortalMessageType.WindowInfoMessage,
                WindowMessage = new APIWindowInfoMessage
                {
                    EQData = GenericDataRepository.GetEQDataLength(window),
                    Lists = ListRepository.GetRegisteredLists(window),
                    Properties = PropertyRepository.GetRegisteredProperties(window)
                }
            });
        }

        public Task RegisterDialogData(GUIDialog dialog)
        {
            return SendMediaPortalMessage(new APIMediaPortalMessage
            {
                MessageType = APIMediaPortalMessageType.DialogInfoMessage,
                WindowMessage = new APIWindowInfoMessage
                {
                    EQData = GenericDataRepository.GetEQDataLength(dialog),
                    Lists = ListRepository.GetRegisteredLists(dialog),
                    Properties = PropertyRepository.GetRegisteredProperties(dialog)
                }
            });
        }

        public Task SendListAction(APIListAction action)
        {
            return SendMediaPortalMessage(new APIMediaPortalMessage
            {
                MessageType = APIMediaPortalMessageType.ActionMessage,
                ActionMessage = new APIActionMessage
                {
                    ActionType = APIActionMessageType.ListAction,
                    ListAction = action
                }
            });
        }

        private DateTime _lastKeepAlive = DateTime.MaxValue;

        public async Task SendKeepAlive()
        {
            if (_lastKeepAlive != DateTime.MaxValue && DateTime.Now > _lastKeepAlive.AddSeconds(30))
            {
                _lastKeepAlive = DateTime.Now;
                if (!InfoRepository.Instance.IsMPDisplayConnected)
                {
                    await Reconnect();
                }
                Log.Message(LogLevel.Info, "[KeepAlive] - Sending connection KeepAlive message.");
                await SendMediaPortalMessage(new APIMediaPortalMessage { MessageType = APIMediaPortalMessageType.KeepAlive });
            }
        }
  
       

        public void ChangeSkinStyle()
        {
            if (CurrentSkin != null)
            {
                CurrentSkin.CycleTheme();
            }
        }


        public void CloseDown(bool exit)
        {
            if (exit)
            {
                Log.Message(LogLevel.Info, "[CloseDown] - Exit requested from GUIAction");
                Application.Current.MainWindow.Close();
                return;
            }

          
            StopSecondTimer();
            _isDisconnecting = true;
            Disconnect();
            ClearRepositories();
            SurfaceElements.Clear();
        }

        private void ClearRepositories()
        {
            Log.Message(LogLevel.Info, "[CloseDown] - Clearing repositories..");
            InfoRepository.Instance.ClearRepository();
            ListRepository.Instance.ClearRepository();
            PropertyRepository.Instance.ClearRepository();
            GenericDataRepository.Instance.ClearRepository();
            Log.Message(LogLevel.Info, "[CloseDown] - Repositories cleared");
        }

        private void StartSecondTimer()
        {
            if (_secondTimer == null)
            {
                _secondTimer = new DispatcherTimer(DispatcherPriority.Background);
                _secondTimer.Interval = TimeSpan.FromSeconds(1);
                _secondTimer.Tick += SecondTimer_Tick;
                _secondTimer.Start();
            }
        }

        private void StopSecondTimer()
        {
            if (_secondTimer != null)
            {
                _secondTimer.Tick -= SecondTimer_Tick;
                _secondTimer.Stop();
                _secondTimer = null;
            }
        }

        private async void SecondTimer_Tick(object sender, EventArgs e)
        {
            await CheckUserInteraction();
            await SendKeepAlive();
        }


        #region Connection

        private APIConnection _connection;
        private EndpointAddress serverEndpoint;
        private NetTcpBinding serverBinding;
        private bool _isDisconnecting = false;
        private static MessageClient _messageBroker;
    

        public async Task InitializeServerConnection(ConnectionSettings settings)
        {
           // _settings = settings;
            serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port));
            Log.Message(LogLevel.Info, "[Initialize] - Initializing server connection. Connection: {0}", serverEndpoint);
            serverBinding = new NetTcpBinding();

            // Security (lol)
            serverBinding.Security.Mode = SecurityMode.None;
            serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

            // Connection
            serverBinding.Name = "NetTcpBinding_IMessage";
            serverBinding.CloseTimeout = new TimeSpan(0, 0, 5);
            serverBinding.OpenTimeout = new TimeSpan(0, 0, 5);
            serverBinding.ReceiveTimeout = new TimeSpan(0, 0, 30);
            serverBinding.SendTimeout = new TimeSpan(0, 0, 30);
            serverBinding.TransferMode = TransferMode.Buffered;
            serverBinding.ListenBacklog = 10;
            serverBinding.MaxConnections = 10;
            serverBinding.MaxReceivedMessageSize = int.MaxValue;
            serverBinding.MaxBufferSize = int.MaxValue;
            serverBinding.MaxBufferPoolSize = int.MaxValue;

            // Message
            serverBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            serverBinding.ReaderQuotas.MaxDepth = 32;
            serverBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            serverBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            serverBinding.ReliableSession.Enabled = true;
            serverBinding.ReliableSession.InactivityTimeout = new TimeSpan( 0, 3, 0);

            InstanceContext site = new InstanceContext(this);
            if (_messageBroker != null)
            {
                _messageBroker.InnerChannel.Faulted -= Channel_Faulted;
                _messageBroker = null;
            }
            _messageBroker = new MessageClient(site, serverBinding, serverEndpoint);
            _messageBroker.InnerChannel.Faulted += new EventHandler(Channel_Faulted);

            _connection = new APIConnection(settings.ConnectionName);
            await ConnectToService();
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            Log.Message(LogLevel.Error, "[Faulted] - Server connection has faulted");
            InfoRepository.Instance.IsMPDisplayConnected = false;
            InfoRepository.Instance.IsMediaPortalConnected = false;
            InfoRepository.Instance.IsTVServerConnected = false;
        }

        public async Task ConnectToService()
        {
            if (_messageBroker != null)
            {
                try
                {
                    _lastKeepAlive = DateTime.Now;
                    Log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
                    InfoRepository.Instance.IsMPDisplayConnected = false;
                    InfoRepository.Instance.IsMediaPortalConnected = false;
                    InfoRepository.Instance.IsTVServerConnected = false;
                    var result = await _messageBroker.ConnectAsync(_connection);
                    if (result != null && result.Any())
                    {
                        Log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                        InfoRepository.Instance.IsMPDisplayConnected = result.Any(x => x.ConnectionName.Equals(_settings.ConnectionSettings.ConnectionName));
                        InfoRepository.Instance.IsMediaPortalConnected = result.Any(x => x.ConnectionName.Equals("MediaPortalPlugin"));
                        InfoRepository.Instance.IsTVServerConnected = result.Any(x => x.ConnectionName.Equals("TVServerPlugin"));
                       
                    }
                }
                catch (Exception ex)
                {
                    Log.Message(LogLevel.Error, "[Connect] - Connection to server failed. Error: {0}", ex.Message);
                }
            }
        }

      

        public async Task Reconnect()
        {
            if (!_isDisconnecting)
            {
                Log.Message(LogLevel.Info, "[Reconnect] - Reconnecting to server.");
                await Disconnect();
                await InitializeServerConnection(_settings.ConnectionSettings);
            }
        }

        public Task Disconnect()
        {
            if (_messageBroker != null)
            {
                try
                {
                    Log.Message(LogLevel.Info, "[Disconnect] - Disconnecting from server.");
                    return Task.WhenAny(_messageBroker.DisconnectAsync(), Task.Delay(5000));
                }
                catch { }
            }
            return Task.FromResult<object>(null);
        }

        public void SessionConnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals(_settings.ConnectionSettings.ConnectionName))
                {
                    InfoRepository.Instance.IsMPDisplayConnected = true;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin connected to network.");
                    InfoRepository.Instance.IsMediaPortalConnected = true;
                    Dispatcher.InvokeAsync<Task>(OpenMediaPortalWindow);
                }

                if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - TVServerPlugin connected to network.");
                    InfoRepository.Instance.IsTVServerConnected = true;
                }
            }
        }

        public void SessionDisconnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals(_settings.ConnectionSettings.ConnectionName))
                {
                    InfoRepository.Instance.IsMPDisplayConnected = false;
                    InfoRepository.Instance.IsMediaPortalConnected = false;
                    InfoRepository.Instance.IsTVServerConnected = false;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin disconnected from network.");
                    ClearRepositories();
                    Dispatcher.InvokeAsync<Task>(() => OpenMPDisplayWindow(-1, false));
                }

                if (connection.ConnectionName.Equals("TVServerPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - TVServerPlugin disconnected from network.");
                    InfoRepository.Instance.IsTVServerConnected = false;
                }
            }
        }

        private Task SendMediaPortalAction(XmlAction action)
        {
            try
            {

                if (action.ActionType == XmlActionType.MediaPortalAction)
                {
                    MediaPortalActions mpAction = MediaPortalActions.ACTION_INVALID;
                    if (Enum.TryParse<MediaPortalActions>(action.GetParam1As<string>("ACTION_INVALID"), out mpAction))
                    {
                        return SendMediaPortalMessage(new APIMediaPortalMessage
                        {
                            MessageType = APIMediaPortalMessageType.ActionMessage,
                            ActionMessage = new APIActionMessage
                            {
                                ActionType = APIActionMessageType.MediaPortalAction,
                                MediaPortalAction = new APIMediaPortalAction
                                {
                                    ActionId = (int)mpAction
                                }
                            }
                        });
                    }


                }

                if (action.ActionType == XmlActionType.MediaPortalWindow)
                {
                    return SendMediaPortalMessage(new APIMediaPortalMessage
                    {
                        MessageType = APIMediaPortalMessageType.ActionMessage,
                        ActionMessage = new APIActionMessage
                        {
                            ActionType = APIActionMessageType.MediaPortalWindow,
                            MediaPortalAction = new APIMediaPortalAction
                            {
                                ActionId = action.GetParam1As<int>(-1)
                            }
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Exception("[SendMediaPortalAction] - ", ex);
            }
            return Task.FromResult<object>(null);
        }

        public Task SendMediaPortalMessage(APIMediaPortalMessage message)
        {
            try
            {
                if (_messageBroker != null &&  InfoRepository.Instance.IsMediaPortalConnected)
                {
                   return _messageBroker.SendMediaPortalMessageAsync(message);
                }
            }
            catch (Exception)
            {
                
            }
            return Task.FromResult<object>(null);
        }


        #region Receive

        public void ReceiveAPIPropertyMessage(APIPropertyMessage message)
        {
            PropertyRepository.Instance.AddProperty(message);
        }

        public void ReceiveAPIListMessage(APIListMessage message)
        {
            ListRepository.Instance.AddListData(message);
        }

        public void ReceiveAPIInfoMessage(APIInfoMessage message)
        {
           InfoRepository.Instance.AddInfo(message);
        }

        public void ReceiveAPIDataMessage(APIDataMessage message)
        {
            GenericDataRepository.Instance.AddDataMessage(message);
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

     

        #endregion

        #region SplashScreen

        private string _splashScreenImage;
        private string _splashScreenText;
        private bool _isSplashScreenVisible;
        private string _splashScreenSkinText;

        public string SplashScreenImage
        {
            get { return _splashScreenImage; }
            set { _splashScreenImage = value; NotifyPropertyChanged("SplashScreenImage"); }
        }

        public string SplashScreenText
        {
            get { return _splashScreenText; }
            set { _splashScreenText = value; NotifyPropertyChanged("SplashScreenText"); }
        }

        public string SplashScreenVersionText
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string SplashScreenSkinText
        {
            get { return _splashScreenSkinText; }
            set { _splashScreenSkinText = value; NotifyPropertyChanged("SplashScreenSkinText"); }
        }


        public bool IsSplashScreenVisible
        {
            get { return _isSplashScreenVisible; }
            set { _isSplashScreenVisible = value; NotifyPropertyChanged("IsSplashScreenVisible"); }
        }

        public async Task SetSplashScreenText(string text, params object[] args)
        {
            SplashScreenText = args != null && args.Length > 0 ? string.Format(text, args) : text;
            await Task.Delay(1);
        }

        private void ShowSplashScreen()
        {
            IsSplashScreenVisible = true;
            SplashScreenSkinText = string.Format("{0}, Author: {1}", CurrentSkin.SkinName, CurrentSkin.Author);
            SplashScreenImage = CurrentSkin.SplashScreenImage;
        }

        private void HideSplashScreen()
        {
            SplashScreenImage = null;
            IsSplashScreenVisible = false;
        }

        private async void ShowSplashScreenError(string error)
        {
            SplashScreenImage = CurrentSkin.ErrorScreenImage;
            await SetSplashScreenText(error);
        }



        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            SetUserInteraction(true);
            base.OnMouseDown(e);
        }
    }
}
