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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUIFramework.GUI;
using GUIFramework.GUI.Controls;
using GUIFramework.GUI.Windows;
using GUIFramework.Managers;
using GUISkinFramework;
using GUISkinFramework.Common;
using MessageFramework.DataObjects;
using MPDisplay.Common.Settings;

namespace GUIFramework
{
    /// <summary>
    /// Interaction logic for GUISurface.xaml
    /// </summary>
  //  [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)] 
    public partial class GUISurface : UserControl, INotifyPropertyChanged
    {
        private XmlSkinInfo _currentSkin;
        //private APIConnection _connection;
        //private EndpointAddress serverEndpoint;
        //private NetTcpBinding serverBinding;
        private ObservableCollection<IControlHost> _surfaceElements = new ObservableCollection<IControlHost>();

        public GUISurface()
        {
            InitializeComponent();
            GUIActionManager.ActionService.Register<XmlAction>(XmlActionType.CloseWindow, async action => await CloseMPDisplayWindow());
            GUIActionManager.ActionService.Register<XmlAction>(XmlActionType.LockWindow,  action =>  LockMPDisplayWindow());
            GUIActionManager.ActionService.Register<XmlAction>(XmlActionType.OpenWindow, async action => await OpenMPDisplayWindow(action.GetParam1As<int>(-1), false));
            GUIActionManager.ActionService.Register<XmlAction>(XmlActionType.OpenDialog, async action => await OpenMPDisplayDialog(action.GetParam1As<int>(-1)));
            GUIActionManager.ActionService.Register<XmlAction>(XmlActionType.PreviousWindow, async action => await OpenPreviousMPDisplayWindow());
            GUIActionManager.ActionService.Register<XmlAction>(XmlActionType.Connect, async action => await GUIMessageService.Instance.Reconnect());

            InfoRepository.RegisterMessage<int>(InfoMessageType.WindowId,async (o) => await OpenMediaPortalWindow(o));
        }

    





        private GUISettings _settings;

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
                IsSplashScreenVisible = true;
                CurrentSkin = skinInfo;
                Width = CurrentSkin.SkinWidth;
                Height = CurrentSkin.SkinHeight;
                SplashScreenSkinText = string.Format("{0}, Author: {1}", CurrentSkin.SkinName, CurrentSkin.Author);
                SplashScreenImage = CurrentSkin.SplashScreenImage;
                await SetSplashScreenText("Loading Skin...");

                await SetSplashScreenText("Loading Skin Files...");
                await Dispatcher.InvokeAsync(() =>
                {
                    CurrentSkin.LoadXmlSkin();
                    GUIDataRepository.Instance.Initialize(Settings, CurrentSkin);
                });
               
                await SetSplashScreenText("Loading Windows...");
                await Dispatcher.InvokeAsync(() =>
                {
                    SurfaceElements.Clear();
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
                GUIMessageService.Instance.InitializeServerConnection(Settings.ConnectionSettings);
                await GUIMessageService.Instance.ConnectToService();
             
                await SetSplashScreenText("Starting MPDisplay++...");
                if (GUIVisibilityManager.IsMediaPortalConnected())
                {
                    if (InfoRepository.Instance.WindowId != -1)
                    {
                        await OpenMediaPortalWindow(InfoRepository.Instance.WindowId);
                    }
                    else if (MediaPortalWindows.DefaultWindow() != null)
                    {
                        await OpenMediaPortalWindow(MediaPortalWindows.DefaultWindow().Id);
                    }
                    else if (MPDisplayWindows.DefaultWindow() != null)
                    {
                        await OpenMPDisplayWindow(MPDisplayWindows.DefaultWindow().Id, false);
                    }
                }
                else if (MPDisplayWindows.DefaultWindow() != null)
                {
                    await OpenMPDisplayWindow(MPDisplayWindows.DefaultWindow().Id, false);
                }
                else
                {
                    SplashScreenImage = CurrentSkin.ErrorScreenImage;
                    await SetSplashScreenText("No Default windows found!");
                    return;
                }
                SplashScreenImage = null;
                IsSplashScreenVisible = false;
            }
            SplashScreenImage = CurrentSkin.ErrorScreenImage;
            await SetSplashScreenText("Failed to load skin");
        }


        public void ChangeSkinStyle()
        {
            if (CurrentSkin != null)
            {
                CurrentSkin.CycleTheme();
            }
        }

        private GUIWindow _currentWindow;
        private GUIMPDialog _currentMediaPortalDialog;
        private List<GUIMPDDialog> _currentMPDisplayDialogs = new List<GUIMPDDialog>();
        private bool _currentWindowIsLocked;
        private Queue<int> _previousMPDisplayWindows = new Queue<int>();

        private async Task OpenMPDisplayWindow(int windowId, bool isPrevious)
        {
            var newWindow = MPDisplayWindows.FirstOrDefault(w => w.Id == windowId);
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
                    await _currentWindow.WindowClose();
                }

                _currentWindow = newWindow;
                await _currentWindow.WindowOpen();
            }
        }

        private async Task OpenPreviousMPDisplayWindow()
        {
            if (_previousMPDisplayWindows.Any())
            {
               await OpenMPDisplayWindow(_previousMPDisplayWindows.Dequeue(), true);
            }
        }

        private void LockMPDisplayWindow()
        {
            _currentWindowIsLocked = !_currentWindowIsLocked;
        }

        private async Task CloseMPDisplayWindow()
        {
            if (_currentWindow is GUIMPDWindow)
            {
                _currentWindowIsLocked = false;
                if (GUIVisibilityManager.IsMediaPortalConnected())
                {
                    await _currentWindow.WindowClose();
                    await OpenMediaPortalWindow(InfoRepository.Instance.WindowId);
                    return;
                }
                if (!_currentWindow.IsDefault)
                {
                    await _currentWindow.WindowClose();
                    var defaultWindow = MPDisplayWindows.FirstOrDefault(w => w.IsDefault) ?? MPDisplayWindows.FirstOrDefault();
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

        private async Task OpenMediaPortalWindow(int windowId)
        {
            if (!_currentWindowIsLocked)
            {
                var newWindow = MediaPortalWindows.FirstOrDefault(w => w.Id == windowId);
                if (newWindow != null && newWindow != _currentWindow)
                {
                    if (_currentWindow != null)
                    {
                        await _currentWindow.WindowClose();
                    }

                    _currentWindow = newWindow;
                    await _currentWindow.WindowOpen();
                }
            }
        }

        private async Task OpenMediaPortalDialog(int dialogId)
        {
            var newDialog = MediaPortalDialogs.FirstOrDefault(w => w.Id == dialogId);
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

        public IEnumerable<GUIMPDDialog> MPDisplayDialogs
        {
            get { return _surfaceElements.OfType<GUIMPDDialog>(); }
        }

        public IEnumerable<GUIMPDialog> MediaPortalDialogs
        {
            get { return _surfaceElements.OfType<GUIMPDialog>(); }
        }





   









        #region Connection

        //private void InitializeServerConnection(ConnectionSettings settings)
        //{
        //    serverEndpoint = new EndpointAddress(string.Format("net.tcp://{0}:{1}/MPDisplayService", settings.IpAddress, settings.Port));
        //    serverBinding = new NetTcpBinding();

        //    // Security (lol)
        //    serverBinding.Security.Mode = SecurityMode.None;
        //    serverBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
        //    serverBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
        //    serverBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

        //    // Connection
        //    serverBinding.Name = "NetTcpBinding_IMessage";
        //    serverBinding.CloseTimeout = new TimeSpan(0, 10, 0);
        //    serverBinding.OpenTimeout = new TimeSpan(0, 10, 0);
        //    serverBinding.ReceiveTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)
        //    serverBinding.SendTimeout = new TimeSpan(0, 10, 0);
        //    serverBinding.TransferMode = TransferMode.Buffered;
        //    serverBinding.ListenBacklog = 1;
        //    serverBinding.MaxConnections = 10;
        //    serverBinding.MaxReceivedMessageSize = 1000000;
        //    serverBinding.MaxBufferSize = 1000000;
        //    serverBinding.MaxBufferPoolSize = 1000000;

        //    // Message
        //    serverBinding.ReaderQuotas.MaxArrayLength = 1000000;
        //    serverBinding.ReaderQuotas.MaxDepth = 32;
        //    serverBinding.ReaderQuotas.MaxStringContentLength = 1000000;
        //    serverBinding.ReaderQuotas.MaxBytesPerRead = 1000000;
        //    serverBinding.ReliableSession.Enabled = true;
        //    serverBinding.ReliableSession.InactivityTimeout = new TimeSpan(7, 0, 0, 0);//7 days should be enough :)

        //    InstanceContext site = new InstanceContext(this);
        //    GUIMessageManager.MessageBroker = new MessageClient(site, serverBinding, serverEndpoint);

        //    //Remove any old events, otherwise when we reconnect we're creating extra events
        //    GUIMessageManager.MessageBroker.InnerChannel.Faulted -= Channel_Faulted;
        //    GUIMessageManager.MessageBroker.InnerChannel.Faulted += new EventHandler(Channel_Faulted);

        //    _connection = new APIConnection(settings.ConnectionName);
        //}

        //private async Task ConnectToService()
        //{
        //    if (GUIMessageManager.MessageBroker != null)
        //    {
        //        await Disconnect();
        //        var task = GUIMessageManager.MessageBroker.ConnectAsync(_connection);
        //        if (await Task.WhenAny(task, Task.Delay(5000)) == task)
        //        {
        //            foreach (var connection in task.Result)
        //            {
        //              // await Dispatcher.InvokeAsync(() => GUIInfoManager.ProcessConnectionMessage(connection, true));
        //            }
        //        }
        //    }
        //}

        //private async Task Reconnect()
        //{
        //    await Disconnect();
        //    InitializeServerConnection(Settings.ConnectionSettings);
        //    await ConnectToService();
        //}

        //private async Task Disconnect()
        //{
        //    if (GUIMessageManager.MessageBroker != null)
        //    {
        //        try
        //        {
        //            await Task.WhenAny(GUIMessageManager.MessageBroker.DisconnectAsync(), Task.Delay(5000));
        //        }
        //        catch { }
        //    }
        //}

        //public async void SessionConnected(APIConnection connection)
        //{
        //   //await Dispatcher.InvokeAsync(() => GUIInfoManager.ProcessConnectionMessage(connection, true));
        //   //if (connection != null && connection.ConnectionName == _connection.ConnectionName)
        //   //{
        //   //    await Dispatcher.InvokeAsync(() => GUIInfoManager.ProcessConnectionMessage(connection, true));
        //   //}
        //}

        //public async void SessionDisconnected(APIConnection connection)
        //{
        //   // await Dispatcher.InvokeAsync(() => GUIInfoManager.ProcessConnectionMessage(connection, false));
        //}


        //private async void Channel_Faulted(object sender, EventArgs e)
        //{
        //  // await Dispatcher.InvokeAsync(() =>GUIInfoManager.ProcessConnectionMessage(null, false, true));
        //}


        //public async void ReceiveAPIPropertyMessage(APIPropertyMessage message)
        //{
        //   await GUIDataRepository.PropertyManager.AddProperty(message);
        //}

        //public async void ReceiveAPIListMessage(APIListMessage message)
        //{
        //    await GUIDataRepository.ListManager.AddListData(message);
        //}

        //public void ReceiveAPIInfoMessage(APIInfoMessage message)
        //{
           
        //}

        //public async void ReceiveAPIDataMessage(APIDataMessage message)
        //{
        //   // GUIDataRepository.AddDataMessage(message);
        //}

        //public void ReceiveMediaPortalMessage(APIMediaPortalMessage message)
        //{
         
        //}

        //public void ReceiveTVServerMessage(APITVServerMessage message)
        //{
           
        //}

     

        #endregion

        #region SplashScreen

        private string _splashScreenImage;
        private string _splashScreenText;
        private bool _isSplashScreenVisible;

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

        private string _splashScreenSkinText;

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
