using Common.Log;
using Common.Settings;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class PluginSettingsView
    {
        #region Fields

        private LogLevel _logLevel = RegistrySettings.LogLevel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginSettingsView"/> class.
        /// </summary>
        public PluginSettingsView()
        {            
            InitializeComponent();
            HasPendingChanges = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the tabs title.
        /// </summary>
        public override string Title => "Plugin";

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set { _logLevel = value; NotifyPropertyChanged(); }
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

        #endregion
    }
}
