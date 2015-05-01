using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "ColorBrush")]
    public class XmlColorBrush : XmlBrush
    {
        private string _color = "Transparent";

        [DefaultValue("Transparent")]
        [XmlAttribute("Color")]
        public string Color
        {
            get { return _color; }
            set { _color = value; NotifyPropertyChanged("Color"); }
        }
    }
}
