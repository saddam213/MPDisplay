using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using Common.Log;
using Common.Settings;
using GUIConfig.Dialogs;
using GUIConfig.ViewModels;
using GUIConfig.Settings;
using GUIConfig.SupportInfo;
using MessageBox = System.Windows.MessageBox;

namespace GUIConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Fields

        private Log _log = LoggingManager.GetLog(typeof(MainWindow));
        private ObservableCollection<ViewModelBase> _views = new ObservableCollection<ViewModelBase>();
        private MPDisplaySettings _mpdisplaySettings;
        private AddImageSettings _addImageSettings;
        private bool _hasChanges;

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

            _log.Message(LogLevel.Info, "Loading settings...");
            MPDisplaySettings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile);
            if (MPDisplaySettings == null)
            {
                _log.Message(LogLevel.Warn, "MPDisplay.xml not found!, creating new settings file.");
                MPDisplaySettings = new MPDisplaySettings();
            }
            _log.Message(LogLevel.Info, "MPDisplay.xml sucessfully loaded.");

            AddImageSettings = SettingsManager.Load<AddImageSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "AddImageSettings.xml"));
            if (AddImageSettings == null)
            {
                _log.Message(LogLevel.Warn, "AddImageSettings.xml not found!, creating new settings file.");
                AddImageSettings = new AddImageSettings();
            }
            _log.Message(LogLevel.Info, "AddIMageSettings.xml sucessfully loaded.");

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

        /// <summary>
        /// Gets or sets the additional image settings.
        /// </summary>
        public AddImageSettings AddImageSettings
        {
            get { return _addImageSettings; }
            set { _addImageSettings = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the views.
        /// </summary>
        private void LoadViews()
        {
            _log.Message(LogLevel.Info, "Loading views for InstallType: {0}", RegistrySettings.InstallType);
            if (RegistrySettings.InstallType != MPDisplayInstallType.GUI)
            {
                _log.Message(LogLevel.Info, "Adding Plugin section.");
                Views.Add(new PluginSettingsView { DataObject = MPDisplaySettings.PluginSettings });
                MPDisplaySettings.PluginSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
                MPDisplaySettings.PluginSettings.ConnectionSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;

                _log.Message(LogLevel.Info, "Adding AddImage section.");
                Views.Add(new AddImageSettingsView { DataObject = AddImageSettings });
                AddImageSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
            }

            if (RegistrySettings.InstallType == MPDisplayInstallType.Plugin) return;
            _log.Message(LogLevel.Info, "Adding MPDisplay section.");
            Views.Add(new GUISettingsView { DataObject = MPDisplaySettings.GUISettings });
            _log.Message(LogLevel.Info, "Adding Skin section.");
            Views.Add(new SkinSettingsView { DataObject = MPDisplaySettings.GUISettings });
            MPDisplaySettings.GUISettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
            MPDisplaySettings.GUISettings.ConnectionSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
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
            SettingsManager.Save(MPDisplaySettings, RegistrySettings.MPDisplaySettingsFile);
            SettingsManager.Save(AddImageSettings, Path.Combine(RegistrySettings.ProgramDataPath, "AddImageSettings.xml"));
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

        /// <summary>
        /// Handles the Click event of the Button_SupportInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_SupportFile_Click(object sender, RoutedEventArgs e)
        {
            var path = FileNamePicker();
            if (string.IsNullOrEmpty(path)) return;

            var supportInfoFile = new SupportInfoFile();
            supportInfoFile.AddItem(new SupportInfoItem {FileName = "systeminfo.txt",Type = SupportInfoItemType.SystemInfo});
            supportInfoFile.AddItem(new SupportInfoItem { Folder = "MPD Config", ItemPath = RegistrySettings.ProgramDataPath, Type = SupportInfoItemType.DataFile });
            supportInfoFile.AddItem(new SupportInfoItem { Folder = "MPD Logs", ItemPath = RegistrySettings.ProgramDataPath + "Logs", Type = SupportInfoItemType.DataFile });
            supportInfoFile.AddItem(new SupportInfoItem { FileName = "assemblyinfo.txt", ItemPath = RegistrySettings.MPDisplayPath, Type = SupportInfoItemType.AssemblyInfo });

            supportInfoFile.AddItem(new SupportInfoItem { FileName = "assemblyinfo.txt", ItemPath = RegistrySettings.MPDPluginPath, Type = SupportInfoItemType.AssemblyInfo });

            supportInfoFile.AddItem(new SupportInfoItem { FileName = "mpdregistry.txt", ItemPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\MPDisplay", Type = SupportInfoItemType.RegistryKey});
            supportInfoFile.AddItem(new SupportInfoItem { FileName = "mpdregistry.txt", ItemPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\MPDisplay", Type = SupportInfoItemType.RegistryKey});
            supportInfoFile.CreateZipFile(path);
        }

        /// <summary>
        /// Launches a file name picker.
        /// </summary>
        private static string FileNamePicker()
        {
            var dlg = new SaveFileDialog
            {
                FileName = "MPD_SupportInfo.zip",
                DefaultExt = "zip",
                Filter = @"Zip files (*.zip)|*.zip",
                CheckFileExists = false,
                CheckPathExists = true
            };
            var result = dlg.ShowDialog();
            return result == System.Windows.Forms.DialogResult.OK ? dlg.FileName : string.Empty;
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
