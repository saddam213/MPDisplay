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


        public GUISurface()
        {
            InitializeComponent();
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
                InitializeServerConnection(Settings.ConnectionSettings);
                await ConnectToService();

                await SetSplashScreenText("Starting MPDisplay++...");
                await UpdateMediaPortalWindow();
             
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

        private bool _userIsActive = false;

        private async Task OpenMPDisplayWindow(int windowId, bool isPrevious)
        {
            if (!_currentWindowIsLocked)
            {
                var newWindow = MPDisplayWindows.FirstOrDefault(w => w.Id == windowId)
                    ?? MPDisplayWindows.DefaultWindow();
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

        private async Task OpenPreviousMPDisplayWindow()
        {
            if (!_currentWindowIsLocked)
            {
                if (_previousMPDisplayWindows.Any())
                {
                    await OpenMPDisplayWindow(_previousMPDisplayWindows.Dequeue(), true);
                }
            }
        }


        private async Task CloseMPDisplayWindow()
        {
            if (!_currentWindowIsLocked)
            {
                if (_currentWindow is GUIMPDWindow)
                {
                    _userIsActive = false;
                    await _currentWindow.WindowClose();

                    if (GUIVisibilityManager.IsMediaPortalConnected())
                    {
                        await UpdateMediaPortalWindow();
                        return;
                    }

                    var defaultWindow = MPDisplayWindows.FirstOrDefault(w => w.IsDefault)
                        ?? MPDisplayWindows.FirstOrDefault();
                    await OpenMPDisplayWindow(defaultWindow.Id, false);
                }
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







        private async Task UpdateMediaPortalWindow()
        {
            if (!_currentWindowIsLocked && !_userIsActive)
            {
                if (InfoRepository.Instance.IsMediaPortalConnected)
                {
                    if (InfoRepository.Instance.PlaybackType != APIPlaybackType.None)
                    {
                        var playerWindow = PlayerWindows.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible());
                        if (playerWindow != null)
                        {
                            await ChangeWindow(playerWindow);
                            return;
                        }
                    }

                    var newWindow = MediaPortalWindows.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible())
                                   ?? MediaPortalWindows.DefaultWindow();
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
        }

        private async Task OpenMediaPortalDialog(int dialogId)
        {
            //var newDialog = MediaPortalDialogs.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible());
            //if (newDialog != null && newDialog != _currentMediaPortalDialog)
            //{
            //    if (_currentMediaPortalDialog != null)
            //    {
            //        await _currentMediaPortalDialog.DialogClose();
            //    }

            //    _currentMediaPortalDialog = newDialog;
            //    await _currentMediaPortalDialog.DialogOpen();
            //}
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






        private void ResetUserTimer()
        {

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

            InfoRepository.RegisterMessage<int>(InfoMessageType.WindowId, async windowId => await UpdateMediaPortalWindow());
            InfoRepository.RegisterMessage<int>(InfoMessageType.PlaybackState, async windowId => await UpdateMediaPortalWindow());
            InfoRepository.RegisterMessage<int>(InfoMessageType.PlaybackType, async windowId => await UpdateMediaPortalWindow());
            InfoRepository.RegisterMessage<int>(InfoMessageType.PlayerType, async windowId => await UpdateMediaPortalWindow());
        }

        public int MyProperty { get; set; }
   
      


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

        public void ChangeSkinStyle()
        {
            if (CurrentSkin != null)
            {
                CurrentSkin.CycleTheme();
            }
        }




        #region Connection

        public static MessageClient MessageBroker
        {
            get { return _messageBroker; }
        }

        private APIConnection _connection;
        private EndpointAddress serverEndpoint;
        private NetTcpBinding serverBinding;
        //private ConnectionSettings _settings;
        private static MessageClient _messageBroker;
    

        public void InitializeServerConnection(ConnectionSettings settings)
        {
           // _settings = settings;
            serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port));
            serverBinding = new NetTcpBinding();

            // Security (lol)
            serverBinding.Security.Mode = SecurityMode.None;
            serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

            // Connection
            serverBinding.Name = "NetTcpBinding_IMessage";
            serverBinding.CloseTimeout = new TimeSpan(0, 0, 30);
            serverBinding.OpenTimeout = new TimeSpan(0, 0, 30);
            serverBinding.ReceiveTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)
            serverBinding.SendTimeout = new TimeSpan(0, 0, 10);
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
            InfoRepository.Instance.IsMPDisplayConnected = false;
            InfoRepository.Instance.IsMediaPortalConnected = false;
            InfoRepository.Instance.IsTVServerConnected = false;
        }

        public async Task ConnectToService()
        {
            if (_messageBroker != null)
            {
                await Disconnect();
                try
                {
                    var task = _messageBroker.ConnectAsync(_connection);
                    if (await Task.WhenAny(task, Task.Delay(5000)) == task)
                    {
                        if (task != null && task.Result != null)
                        {
                            foreach (var connection in task.Result)
                            {
                                InfoRepository.Instance.IsMPDisplayConnected = task.Result.Any(x => x.ConnectionName.Equals(_settings.ConnectionSettings.ConnectionName));
                                InfoRepository.Instance.IsMediaPortalConnected = task.Result.Any(x => x.ConnectionName.Equals("MediaPortalPlugin"));
                                InfoRepository.Instance.IsTVServerConnected = task.Result.Any(x => x.ConnectionName.Equals("TVServerPlugin"));
                            }
                        }
                    }
                }
                catch (Exception)
                {
                   
                }
            }
        }

        public async Task Reconnect()
        {
            await Disconnect();
            InitializeServerConnection(_settings.ConnectionSettings);
            await ConnectToService();
        }

        public Task Disconnect()
        {
            if (_messageBroker != null)
            {
                try
                {
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
                if (connection.ConnectionName.Equals(_settings.ConnectionSettings.ConnectionName) && !InfoRepository.Instance.IsMPDisplayConnected)
                {
                    InfoRepository.Instance.IsMPDisplayConnected = true;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin") && !InfoRepository.Instance.IsMediaPortalConnected)
                {
                    InfoRepository.Instance.IsMediaPortalConnected = true;
                    Dispatcher.InvokeAsync<Task>(UpdateMediaPortalWindow);
                }

                if (connection.ConnectionName.Equals("TVServerPlugin") && !InfoRepository.Instance.IsTVServerConnected)
                {
                    InfoRepository.Instance.IsTVServerConnected = true;
                }
            }
        }

        public void SessionDisconnected(APIConnection connection)
        {
            if (connection != null)
            {
                if (connection.ConnectionName.Equals(_settings.ConnectionSettings.ConnectionName) && InfoRepository.Instance.IsMPDisplayConnected)
                {
                    InfoRepository.Instance.IsMPDisplayConnected = false;
                    InfoRepository.Instance.IsMediaPortalConnected = false;
                    InfoRepository.Instance.IsTVServerConnected = false;
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin") && InfoRepository.Instance.IsMediaPortalConnected)
                {
                    InfoRepository.Instance.IsMediaPortalConnected = false;
                    InfoRepository.Instance.ClearRepository();
                    Dispatcher.InvokeAsync<Task>(UpdateMediaPortalWindow);
                }

                if (connection.ConnectionName.Equals("TVServerPlugin") && InfoRepository.Instance.IsTVServerConnected)
                {
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
                    return SendMediaPortalMessage(new APIMediaPortalMessage
                    {
                        MessageType = APIMediaPortalMessageType.ActionMessage,
                        ActionMessage = new APIActionMessage
                        {
                            ActionType = APIActionMessageType.MediaPortalAction,
                            MediaPortalAction = new APIMediaPortalAction
                            {
                                ActionId = action.GetParam1As<int>(-1)
                            }
                        }
                    });
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
                if (_messageBroker != null)
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
    }
}
