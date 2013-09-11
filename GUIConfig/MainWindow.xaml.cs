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
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace GUIConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Log Log = LoggingManager.GetLog(typeof(MainWindow));
        private ObservableCollection<ViewModelBase> _views = new ObservableCollection<ViewModelBase>();
        private MPDisplaySettings _mpdisplaySettings;

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

         

            MPDisplaySettings = SettingsManager.Load<MPDisplaySettings>(RegistrySettings.MPDisplaySettingsFile) ?? new MPDisplaySettings();
            LoadViews();
        }

        public ObservableCollection<ViewModelBase> Views
        {
            get { return _views; }
            set { _views = value; }
        }
      
        public MPDisplaySettings MPDisplaySettings
        {
            get { return _mpdisplaySettings; }
            set { _mpdisplaySettings = value; NotifyPropertyChanged(); }
        }

        private void LoadViews()
        {

         

            if (RegistrySettings.InstallType != MPDisplayInstallType.GUI)
            {
                Views.Add(new PluginSettingsView { DataObject = MPDisplaySettings.PluginSettings });
                MPDisplaySettings.PluginSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
                MPDisplaySettings.PluginSettings.ConnectionSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
            }

            if (RegistrySettings.InstallType != MPDisplayInstallType.Plugin)
            {
                Views.Add(new GUISettingsView { DataObject = MPDisplaySettings.GUISettings });
                Views.Add(new SkinSettingsView { DataObject = MPDisplaySettings.GUISettings });
                MPDisplaySettings.GUISettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
                MPDisplaySettings.GUISettings.ConnectionSettings.PropertyChanged += MPDisplaySettings_PropertyChanged;
            }
        }

        private bool _hasChanges = false;

        void MPDisplaySettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _hasChanges = true;
        }

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            LoggingManager.Destroy();
        }

        private void SaveChanges()
        {
            foreach (var view in Views)
            {
                view.SaveChanges();
            }
            SettingsManager.Save<MPDisplaySettings>(MPDisplaySettings, RegistrySettings.MPDisplaySettingsFile);
            _hasChanges = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            Close();
        }


        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _hasChanges = false;
            Close();
        }
    }
}
