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
using MPDisplay.Common.Settings;
using System.ComponentModel;
using MPDisplay.Common.Utils;
using Common.Helpers;
using GUIConfig.Settings.Language;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for ConnectionSettingsView.xaml
    /// </summary>
    public partial class ConnectionSettingsView : UserControl, INotifyPropertyChanged
    {
        public ConnectionSettingsView()
        {
            InitializeComponent();
            TestButtonCommand = new RelayCommand(TestButtonExecute, CanTestButtonExecute);
            ServerButtonCommand = new RelayCommand(ServerButtonExecute, CanServerButtonExecute);
        }

        public ConnectionSettings ConnectionSettings
        {
            get { return (ConnectionSettings)GetValue(ConnectionSettingsProperty); }
            set { SetValue(ConnectionSettingsProperty, value); NotifyPropertyChanged("ConnectionSettings"); }
        }

        // Using a DependencyProperty as the backing store for ConnectionSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionSettingsProperty =
            DependencyProperty.Register("ConnectionSettings", typeof(ConnectionSettings), typeof(ConnectionSettingsView));//, new PropertyMetadata(new ConnectionSettings));

        private bool _canEditConnectionName;
        private string _testStatus;
        private string _serverStatus;
        private string _serverButtonText;
        private ICommand _testButtonCommand;
        private ICommand _serverButtonCommand;
        private bool _isTestingConnection = false;

        public bool CanEditConnectionName
        {
            get { return _canEditConnectionName; }
            set { _canEditConnectionName = value; NotifyPropertyChanged("CanEditConnectionName"); }
        }

        public ICommand TestButtonCommand
        {
            get { return _testButtonCommand; }
            private set { _testButtonCommand = value; NotifyPropertyChanged("TestButtonCommand"); }
        }

        public string TestStatus
        {
            get { return _testStatus; }
            set { _testStatus = value; NotifyPropertyChanged("TestStatus"); }
        }

        public ICommand ServerButtonCommand
        {
            get { return _serverButtonCommand; }
            private set { _serverButtonCommand = value; NotifyPropertyChanged("ServerButtonCommand"); }
        }

        public string ServerStatus
        {
            get { return _serverStatus; }
            set { _serverStatus = value; NotifyPropertyChanged("ServerStatus"); }
        }

        public string ServerButtonText
        {
            get { return _serverButtonText; }
            set { _serverButtonText = value; NotifyPropertyChanged("ServerButtonText"); }
        }


        public async void ServerButtonExecute()
        {
            bool isRunning = ServiceHelper.IsServiceRunning("MPDisplayServer");
            TestStatus = string.Empty;
            await Task.Factory.StartNew(() =>
            {
                if (!isRunning)
                {
                    ServiceHelper.StartService("MPDisplayServer", 5000);
                }
                else
                {
                    ServiceHelper.StopService("MPDisplayServer", 5000);
                }
            });
            ServerButtonText = ServiceHelper.GetServiceStatus("MPDisplayServer") != ServiceStatus.Running
                ? LanguageHelper.GetLanguageValue("Start Server")
                : LanguageHelper.GetLanguageValue("Stop Server");
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanServerButtonExecute()
        {
            var status = ServiceHelper.GetServiceStatus("MPDisplayServer");
            ServerStatus = LanguageHelper.GetLanguageValue(status.ToString());
            ServerButtonText = status != ServiceStatus.Running
               ? LanguageHelper.GetLanguageValue("Start Server")
               : LanguageHelper.GetLanguageValue("Stop Server");
            return !_isTestingConnection && (status == ServiceStatus.Running || status == ServiceStatus.Stopped);
        }

        public async void TestButtonExecute()
        {
            _isTestingConnection = true;
            TestStatus = await ConnectionTester.TestConnection(ConnectionSettings)
                ? LanguageHelper.GetLanguageValue("Ok")
                : LanguageHelper.GetLanguageValue("Error");
            _isTestingConnection = false;
            CommandManager.InvalidateRequerySuggested();
        }

     

        public bool CanTestButtonExecute()
        {
            return !_isTestingConnection && ServiceHelper.IsServiceRunning("MPDisplayServer");
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    
        
    }
}
