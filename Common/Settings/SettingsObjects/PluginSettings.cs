using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPDisplay.Common.Settings
{
    [XmlInclude(typeof(ConnectionSettings))]
    public class PluginSettings : SettingsBase
    {
        private ConnectionSettings _connectionSettings = new ConnectionSettings();
        public ConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { _connectionSettings = value; NotifyPropertyChanged("ConnectionSettings"); }
        }
    }
}
