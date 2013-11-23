using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Settings
{
    public class ConnectionSettings : SettingsBase
    {
        private int _port = 44444;
        private string _ipAddress = "localhost";
        private string _connectionName = "MPDisplay";
        private int _resumeDelay = 5000;

        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; NotifyPropertyChanged("IpAddress"); }
        }

        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; NotifyPropertyChanged("ConnectionName"); }
        }

        public int ResumeDelay
        {
            get { return _resumeDelay; }
            set { _resumeDelay = value; NotifyPropertyChanged("ResumeDelay"); }
        }
    }
}
