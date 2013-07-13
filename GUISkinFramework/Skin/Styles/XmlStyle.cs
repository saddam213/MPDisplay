using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Styles
{
    [XmlInclude(typeof(XmlButtonStyle))]
    [XmlInclude(typeof(XmlLabelStyle))]
    [XmlInclude(typeof(XmlProgressBarStyle))]
    [XmlInclude(typeof(XmlGroupStyle))]
    [XmlInclude(typeof(XmlListStyle))]
    [XmlInclude(typeof(XmlGuideStyle))]
    [XmlInclude(typeof(XmlImageStyle))]
    [XmlInclude(typeof(XmlListItemStyle))]
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
            get { return this.GetType().Name.Replace("Xml", ""); }
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
