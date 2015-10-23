using System;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "ListItem")]
    public class XmlListItem : XmlButton
    {
        private string _label2Text = "";
        private string _label3Text = "";
       
        public string Label2Text
        {
            get { return _label2Text; }
            set { _label2Text = value; NotifyPropertyChanged("Label2Text"); }
        }

     
        public string Label3Text
        {
            get { return _label3Text; }
            set { _label3Text = value; NotifyPropertyChanged("Label3Text"); }
        }
    }
}
