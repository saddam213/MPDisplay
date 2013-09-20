using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
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
using GUIFramework.Utils;
using Microsoft.Win32;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;
using System.Threading.Tasks;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Log Log = LoggingManager.GetLog(typeof(MainWindow));
        private GUISettings _settings;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            //   Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),   new FrameworkPropertyMetadata { DefaultValue = 30 });
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
          //  RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            //     RenderOptions.SetCachingHint(this, CachingHint.Cache);
            // RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
         //   TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
         //   TextOptions.SetTextHintingMode(this, TextHintingMode.Animated);
         //   TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            
            InitializeComponent();
            DataContext = this;
            LoadSettings();
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Exception("[UnhandledException] - An unknown exception occured", e.Exception);
            e.Handled = true;
            RestartMPDisplay();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Message(LogLevel.Error, "[UnhandledException] - An unknown exception occured{0}{1}", Environment.NewLine, e);
            RestartMPDisplay();
        }

        #endregion






        public GUISettings Settings
        {
            get { return _settings; }
            set { _settings = value; NotifyPropertyChanged("Settings"); }
        }



        /// <summary>
        /// Load MPDisplay.xml
        /// </summary>
        private async void LoadSettings()
        {
            var settings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (settings == null)
            {
                settings = new MPDisplaySettings();
                SettingsManager.Save<MPDisplaySettings>(settings, RegistrySettings.MPDisplaySettingsFile);
            }
            settings.GUISettings.SkinInfoXml = string.Format("{0}{1}\\SkinInfo.xml", RegistrySettings.MPDisplaySkinFolder, settings.GUISettings.SkinName);
            if (!File.Exists(settings.GUISettings.SkinInfoXml))
            {
                MessageBox.Show("File does not exist: " + settings.GUISettings.SkinInfoXml, "Error!", MessageBoxButton.OK);
                Close();
            }
            Settings = settings.GUISettings;


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
                if (dpiX != 96.0 || dpiX != 96.0)
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

            if (!_settings.DesktopMode)
            {
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
            }


            await surface.LoadSkin(Settings);
        }


        private void RestartMPDisplay()
        {
            try
            {
                surface.CloseDown(false);
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

        protected override void OnClosing(CancelEventArgs e)
        {
            Log.Message(LogLevel.Info, "[OnClosing] - Close Requested.");
            surface.CloseDown(false);
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Log.Message(LogLevel.Info, "[OnClosing] - Close complete.");
            LoggingManager.Destroy();
            base.OnClosed(e);
        }

        protected async override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            await Task.Delay(250);
            FocusHelper.ActivateApplication("MediaPortal");
        }

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
    }
}

