using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Language")]
    [XmlInclude(typeof(XmlLanguageEntry))]
    public class XmlLanguage : INotifyPropertyChanged
    {
        private ObservableCollection<XmlLanguageEntry> _languageEntries = new ObservableCollection<XmlLanguageEntry>();

        [XmlArray]
        public ObservableCollection<XmlLanguageEntry> LanguageEntries
        {
            get { return _languageEntries; }
            set { _languageEntries = value; NotifyPropertyChanged("LanguageEntries"); }
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
