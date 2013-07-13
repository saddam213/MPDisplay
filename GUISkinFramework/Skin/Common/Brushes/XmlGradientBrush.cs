using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace GUISkinFramework.Common.Brushes
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
