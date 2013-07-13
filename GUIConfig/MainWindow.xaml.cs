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
            Views.Add(new BasicSettingsView { DataObject = MPDisplaySettings });
           // Views.Add(new SkinSettingsView { DataObject = SkinSettings });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Views.Any(v => v.HasPendingChanges))
            {
                if (MessageBox.Show("save changes?", "Save?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //SettingsManager.Save<MPDisplaySettings>(MPDisplaySettings, RegistrySettings.MPDisplaySettingsFile);
                }
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            LoggingManager.Destroy();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
