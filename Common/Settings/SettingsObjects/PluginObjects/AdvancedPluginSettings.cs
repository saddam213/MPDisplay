using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Common.Settings
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
