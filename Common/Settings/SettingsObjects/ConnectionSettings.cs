﻿namespace Common.Settings
{
    public class ConnectionSettings : SettingsBase
    {
        private int _port = 44444;
        private string _ipAddress = "localhost";
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

        public int ResumeDelay
        {
            get { return _resumeDelay; }
            set { _resumeDelay = value; NotifyPropertyChanged("ResumeDelay"); }
        }
    }
}
