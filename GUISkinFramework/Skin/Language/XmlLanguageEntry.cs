using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "LanguageEntry")]
    [XmlInclude(typeof(XmlLanguageValue))]
    public class XmlLanguageEntry : INotifyPropertyChanged
    {
        private string _skinTag;
        private ObservableCollection<XmlLanguageValue> _values = new ObservableCollection<XmlLanguageValue>();

        [XmlAttribute(AttributeName="Tag")]
        public string SkinTag
        {
            get { return _skinTag; }
            set { _skinTag = value; NotifyPropertyChanged("SkinTag"); }
        }

        public ObservableCollection<XmlLanguageValue> Values
        {
            get { return _values; }
            set { _values = value; NotifyPropertyChanged("Values"); }
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

    [Serializable]
    [XmlType(TypeName = "LanguageValue")]
    public class XmlLanguageValue : INotifyPropertyChanged
    {
        private string _value;
        private string _language;

        [XmlAttribute(AttributeName = "Language")]
        public string Language
        {
            get { return _language; }
            set { _language = value; NotifyPropertyChanged("Language"); }
        }

        [XmlAttribute(AttributeName = "Value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; NotifyPropertyChanged("Value"); }
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
}
