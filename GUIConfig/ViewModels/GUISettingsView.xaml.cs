using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Common.Helpers;
using System.Drawing;
using Common.Logging;
using Common.Settings;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class GUISettingsView : ViewModelBase
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
            set { _selectedDisplay = value; NotifyPropertyChanged("SelectedDisplay"); SetScreenSettings(DataObject as GUISettings); }
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

        /// <summary>
        /// Sets the screen settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void SetScreenSettings(GUISettings settings)
        {
            if (settings != null)
            {
                if (!settings.CustomResolution)
                {
                    if (SelectedDisplay != null)
                    {
                        settings.ScreenHeight = SelectedDisplay.Bounds.Height;
                        settings.ScreenWidth = SelectedDisplay.Bounds.Width;
                        settings.ScreenOffSetX = SelectedDisplay.Bounds.X;
                        settings.ScreenOffSetY = SelectedDisplay.Bounds.Y;
                    }
                }
                else
                {
                    settings.ScreenOffSetX = 0;
                    settings.ScreenOffSetY = 0;
                }
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
