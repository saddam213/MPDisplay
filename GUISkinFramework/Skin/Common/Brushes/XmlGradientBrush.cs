using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "GradientBrush")]
    public class XmlGradientBrush : XmlBrush
    {
       // private string _startPoint = new Point(0.5,0).ToXmlString();
        private XmlGradientAngle _angle = XmlGradientAngle.Vertical;
        private ObservableCollection<XmlGradientStop> _gradientStops = new ObservableCollection<XmlGradientStop>();

    ////    [DefaultValue("0.5,0")]
    //    public string StartPoint
    //    {
    //        get { return _startPoint; }
    //        set { _startPoint = value; NotifyPropertyChanged("StartPoint"); }
    //    }

     //   [DefaultValue("0.5,1")]
        public XmlGradientAngle Angle
        {
            get { return _angle; }
            set { _angle = value; NotifyPropertyChanged("Angle"); }
        }

        [DefaultValue(null)]
        [XmlArray(ElementName = "GradientStops")]
        public ObservableCollection<XmlGradientStop> GradientStops
        {
            get { return _gradientStops; }
            set { _gradientStops = value; NotifyPropertyChanged("GradientStops"); }
        }
    }

    public enum XmlGradientAngle
    {
        Vertical,
        Horizontal
    }
}
