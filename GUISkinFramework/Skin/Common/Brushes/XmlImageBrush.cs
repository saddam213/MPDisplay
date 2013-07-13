using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GUISkinFramework.Common.Brushes
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
