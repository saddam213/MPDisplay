using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Common.Settings.SettingsObjects;

namespace MPDisplay.Common.Settings
{
    [XmlInclude(typeof(ConnectionSettings))]
    [XmlInclude(typeof(PlayerPlugin))]
    public class PluginSettings : SettingsBase
    {
        private ConnectionSettings _connectionSettings = new ConnectionSettings();
        private ObservableCollection<PlayerPlugin> _playerPlugins;
        private bool _launchMPDisplayOnStart = true;
        private bool _restartMPDisplayOnStart = false;
        private bool _closeMPDisplayOnExit = true;

        public ConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { _connectionSettings = value; NotifyPropertyChanged("ConnectionSettings"); }
        }

        public ObservableCollection<PlayerPlugin> PlayerPlugins
        {
            get { return _playerPlugins; }
            set { _playerPlugins = value; NotifyPropertyChanged("PlayerPlugins"); }
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
    }
}
