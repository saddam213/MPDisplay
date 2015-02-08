using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Common.Helpers;
using Common.Logging;
using Common.Settings;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        private Log Log = LoggingManager.GetLog(typeof(MainWindow));
        private GUISettings _settings; 

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            InitializeComponent();
            DataContext = this;
            LoadSettings();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public GUISettings Settings
        {
            get { return _settings; }
            set { _settings = value; NotifyPropertyChanged("Settings"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load MPDisplay.xml
        /// </summary>
        private async void LoadSettings()
        {
            Log.Message(LogLevel.Info, "[LoadSettings] - Loading UI settings");
            var settings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (settings == null)
            {
                Log.Message(LogLevel.Warn, "[LoadSettings] - MPDisplay.xml not found!, creating file..");
                settings = new MPDisplaySettings();
                SettingsManager.Save<MPDisplaySettings>(settings, RegistrySettings.MPDisplaySettingsFile);
            }
            Log.Message(LogLevel.Info, "[LoadSettings] - MPDisplay.xml sucessfully loaded.");
            settings.GUISettings.SkinInfoXml = string.Format("{0}{1}\\SkinInfo.xml", RegistrySettings.MPDisplaySkinFolder, settings.GUISettings.SkinName);
            if (!File.Exists(settings.GUISettings.SkinInfoXml))
            {
                Log.Message(LogLevel.Error, "[LoadSettings] - Failed to locate the selected skins info file '{0}'", settings.GUISettings.SkinInfoXml);
                Close();
            }
            Settings = settings.GUISettings;


            if (!_settings.DesktopMode)
            {
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
            }

            await surface.LoadSkin(Settings);
        }

        #endregion

        #region Window Events/Overrides

        /// <summary>
        /// Called when the Window has been loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set base screen size/location, scaled to screens DPI
            var screen = GetDisplayByDeviceName(_settings.Display);
            double left = _settings.CustomResolution ? screen.Bounds.X + _settings.ScreenOffSetX : screen.Bounds.X;
            double top = _settings.CustomResolution ? screen.Bounds.Y + _settings.ScreenOffSetY : screen.Bounds.Y;
            double width = _settings.CustomResolution ? (double)_settings.ScreenWidth : screen.Bounds.Width;
            double height = _settings.CustomResolution ? (double)_settings.ScreenHeight : screen.Bounds.Height;

            PresentationSource source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                double dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                double dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                if (dpiX != 96.0 || dpiY != 96.0)
                {
                    width = width * 96.0 / dpiX;
                    height = height * 96.0 / dpiY;
                    left = left * 96.0 / dpiX;
                    top = top * 96.0 / dpiY;
                }
            }
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            Log.Message(LogLevel.Info, "[Load_Window] - Set UI surface, Width: {0}, Height: {1}, X: {2}, Y: {3}, DesktopMode: {4}", width, height, left, top, _settings.DesktopMode);

        }  

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            Log.Message(LogLevel.Info, "[OnClosing] - Close Requested.");
            surface.CloseDown();
            base.OnClosing(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            Log.Message(LogLevel.Info, "[OnClosing] - Close complete.");
            LoggingManager.Destroy();
            base.OnClosed(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the mouse button was released.</param>
        protected async override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            await Task.Delay(250);
            ProcessHelper.ActivateApplication("MediaPortal");
        }

        /// <summary>
        /// Handles the DispatcherUnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null && e.Exception.StackTrace.Contains("System.Windows.Controls.VirtualizingStackPanel.get_ItemCount()"))
            {
                Log.Message(LogLevel.Warn, "An Error occured in Microsoft VirtualizingStackPanel");
                e.Handled = true;
                return;
            }
            Log.Exception("[UnhandledException] - An unknown exception occured", e.Exception);
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Message(LogLevel.Error, "[UnhandledException] - An unknown exception occured{0}{1}", Environment.NewLine, e);
        } 

        #endregion

        #region Helpers

        private System.Windows.Forms.Screen GetDisplayByDeviceName(string name)
        {
            return System.Windows.Forms.Screen.AllScreens.FirstOrDefault(d => d.DeviceName.Equals(name))
                ?? System.Windows.Forms.Screen.AllScreens.FirstOrDefault(d => d.Primary);
        }

        private void SetThreadPriority(string priority)
        {
            ThreadPriority option = ThreadPriority.Normal;
            Enum.TryParse<ThreadPriority>(priority, out option);
            Thread.CurrentThread.Priority = option;
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


        /// <summary>
        /// Restarts the mp display.
        /// </summary>
        private async void RestartMPDisplay()
        {
            await Task.Delay(2000);
            try
            {
                surface.CloseDown();
            }
            catch (Exception ex)
            {
                Log.Exception("[CloseDown] - Error occured closing down GUISurface", ex);
            }

            surface = null;
            if (_settings.RestartOnError)
            {
                Log.Message(LogLevel.Info, "[RestartMPDisplay] - RestartOnError is enabled, Waiting for user input...");
                if (MessageBox.Show("An exception occured, Would you like to restart?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Log.Message(LogLevel.Info, "[RestartMPDisplay] - RestartOnError requested, Attempting to restart MPDisplay...");
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    LoadSettings();
                    return;
                }

                Log.Message(LogLevel.Info, "[RestartMPDisplay] - RestartOnError declined, Shutting down MPDisplay.");
            }
            Close();
        }
    }
}

