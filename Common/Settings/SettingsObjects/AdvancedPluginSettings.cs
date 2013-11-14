using Common.Settings.SettingsObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPDisplay.Common.Settings
{
    [XmlInclude(typeof(SupportedPluginSettings))]
    [XmlInclude(typeof(TvSeriesPluginSettings))]
    public class AdvancedPluginSettings : SettingsBase
    {
        private ObservableCollection<SupportedPluginSettings> _supportedPlugins = new ObservableCollection<SupportedPluginSettings>();
        public ObservableCollection<SupportedPluginSettings> SupportedPlugins
        {
            get { return _supportedPlugins; }
            set { _supportedPlugins = value; NotifyPropertyChanged("SupportedPlugins"); }
        }
    }
}
