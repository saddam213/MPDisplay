using System.Windows;
using System.Windows.Forms;
using Common.Log;
using Common.Settings;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class GUISettingsView
    {
        #region Fields

        private LogLevel _logLevel = RegistrySettings.LogLevel;
        private Screen _selectedDisplay;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUISettingsView"/> class.
        /// </summary>
        public GUISettingsView()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set { _logLevel = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the selected display.
        /// </summary>
        public Screen SelectedDisplay
        {
            get { return _selectedDisplay; }
            set { _selectedDisplay = value; NotifyPropertyChanged(); SetScreenSettings(DataObject as GUISettings); }
        }

        /// <summary>
        /// Gets the tabs title.
        /// </summary>
        public override string Title
        {
            get { return "MPDisplay"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the changes.
        /// </summary>
        public override void SaveChanges()
        {
            base.SaveChanges();
            if (_logLevel != RegistrySettings.LogLevel)
            {
                RegistrySettings.SetRegistryValue(RegistrySettings.MPDisplayKeys.LogLevel, _logLevel.ToString());
            }
        }

        /// <summary>
        /// Sets the screen settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void SetScreenSettings(GUISettings settings)
        {
            if (settings == null) return;
            if (!settings.CustomResolution)
            {
                if (SelectedDisplay == null) return;
                settings.ScreenHeight = SelectedDisplay.Bounds.Height;
                settings.ScreenWidth = SelectedDisplay.Bounds.Width;
                settings.ScreenOffSetX = SelectedDisplay.Bounds.X;
                settings.ScreenOffSetY = SelectedDisplay.Bounds.Y;
            }
            else
            {
                settings.ScreenOffSetX = 0;
                settings.ScreenOffSetY = 0;
            }
        }

        /// <summary>
        /// Handles the Checked and Unchecked event of the CheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            SetScreenSettings(DataObject as GUISettings);
        }

        #endregion
    }
}
