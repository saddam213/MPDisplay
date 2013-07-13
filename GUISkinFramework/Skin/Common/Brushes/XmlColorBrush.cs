using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GUISkinFramework.Common.Brushes
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
