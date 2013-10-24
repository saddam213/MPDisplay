﻿using System;
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
using GUIFramework.Utils;
using Microsoft.Win32;

namespace GUIFramework
{
    /// <summary>
    /// Interaction logic for GUISurface.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class GUISurface : UserControl, IMessageCallback, INotifyPropertyChanged 
    {
        #region Fields

        private Log Log = LoggingManager.GetLog(typeof(GUISurface));
        private APIConnection _connection;
        private EndpointAddress serverEndpoint;
        private NetTcpBinding serverBinding;
        private bool _isDisconnecting = false;
        private static MessageClient _messageBroker;
        private XmlSkinInfo _currentSkin;
        private GUISettings _settings;
        private ObservableCollection<IControlHost> _surfaceElements = new ObservableCollection<IControlHost>();
        private GUIWindow _currentWindow;
        private GUIMPDialog _currentMediaPortalDialog;
        private List<GUIMPDDialog> _currentMPDisplayDialogs = new List<GUIMPDDialog>();
        private bool _currentWindowIsLocked;
        private Stack<int> _previousWindows = new Stack<int>();
        private DispatcherTimer _secondTimer;
        private bool _processWindow = false;
        private int _currentMPDWindowId = -1;
        private DateTime _lastUserInteraction = DateTime.MinValue;
        private DateTime _lastKeepAlive = DateTime.MinValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUISurface"/> class.
        /// </summary>
        public GUISurface()
        {
            InitializeComponent(); 
            StartSecondTimer();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public GUISettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// Gets or sets the current skin.
        /// </summary>
        public XmlSkinInfo CurrentSkin
        {
            get { return _currentSkin; }
            set { _currentSkin = value; }
        }

        /// <summary>
        /// Gets or sets the surface elements.
        /// </summary>
        public ObservableCollection<IControlHost> SurfaceElements
        {
            get { return _surfaceElements; }
            set { _surfaceElements = value; }
        }

        /// <summary>
        /// Gets the mp display windows.
        /// </summary>
        public IEnumerable<GUIMPDWindow> MPDisplayWindows
        {
            get { return _surfaceElements.OfType<GUIMPDWindow>(); }
        }

        /// <summary>
        /// Gets the media portal windows.
        /// </summary>
        public IEnumerable<GUIMPWindow> MediaPortalWindows
        {
            get { return _surfaceElements.OfType<GUIMPWindow>(); }
        }

        /// <summary>
        /// Gets the player windows.
        /// </summary>
        public IEnumerable<GUIPlayerWindow> PlayerWindows
        {
            get { return _surfaceElements.OfType<GUIPlayerWindow>(); }
        }

        /// <summary>
        /// Gets the mp display dialogs.
        /// </summary>
        public IEnumerable<GUIMPDDialog> MPDisplayDialogs
        {
            get { return _surfaceElements.OfType<GUIMPDDialog>(); }
        }

        /// <summary>
        /// Gets the media portal dialogs.
        /// </summary>
        public IEnumerable<GUIMPDialog> MediaPortalDialogs
        {
            get { return _surfaceElements.OfType<GUIMPDialog>(); }
        }

        /// <summary>
        /// Gets a value indicating whether [is user interacting].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is user interacting]; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserInteracting
        {
            get { return DateTime.Now < _lastUserInteraction.AddSeconds(10); }
        } 
        #endregion

        #region Load Skin

        /// <summary>
        /// Loads the skin.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Loads the skin.
        /// </summary>
        /// <param name="skinInfo">The skin information.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public async Task LoadSkin(XmlSkinInfo skinInfo, GUISettings settings)
        {
            if (settings != null && skinInfo != null)
            {
                Settings = settings;
                await LoadSkin(skinInfo);
            }
        }

        /// <summary>
        /// Loads the skin.
        /// </summary>
        /// <param name="skinInfo">The skin information.</param>
        /// <returns></returns>
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
                    SetRepositories();
                });


                Log.Message(LogLevel.Info, "[LoadSkin] - Loading VisibleConditions...");
                await SetSplashScreenText("Loading VisibleConditions...");
                DateTime conditionStart = DateTime.Now;
                GUIVisibilityManager.CreateVisibleConditions(CurrentSkin.Windows.Cast<IXmlControlHost>().Concat(CurrentSkin.Dialogs));
                Log.Message(LogLevel.Info, "[LoadSkin] - Loading VisibleConditions, LoadTime: {0}ms", (DateTime.Now - conditionStart).TotalMilliseconds);



                Log.Message(LogLevel.Info, "[LoadSkin] - Loading GUIWindows...");
                await SetSplashScreenText("Loading Windows...");
                DateTime windowsStart = DateTime.Now;
                foreach (var window in CurrentSkin.Windows.OrderByDescending(x => x.IsDefault))
                {
                    await SetSplashScreenText("Loading Window...({0})", window.Name);
                    await Dispatcher.InvokeAsync(() =>
                    {
                        DateTime start = DateTime.Now;
                        SurfaceElements.Add(GUIElementFactory.CreateWindow(window));
                        Log.Message(LogLevel.Verbose, "[LoadSkin] - Loading GUIWindow {0}, Took: {1}ms", window.Name, (DateTime.Now - start).TotalMilliseconds);
                    }, DispatcherPriority.Background);
                }
                Log.Message(LogLevel.Info, "[LoadSkin] - Loading GUIWindows Complete, LoadTime: {0}ms", (DateTime.Now - windowsStart).TotalMilliseconds);


                Log.Message(LogLevel.Info, "[LoadSkin] - Loading GUIDialogs...");
                await SetSplashScreenText("Loading Dialogs...");
                DateTime dialogsStart = DateTime.Now;
                foreach (var dialog in CurrentSkin.Dialogs)
                {
                    await SetSplashScreenText("Loading Dialog...({0})", dialog.Name);
                    await Dispatcher.InvokeAsync(() =>
                    {
                        DateTime start = DateTime.Now;
                        SurfaceElements.Add(GUIElementFactory.CreateDialog(dialog));
                        Log.Message(LogLevel.Verbose, "[LoadSkin] - Loading GUIDialog {0}, LoadTime: {1}ms", dialog.Name, (DateTime.Now - start).TotalMilliseconds);
                    });
                }
                Log.Message(LogLevel.Info, "[LoadSkin] - Loading GUIDialogs Complete, LoadTime: {0}ms", (DateTime.Now - dialogsStart).TotalMilliseconds);


                await SetSplashScreenText("Connecting to service...");
                await InitializeServerConnection(Settings.ConnectionSettings);


                await SetSplashScreenText("Starting MPDisplay++...");
                if (MPDisplayWindows.Any(w => w.IsDefault) && MediaPortalWindows.Any(w => w.IsDefault))
                {
                    _processWindow = true;
                    StartWindowProcessThread();
                    HideSplashScreen();
                    RegisterCallbacks();
                    return;
                }
                ShowSplashScreenError("No Default windows found!");
                return;
            }
            ShowSplashScreenError("Failed to load skin");
        }

        /// <summary>
        /// Closes down.
        /// </summary>
        public void CloseDown()
        {
            _processWindow = false;
            StopSecondTimer();
            DeregisterCallbacks();
            _isDisconnecting = true;
            Disconnect();
            ClearRepositories(true);
            SurfaceElements.Clear();
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }

        /// <summary>
        /// Registers the callbacks.
        /// </summary>
        private void RegisterCallbacks()
        {
            GUIActionManager.RegisterAction(XmlActionType.CloseWindow, action => CloseWindow());
            GUIActionManager.RegisterAction(XmlActionType.OpenWindow, action => OpenWindow(action.GetParam1As<int>(-1)));
            GUIActionManager.RegisterAction(XmlActionType.OpenDialog, async action => await OpenMPDisplayDialog(action.GetParam1As<int>(-1)));
            GUIActionManager.RegisterAction(XmlActionType.PreviousWindow, action => OpenPreviousWindow());
            GUIActionManager.RegisterAction(XmlActionType.Connect, async action => await Reconnect());
            GUIActionManager.RegisterAction(XmlActionType.LockWindow, action => LockWindow());
            GUIActionManager.RegisterAction(XmlActionType.MediaPortalWindow, async action => await SendMediaPortalAction(action));
            GUIActionManager.RegisterAction(XmlActionType.MediaPortalAction, async action => await SendMediaPortalAction(action));
            GUIActionManager.RegisterAction(XmlActionType.SwitchTheme, action => CurrentSkin.CycleTheme());
            GUIActionManager.RegisterAction(XmlActionType.Exit, action => ExitMPDisplay());
            GUIActionManager.RegisterAction(XmlActionType.NowPlaying, action => NowPlaying());
            GUIActionManager.RegisterAction(XmlActionType.RunProgram, StartApplication);
            GUIActionManager.RegisterAction(XmlActionType.KillProgram, StopApplication);

            InfoRepository.RegisterMessage<int>(InfoMessageType.DialogId, async windowId => await OpenMediaPortalDialog());
            ListRepository.RegisterMessage<APIListAction>(ListServiceMessage.SendItem, async item => await SendListAction(item));
        }

        /// <summary>
        /// Deregisters the callbacks.
        /// </summary>
        private void DeregisterCallbacks()
        {
            GUIActionManager.DeregisterAction(XmlActionType.CloseWindow, this);
            GUIActionManager.DeregisterAction(XmlActionType.OpenWindow, this);
            GUIActionManager.DeregisterAction(XmlActionType.OpenDialog, this);
            GUIActionManager.DeregisterAction(XmlActionType.PreviousWindow, this);
            GUIActionManager.DeregisterAction(XmlActionType.Connect, this);
            GUIActionManager.DeregisterAction(XmlActionType.LockWindow, this);
            GUIActionManager.DeregisterAction(XmlActionType.MediaPortalWindow, this);
            GUIActionManager.DeregisterAction(XmlActionType.MediaPortalAction, this);
            GUIActionManager.DeregisterAction(XmlActionType.SwitchTheme, this);
            GUIActionManager.DeregisterAction(XmlActionType.Exit, this);
            GUIActionManager.DeregisterAction(XmlActionType.NowPlaying, this);
            GUIActionManager.DeregisterAction(XmlActionType.RunProgram, this);
            GUIActionManager.DeregisterAction(XmlActionType.KillProgram, this);

            InfoRepository.DeregisterMessage(InfoMessageType.DialogId, this);
            ListRepository.DeregisterMessage(ListServiceMessage.SendItem, this);
        }

        /// <summary>
        /// Sets the repositories.
        /// </summary>
        private void SetRepositories()
        {
            Log.Message(LogLevel.Info, "[LoadSkin] - Setting repositories..");
            GUIImageManager.Init(CurrentSkin);
            InfoRepository.Instance.Initialize(Settings, CurrentSkin);
            PropertyRepository.Instance.Initialize(Settings, CurrentSkin);
            ListRepository.Instance.Initialize(Settings, CurrentSkin);
            GenericDataRepository.Instance.Initialize(Settings, CurrentSkin);
            Log.Message(LogLevel.Info, "[LoadSkin] - Repositories set");
        }

        /// <summary>
        /// Clears the repositories.
        /// </summary>
        private void ClearRepositories(bool isExit)
        {
            Log.Message(LogLevel.Info, "[CloseDown] - Clearing repositories..");
            if (isExit)
            {
                InfoRepository.Instance.ResetRepository();
                ListRepository.Instance.ResetRepository();
                PropertyRepository.Instance.ResetRepository();
                GenericDataRepository.Instance.ResetRepository();
            }
            else
            {
                InfoRepository.Instance.ClearRepository();
                ListRepository.Instance.ClearRepository();
                PropertyRepository.Instance.ClearRepository();
                GenericDataRepository.Instance.ClearRepository();
            }
            Log.Message(LogLevel.Info, "[CloseDown] - Repositories cleared");
        }
      
        #endregion

        #region Windows

        /// <summary>
        /// Starts the window process thread.
        /// </summary>
        private async void StartWindowProcessThread()
        {
            while (_processWindow)
            {
                await Task.Delay(150);
                await ProcessWindow();
            }
        }

        /// <summary>
        /// Processes the window.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessWindow()
        {
            if (_currentWindowIsLocked)
            {
                return;
            }

            if (InfoRepository.Instance.IsMediaPortalConnected && !IsUserInteracting)
            {
                _currentMPDWindowId = -1;
                _lastUserInteraction = DateTime.MinValue;
            }


            if (_currentMPDWindowId != -1 || !InfoRepository.Instance.IsMediaPortalConnected)
            {
                var newWindow = MPDisplayWindows.GetOrDefault(_currentMPDWindowId);
                if (!IsWindowOpen(newWindow))
                {
                    foreach (var dialog in MPDisplayDialogs.Where(d => d.CloseOnWindowChanged && IsDialogOpen(d)))
                    {
                        await dialog.DialogClose();
                    }
                    AddPreviousWindow(_currentMPDWindowId);
                    await ChangeWindow(newWindow);
                }
            }
            else
            {
                if (InfoRepository.Instance.PlaybackType != APIPlaybackType.None)
                {
                    if (InfoRepository.Instance.IsFullscreenVideo || InfoRepository.Instance.IsFullscreenMusic)
                    {
                        await ChangeWindow(PlayerWindows.GetOrDefault());
                        return;
                    }
                    await ChangeWindow(MediaPortalWindows.GetOrDefault());
                    return;
                }
                await ChangeWindow(MediaPortalWindows.GetOrDefault());
            }
        }

        /// <summary>
        /// Changes the window.
        /// </summary>
        /// <param name="newWindow">The new window.</param>
        /// <returns></returns>
        private async Task ChangeWindow(GUIWindow newWindow)
        {
            if (newWindow != null)
            {
                if (!IsWindowOpen(newWindow))
                {
                    if (IsWindowOpen(_currentWindow))
                    {
                        await _currentWindow.WindowClose();
                    }
                    await RegisterWindowData(newWindow);
                    _currentWindow = newWindow;
                    await _currentWindow.WindowOpen();
                }
            }
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        private void CloseWindow()
        {
            if (_currentWindowIsLocked)
            {
                LockWindow();
            }
            _currentMPDWindowId = -1;
        }

        /// <summary>
        /// Opens the window.
        /// </summary>
        /// <param name="windowId">The window identifier.</param>
        private void OpenWindow(int windowId)
        {
            if (!_currentWindowIsLocked)
            {
                _currentMPDWindowId = MPDisplayWindows.GetOrDefault(windowId).Id;
            }
        }

        /// <summary>
        /// Locks the window.
        /// </summary>
        private void LockWindow()
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

        /// <summary>
        /// Opens the previous window.
        /// </summary>
        private void OpenPreviousWindow()
        {
            if (_currentWindowIsLocked)
            {
                return;
            }

            if (_previousWindows.Any())
            {
                _currentMPDWindowId = _previousWindows.Pop();
            }
        }

        /// <summary>
        /// Adds the previous window.
        /// </summary>
        /// <param name="windowId">The window identifier.</param>
        private void AddPreviousWindow(int windowId)
        {
            if (_previousWindows.Any())
            {
                if (_previousWindows.Peek() == windowId)
                {
                    _previousWindows.Pop();
                    return;
                }
            }
            _previousWindows.Push(windowId);
        }

        /// <summary>
        /// Determines whether [is window open] [the specified window].
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        private bool IsWindowOpen(GUIWindow window)
        {
            return window != null && window.IsOpen;
        }

        /// <summary>
        /// Registers the window data.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        private Task RegisterWindowData(GUIWindow window)
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

        #endregion

        #region Dialogs

        /// <summary>
        /// Opens the mp display dialog. 
        /// </summary>
        /// <param name="dialogId">The dialog identifier.</param>
        /// <returns></returns>
        private async Task OpenMPDisplayDialog(int dialogId)
        {
            var existing = MPDisplayDialogs.FirstOrDefault(w => w.Id == dialogId && IsDialogOpen(w));
            if (existing != null)
            {
                await existing.DialogClose();
                return;
            }

            var newDialog = MPDisplayDialogs.FirstOrDefault(w => w.Id == dialogId && !IsDialogOpen(w));
            if (newDialog != null)
            {
                await newDialog.DialogOpen();
            }
        }

        /// <summary>
        /// Opens the media portal dialog.
        /// </summary>
        /// <returns></returns>
        private async Task OpenMediaPortalDialog()
        {
            if (IsDialogOpen(_currentMediaPortalDialog) && InfoRepository.Instance.DialogId == -1)
            {
                await _currentMediaPortalDialog.DialogClose();
                return;
            }

            var newDialog = MediaPortalDialogs.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible());
            if (!IsDialogOpen(newDialog))
            {
                if (IsDialogOpen(_currentMediaPortalDialog))
                {
                    await _currentMediaPortalDialog.DialogClose();
                }

                if (newDialog != null)
                {
                    _currentMediaPortalDialog = newDialog;
                    await _currentMediaPortalDialog.DialogOpen();
                }
            }
        }

        /// <summary>
        /// Determines whether [is dialog open] [the specified dialog].
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns></returns>
        private bool IsDialogOpen(GUIDialog dialog)
        {
            return dialog != null && dialog.IsOpen;
        }

        /// <summary>
        /// Registers the dialog data.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns></returns>
        private Task RegisterDialogData(GUIDialog dialog)
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

        #endregion 

        #region Actions

        /// <summary>
        /// Changes the skin style.
        /// </summary>
        private void ChangeSkinStyle()
        {
            if (CurrentSkin != null)
            {
                CurrentSkin.CycleTheme();
            }
        }

        /// <summary>
        /// Exits the mp display.
        /// </summary>
        private void ExitMPDisplay()
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Nows the playing.
        /// </summary>
        private void NowPlaying()
        {
            if (InfoRepository.Instance.PlaybackType != APIPlaybackType.None)
            {
                _lastUserInteraction = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="action">The action.</param>
        private void StartApplication(XmlAction action)
        {
            ProgramHelper.StartApplication(action.Param1, action.Param2);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        /// <param name="action">The action.</param>
        private void StopApplication(XmlAction action)
        {
            ProgramHelper.KillApplication(action.Param1);
        } 

        #endregion

        #region Timer

        /// <summary>
        /// Starts the second timer.
        /// </summary>
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

        /// <summary>
        /// Stops the second timer.
        /// </summary>
        private void StopSecondTimer()
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

        #endregion

        #region Connection

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
            Disconnect();
        }

        public async Task ConnectToService()
        {
            if (_messageBroker != null)
            {
                try
                {
                
                    Log.Message(LogLevel.Info, "[Connect] - Connecting to server.");
                    var result = await _messageBroker.ConnectAsync(_connection);
                    if (result != null && result.Any())
                    {
                        Log.Message(LogLevel.Info, "[Connect] - Connection to server successful.");
                        InfoRepository.Instance.IsMPDisplayConnected = true;
                        InfoRepository.Instance.IsMediaPortalConnected = result.Any(x => x.ConnectionName.Equals("MediaPortalPlugin"));
                        InfoRepository.Instance.IsTVServerConnected = result.Any(x => x.ConnectionName.Equals("TVServerPlugin"));
                        _lastKeepAlive = DateTime.Now;
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
            InfoRepository.Instance.IsMPDisplayConnected = false;
            InfoRepository.Instance.IsMediaPortalConnected = false;
            InfoRepository.Instance.IsTVServerConnected = false;
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
                    Disconnect();
                }

                if (connection.ConnectionName.Equals("MediaPortalPlugin"))
                {
                    Log.Message(LogLevel.Info, "[Session] - MediaPortalPlugin disconnected from network.");
                    ClearRepositories(false);
                    InfoRepository.Instance.IsMediaPortalConnected = false;
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

        public Task SendListAction(APIListAction action)
        {
            return SendMediaPortalMessage(new APIMediaPortalMessage
            {
                MessageType = APIMediaPortalMessageType.ActionMessage,
                ActionMessage = new APIActionMessage
                {
                    ActionType = action.ItemListType == APIListType.DialogList 
                           ? APIActionMessageType.DialogListAction 
                           : APIActionMessageType.WindowListAction,
                    ListAction = action
                }
            });
        }



        public async Task SendKeepAlive()
        {
            if (InfoRepository.Instance.IsMPDisplayConnected)
            {
                if (DateTime.Now > _lastKeepAlive.AddSeconds(45))
                {
                    await Disconnect();
                }

                if (DateTime.Now > _lastKeepAlive.AddSeconds(30))
                {
                    Log.Message(LogLevel.Info, "[KeepAlive] - Sending KeepAlive message.");
                    await SendKeepAliveMessage();
                }
            }
        }

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

        private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
              await Task.Delay(_settings.ConnectionSettings.ResumeDelay);
              await InitializeServerConnection(_settings.ConnectionSettings);
            }

            if (e.Mode == PowerModes.Suspend)
            {
               await Disconnect();
            }
        }


        #region Receive

        public async void ReceiveAPIPropertyMessage(APIPropertyMessage message)
        {
            await Task.Factory.StartNew(() => PropertyRepository.Instance.AddProperty(message));
        }

        public async void ReceiveAPIListMessage(APIListMessage message)
        {
            await Task.Factory.StartNew(() => ListRepository.Instance.AddListData(message));
        }

        public async void ReceiveAPIInfoMessage(APIInfoMessage message)
        {
            await Task.Factory.StartNew(() => InfoRepository.Instance.AddInfo(message));
        }

        public async void ReceiveAPIDataMessage(APIDataMessage message)
        {
            if (message != null && message.DataType == APIDataMessageType.KeepAlive)
            {
                _lastKeepAlive = DateTime.Now;
                return;
            }
            await Task.Factory.StartNew(() => GenericDataRepository.Instance.AddDataMessage(message));
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

        #region Mouse Events

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _lastUserInteraction = DateTime.Now;
            base.OnMouseDown(e);
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
