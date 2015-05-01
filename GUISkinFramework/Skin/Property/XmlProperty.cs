using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    public class XmlProperty : INotifyPropertyChanged
    {
        private string _skinTag;
        private XmlPropertyType _propertyType;
        private string _designerValue;
        private ObservableCollection<XmlMediaPortalTag> _mediaPortalTags = new ObservableCollection<XmlMediaPortalTag>();

        public string SkinTag
        {
            get { return _skinTag; }
            set { _skinTag = value; NotifyPropertyChanged("SkinTag"); }
        }

        public XmlPropertyType PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; NotifyPropertyChanged("PropertyType"); }
        }

        public string DesignerValue
        {
            get { return _designerValue; }
            set { _designerValue = value; NotifyPropertyChanged("DesignerValue"); }
        }
     
        public ObservableCollection<XmlMediaPortalTag> MediaPortalTags
        {
            get { return _mediaPortalTags; }
            set { _mediaPortalTags = value; NotifyPropertyChanged("MediaPortalTags"); }
        }

        public bool IsInternal { get; set; }
      
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public class XmlMediaPortalTag : INotifyPropertyChanged
    {
        private string _tag;
     
        [XmlAttribute("Tag")]
        public string Tag
        {
            get { return _tag; }
            set { _tag = value; NotifyPropertyChanged("Tag"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public enum XmlPropertyType
    {
        Label = 0,
        Number,
        Image
    }
}
