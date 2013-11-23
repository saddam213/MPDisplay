using System;
using System.Collections.Generic;
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
using Common.Logging;
using Common.Settings;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class PluginSettingsView : ViewModelBase
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
        public override string Title
        {
            get { return "Plugin"; }
        }

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
        /// Called when model tab opens.
        /// </summary>
        public override void OnModelOpen()
        {
            base.OnModelOpen();
        }

        /// <summary>
        /// Called when model tab closes.
        /// </summary>
        public override void OnModelClose()
        {
            base.OnModelClose();
        }

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
