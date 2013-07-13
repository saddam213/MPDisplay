using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUISkinFramework.Controls
{
    public class XmlListItem : XmlControl
    {
        private string _labelText;
        private XmlListItemStyle _controlStyle;
        private string _image = "";

        public XmlListItemStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        public string Label
        {
            get { return _labelText; }
            set { _labelText = value; NotifyPropertyChanged("Label"); }
        }
    }
   
}
