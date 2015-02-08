using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Helpers;
using GUIConfig.Settings.Language;
using GUIConfig.Dialogs;
using GUIConfig.ViewModels;
using Common.Logging;
using Common.Settings;

namespace GUIConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        private Log Log = LoggingManager.GetLog(typeof(MainWindow));
        private ObservableCollection<ViewModelBase> _views = new ObservableCollection<ViewModelBase>();
        private MPDisplaySettings _mpdisplaySettings;
        private bool _hasChanges = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            LanguageHelper.LoadLanguage();
            if (!LanguageHelper.HasLanguage(RegistrySettings.ConfigLanguage))
            {
                var dialog = new LanguageDialog();
                dialog.ShowDialog();
                RegistrySettings.SetRegistryValue(RegistrySettings.MPDisplayKeys.LanguageFile, dialog.SelectedLanguage);
            }
            LanguageHelper.SetLanguage(RegistrySettings.ConfigLanguage);
            InitializeComponent();

            Log.Message(LogLevel.Info, "Loading settings...");
            MPDisplaySettings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (MPDisplaySettings == null)
            {
                Log.Message(LogLevel.Warn, "MPDisplay.xml not found!, creating new settings file.");
                MPDisplaySettings = new MPDisplaySettings();
            }
            Log.Message(LogLevel.Info, "MPDisplay.xml sucessfully loaded.");
            LoadViews();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the views.
        /// </summary>
        public ObservableCollection<ViewModelBase> Views
        {
            get { return _views; }
            set { _views = value; }
        }

        /// <summary>
        /// Gets or sets the mp display settings.
        /// </summary>
        public MPDisplaySettings MPDisplaySettings
        {
            get { return _mpdisplaySettings; }
            set { _mpdisplaySettings = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the views.
        /// </summary>
        private void LoadViews()
        {
            Log.Message(LogLevel.Info, "Loading views for InstallType: {0}", RegistrySettings.InstallType);
            if (RegistrySettings.InstallType != MPDisplayInstallType.GUI)
            {
                Log.Message(LogLevel.Info, "Adding Plugin section.");
                Views.Add(new PluginSettingsView { DataObject = MPDisplaySettings.PluginSettings });
                MPDisplaySettings.PluginSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
                MPDisplaySettings.PluginSettings.ConnectionSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
            }

            if (RegistrySettings.InstallType != MPDisplayInstallType.Plugin)
            {
                Log.Message(LogLevel.Info, "Adding MPDisplay section.");
                Views.Add(new GUISettingsView { DataObject = MPDisplaySettings.GUISettings });
                Log.Message(LogLevel.Info, "Adding Skin section.");
                Views.Add(new SkinSettingsView { DataObject = MPDisplaySettings.GUISettings });
                MPDisplaySettings.GUISettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
                MPDisplaySettings.GUISettings.ConnectionSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_hasChanges)
            {
                if (MessageBox.Show("Would you like to save changes?", "Save Changes?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveChanges();
                }
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            LoggingManager.Destroy();
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        private void SaveChanges()
        {
            MPDisplaySettings.PluginSettings.UserInteractionDelay = MPDisplaySettings.GUISettings.UserInteractionDelay;

            foreach (var view in Views)
            {
                view.SaveChanges();
            }
            SettingsManager.Save<MPDisplaySettings>(MPDisplaySettings, RegistrySettings.MPDisplaySettingsFile);
            _hasChanges = false;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the MPDisplaySettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void MPDisplaySettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _hasChanges = true;
        }

        /// <summary>
        /// Handles the Click event of the Button_Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            Close();
        }

        /// <summary>
        /// Handles the Click event of the Button_Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _hasChanges = false;
            Close();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
