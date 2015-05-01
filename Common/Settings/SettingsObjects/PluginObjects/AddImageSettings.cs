using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Common.Settings
{
    [XmlInclude(typeof(AddImagePropertySettings))]
    public class AddImageSettings : SettingsBase
    {
        private ObservableCollection<AddImagePropertySettings> _addImagePropertySettings = new ObservableCollection<AddImagePropertySettings>();
        public ObservableCollection<AddImagePropertySettings> AddImagePropertySettings
        {
            get { return _addImagePropertySettings; }
            set { _addImagePropertySettings = value; }
        }

        public void InitAddImagePropertySettings(string mpThumbsPath)
        {
            foreach (var prop in _addImagePropertySettings)
            {
                prop.FullPath = prop.Path.Replace("#MPThumbsPath#", mpThumbsPath);
                prop.PathExists = false;                // this will check if the path exists and set the property accordingly
                prop.MPProperties = null;               // this will init the list of MP properties referenced in this property
            }
        }
    }
}
