using System.Xml.Serialization;

namespace Common.Settings
{
    [XmlInclude(typeof(ConnectionSettings))]
    public class PluginSettings : SettingsBase
    {
        private ConnectionSettings _connectionSettings = new ConnectionSettings();
        private bool _launchMPDisplayOnStart = true;
        private bool _restartMPDisplayOnStart = false;
        private bool _closeMPDisplayOnExit = true;
        private bool _isSystemInfoEnabled = true;
        private int _listBatchThreshold = 600;
        private int _listBatchSize = 200;
        private int _epgDays = 3;
        private int _userInteractionDelay = 10;

        public ConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { _connectionSettings = value; NotifyPropertyChanged("ConnectionSettings"); }
        } 

        public bool LaunchMPDisplayOnStart
        {
            get { return _launchMPDisplayOnStart; }
            set { _launchMPDisplayOnStart = value; NotifyPropertyChanged("LaunchMPDisplayOnStart"); }
        }

        public bool RestartMPDisplayOnStart
        {
            get { return _restartMPDisplayOnStart; }
            set { _restartMPDisplayOnStart = value; NotifyPropertyChanged("RestartMPDisplayOnStart"); }
        }

        public bool CloseMPDisplayOnExit
        {
            get { return _closeMPDisplayOnExit; }
            set { _closeMPDisplayOnExit = value; NotifyPropertyChanged("CloseMPDisplayOnExit"); }
        }


        public bool IsSystemInfoEnabled
        {
            get { return _isSystemInfoEnabled; }
            set { _isSystemInfoEnabled = value; NotifyPropertyChanged("IsSystemInfoEnabled"); }
        }

        public int ListBatchThreshold
        {
            get { return _listBatchThreshold; }
            set { _listBatchThreshold = value; NotifyPropertyChanged("ListBatchThreshold"); }
        }

        public int ListBatchSize
        {
            get { return _listBatchSize; }
            set { _listBatchSize = value; NotifyPropertyChanged("ListBatchSize"); }
        }

        public int EPGDays
        {
            get { return _epgDays; }
            set { _epgDays = value; NotifyPropertyChanged("EPGDays"); }
        }

        public int UserInteractionDelay
        {
            get { return _userInteractionDelay; }
            set { _userInteractionDelay = value; NotifyPropertyChanged("UserInteractionDelay"); }
        }
    }
}
