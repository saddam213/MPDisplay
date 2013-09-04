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
    }
}
