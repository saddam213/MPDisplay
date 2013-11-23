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
using System.ComponentModel;
using MPDisplay.Common.Utils;
using Common.Helpers;
using GUIConfig.Settings.Language;
using Common.Settings;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for ConnectionSettingsView.xaml
    /// </summary>
    public partial class ConnectionSettingsView : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private bool _canEditConnectionName;
        private string _testStatus;
        private string _serverStatus;
        private string _serverButtonText;
        private ICommand _testButtonCommand;
        private ICommand _serverButtonCommand;
        private bool _isTestingConnection = false; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionSettingsView"/> class.
        /// </summary>
        public ConnectionSettingsView()
        {
            InitializeComponent();
            TestButtonCommand = new RelayCommand(TestButtonExecute, CanTestButtonExecute);
            ServerButtonCommand = new RelayCommand(ServerButtonExecute, CanServerButtonExecute);
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection settings.
        /// </summary>
        public ConnectionSettings ConnectionSettings
        {
            get { return (ConnectionSettings)GetValue(ConnectionSettingsProperty); }
            set { SetValue(ConnectionSettingsProperty, value); NotifyPropertyChanged("ConnectionSettings"); }
        }

        // Using a DependencyProperty as the backing store for ConnectionSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionSettingsProperty =
            DependencyProperty.Register("ConnectionSettings", typeof(ConnectionSettings), typeof(ConnectionSettingsView));//, new PropertyMetadata(new ConnectionSettings));

        /// <summary>
        /// Gets or sets a value indicating whether can edit connection name.
        /// </summary>
        /// <value>
        /// <c>true</c> if [can edit connection name]; otherwise, <c>false</c>.
        /// </value>
        public bool CanEditConnectionName
        {
            get { return _canEditConnectionName; }
            set { _canEditConnectionName = value; NotifyPropertyChanged("CanEditConnectionName"); }
        }

        /// <summary>
        /// Gets the test button command.
        /// </summary>
        public ICommand TestButtonCommand
        {
            get { return _testButtonCommand; }
            private set { _testButtonCommand = value; NotifyPropertyChanged("TestButtonCommand"); }
        }

        /// <summary>
        /// Gets or sets the test status.
        /// </summary>
        public string TestStatus
        {
            get { return _testStatus; }
            set { _testStatus = value; NotifyPropertyChanged("TestStatus"); }
        }

        /// <summary>
        /// Gets the server button command.
        /// </summary>
        public ICommand ServerButtonCommand
        {
            get { return _serverButtonCommand; }
            private set { _serverButtonCommand = value; NotifyPropertyChanged("ServerButtonCommand"); }
        }

        /// <summary>
        /// Gets or sets the server status.
        /// </summary>
        public string ServerStatus
        {
            get { return _serverStatus; }
            set { _serverStatus = value; NotifyPropertyChanged("ServerStatus"); }
        }

        /// <summary>
        /// Gets or sets the server button text.
        /// </summary>
        public string ServerButtonText
        {
            get { return _serverButtonText; }
            set { _serverButtonText = value; NotifyPropertyChanged("ServerButtonText"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Servers button command to execute.
        /// </summary>
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

        /// <summary>
        /// Determines whether this instance [can server button execute].
        /// </summary>
        /// <returns></returns>
        public bool CanServerButtonExecute()
        {
            var status = ServiceHelper.GetServiceStatus("MPDisplayServer");
            ServerStatus = LanguageHelper.GetLanguageValue(status.ToString());
            ServerButtonText = status != ServiceStatus.Running
               ? LanguageHelper.GetLanguageValue("Start Server")
               : LanguageHelper.GetLanguageValue("Stop Server");
            return !_isTestingConnection && (status == ServiceStatus.Running || status == ServiceStatus.Stopped);
        }

        /// <summary>
        /// Tests button command to execute.
        /// </summary>
        public async void TestButtonExecute()
        {
            _isTestingConnection = true;
            TestStatus = await ConnectionTester.TestConnection(ConnectionSettings)
                ? LanguageHelper.GetLanguageValue("Ok")
                : LanguageHelper.GetLanguageValue("Error");
            _isTestingConnection = false;
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Determines whether this instance [can test button execute].
        /// </summary>
        /// <returns></returns>
        public bool CanTestButtonExecute()
        {
            return !_isTestingConnection && ServiceHelper.IsServiceRunning("MPDisplayServer");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="property">The property.</param>
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
