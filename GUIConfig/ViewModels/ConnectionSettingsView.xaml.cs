﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Management;
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

        private const string _servicename = "MPDisplayServer";
        private bool _canEditConnectionName;
        private bool _canManageService;
        private string _testStatus;
        private string _serverStatus;
        private string _serverButtonText;
        private string _startmodeStatus;
        private string _startmodeStatusText;
        private string _startmodeButtonText;
        private ICommand _testButtonCommand;
        private ICommand _serverButtonCommand;
        private ICommand _startmodeButtonCommand;
        private bool _isTestingConnection = false;
        private bool _isStartModeKnown = false;
 
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionSettingsView"/> class.
        /// </summary>
        public ConnectionSettingsView()
        {
            InitializeComponent();

            _startmodeStatus = getServiceStartupType();
            StartModeStatus = LanguageHelper.GetLanguageValue(_startmodeStatus);

            TestButtonCommand = new RelayCommand(TestButtonExecute, CanTestButtonExecute);
            ServerButtonCommand = new RelayCommand(ServerButtonExecute, CanServerButtonExecute);
            StartModeButtonCommand = new RelayCommand(StartModeButtonExecute, CanStartModeButtonExecute);

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
        ///  Gets the flag if the MPDisplay service is installed locally
        /// </summary>
        public bool CanManageService
        {
            get { return _canManageService; }
            set { _canManageService = value; NotifyPropertyChanged("CanManageService"); }
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

        /// <summary>
        /// Gets the start mode button command.
        /// </summary>
        public ICommand StartModeButtonCommand
        {
            get { return _startmodeButtonCommand; }
            private set { _startmodeButtonCommand = value; NotifyPropertyChanged("StartModeButtonCommand"); }
        }

        /// <summary>
        /// Gets or sets the start mode status.
        /// </summary>
        public string StartModeStatus
        {
            get { return _startmodeStatusText; }
            set { _startmodeStatusText = value; NotifyPropertyChanged("StartModeStatus"); }
        }

        /// <summary>
        /// Gets or sets thestart mode button text.
        /// </summary>
        public string StartModeButtonText
        {
            get { return _startmodeButtonText; }
            set { _startmodeButtonText = value; NotifyPropertyChanged("StartModeButtonText"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Server button command to execute.
        /// </summary>
        public async void ServerButtonExecute()
        {
            bool isRunning = ServiceHelper.IsServiceRunning("MPDisplayServer");
            TestStatus = string.Empty;
            await Task.Factory.StartNew(() =>
            {
                if (!isRunning)
                {
                    ServiceHelper.StartService(_servicename, 5000);
                }
                else
                {
                    ServiceHelper.StopService(_servicename, 5000);
                }
            });
            ServerButtonText = ServiceHelper.GetServiceStatus(_servicename) != ServiceStatus.Running
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
            var status = ServiceHelper.GetServiceStatus(_servicename);
            ServerStatus = LanguageHelper.GetLanguageValue(status.ToString());
            ServerButtonText = status != ServiceStatus.Running
               ? LanguageHelper.GetLanguageValue("Start Server")
               : LanguageHelper.GetLanguageValue("Stop Server");
            return !_isTestingConnection && (status == ServiceStatus.Running || status == ServiceStatus.Stopped);
        }

        /// <summary>
        /// Start mode button command to execute.
        /// </summary>
        public async void StartModeButtonExecute()
        {
            await Task.Factory.StartNew(() =>
            {
                 var mode = "";
                 switch (_startmodeStatus)
                 {
                     case "Auto":
                         mode = "Manual";
                         break;
                     case "Disabled":
                     case "Manual":
                         mode = "Automatic";
                         break;
                 }
                 if (mode.Length > 0)
                 {
                     setServiceStartupType(mode);

                     _startmodeStatus = getServiceStartupType();

                 }
             });

            StartModeStatus = LanguageHelper.GetLanguageValue(_startmodeStatus);
            CommandManager.InvalidateRequerySuggested();
        }

 
        /// <summary>
        /// Determines whether this instance [can start mode button execute].
        /// </summary>
        /// <returns></returns>
        public bool CanStartModeButtonExecute()
        {

            if (StartModeStatus.Length > 0)
            {
                _isStartModeKnown = true;
            }
            else
            {
                _isStartModeKnown = false;
            }
            StartModeButtonText = _startmodeStatus == "Auto"
                ? "--> " + LanguageHelper.GetLanguageValue("Manual")
                : "--> " + LanguageHelper.GetLanguageValue("Auto");

            return !_isTestingConnection && _isStartModeKnown;
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
            return !_isTestingConnection;
        }

        #endregion

        #region WMI Management

        private  string  getServiceStartupType()
        {
            //construct the management path
            string path = "Win32_Service.Name='" + _servicename + "'";
            //construct the management object
            if( ServiceHelper.CheckIfServiceExists(_servicename) )
            {
                ManagementObject ManagementObj = new ManagementObject(path);
               return ManagementObj["StartMode"].ToString();
            }
            else
            {
                 return "Not installed";
             }
        }

        private void setServiceStartupType(string mode )
        {
                //construct the management path
                string path = "Win32_Service.Name='" + _servicename + "'";
                //construct the management object
                ManagementObject ManagementObj = new ManagementObject(path);
                //we will use the invokeMethod method of the ManagementObject class
                object[] parameters = new object[1];
                parameters[0] = mode;
                ManagementObj.InvokeMethod("ChangeStartMode", parameters);
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
