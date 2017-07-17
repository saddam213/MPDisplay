using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

// this class maps the MP2 Guids to int IDs used by MPDisplay

namespace Common.Settings
{
    [XmlType("MP2PluginSettings")]
    public class MP2PluginSettings : SettingsBase
    {
        public int MaxWindowId { get; set; }
        public int MaxDialogId { get; set; }

        [XmlArrayItem("WindowIdMap")]
        public ObservableCollection<MP2IdMapping> WindowIdMap { get; set; } = new ObservableCollection<MP2IdMapping>();

        [XmlArrayItem("DialogIdMap")]
        public ObservableCollection<MP2IdMapping> DialogIdMap { get; set; } = new ObservableCollection<MP2IdMapping>();

        [XmlIgnore]
        public bool IsModified { get; private set; }

        public MP2IdMapping GetWindowMapping(string guid)
        {
            var item = WindowIdMap.FirstOrDefault( x => x.MP2Guid.Equals(guid));

            if (item != null) return item;

            MaxWindowId++;
            item = new MP2IdMapping {MP2Guid = guid, MPDid = MaxWindowId};
            WindowIdMap.Add(item);
            IsModified = true;
            return item;
        }

        public MP2IdMapping GetDialogMapping(string guid)
        {
            var item = DialogIdMap.FirstOrDefault(x => x.MP2Guid.Equals(guid));

            if (item != null) return item;

            MaxDialogId++;
            item = new MP2IdMapping { MP2Guid = guid, MPDid = MaxDialogId };
            DialogIdMap.Add(item);
            IsModified = true;
            return item;
        }
    }

}
