using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPDisplay.Common.Settings
{
    [XmlInclude(typeof(GUISettings))]
    [XmlInclude(typeof(PluginSettings))]
    [XmlInclude(typeof(ConnectionSettings))]
    public class MPDisplaySettings : SettingsBase
    {
        private GUISettings _guiSettings = new GUISettings();
        private PluginSettings _pluginSettings = new PluginSettings();

        public GUISettings GUISettings
        {
            get { return _guiSettings; }
            set { _guiSettings = value; NotifyPropertyChanged("GUISettings"); }
        }

        public PluginSettings PluginSettings
        {
            get { return _pluginSettings; }
            set { _pluginSettings = value; NotifyPropertyChanged("PluginSettings"); }
        }
    }
}
