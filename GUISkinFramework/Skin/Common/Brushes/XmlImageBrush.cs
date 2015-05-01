using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "ImageBrush")]
    public class XmlImageBrush : XmlBrush
    {
        private string _imageName;
        private Stretch _imageStretch;
     
        [DefaultValue("")]
        [XmlAttribute("Image")]
        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; NotifyPropertyChanged("ImageName"); }
        }

        [DefaultValue(Stretch.None)]
        [XmlAttribute("Stretch")]
        public Stretch ImageStretch
        {
            get { return _imageStretch; }
            set { _imageStretch = value; NotifyPropertyChanged("ImageStretch"); }
        }

    }
}
