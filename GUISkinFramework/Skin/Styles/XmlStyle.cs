using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [XmlInclude(typeof(XmlButtonStyle))]
    [XmlInclude(typeof(XmlLabelStyle))]
    [XmlInclude(typeof(XmlProgressBarStyle))]
    [XmlInclude(typeof(XmlGroupStyle))]
    [XmlInclude(typeof(XmlListStyle))]
    [XmlInclude(typeof(XmlGuideStyle))]
    [XmlInclude(typeof(XmlImageStyle))]
    [XmlInclude(typeof(XmlListItemStyle))]
    [XmlInclude(typeof(XmlEqualizerStyle))]
    [XmlInclude(typeof(XmlGuideChannelStyle))]
    [XmlInclude(typeof(XmlGuideProgramStyle))]
    public class XmlStyle : INotifyPropertyChanged
    {
        public XmlStyle()
        {
            this.SetDefaultValues();
        }

        private string _styleId;
        [XmlAttribute("StyleId")]
        [Browsable(false)]
        [DefaultValue("")]
        public string StyleId
        {
            get { return _styleId; }
            set { _styleId = value; NotifyPropertyChanged("StyleId"); }
        }

        [Browsable(false)]
        public virtual string StyleType
        {
            get { return GetType().Name.Replace("Xml", ""); }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
