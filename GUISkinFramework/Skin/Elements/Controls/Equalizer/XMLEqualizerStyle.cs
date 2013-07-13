using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Common.Brushes;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;

namespace GUISkinFramework.Controls
{
    [Serializable]
    [XmlType(TypeName = "EqualizerStyle")]
    [ExpandableObject]
    public class XMLEqualizerStyle : XmlControlStyle
    {
     
        private string _bandBorderColor;
        private string _bandLowRangeColor;
        private string _bandMedRangeColor;
        private string _bandHighRangeColor;
    
        private string _fallOffColor;
        private string _fallOffBorderColor;
     
     

      

        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [DisplayName("Band Border Color ")]
        [Description("The Color Of The Bands Border")]
        public string BandBorderColor
        {
            get { return _bandBorderColor; }
            set { _bandBorderColor = value; NotifyPropertyChanged("BandBorderColor"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Appearance", 3)]
        [DisplayName("Band Minimum Color")]
        [Description("The Color Of The Band Lower Region")]
        public string BandLowRangeColor
        {
            get { return _bandLowRangeColor; }
            set { _bandLowRangeColor = value; NotifyPropertyChanged("BandLowRangeColor"); }
        }

        [PropertyOrder(80)]
        [EditorCategory("Appearance", 3)]
        [DisplayName("Band Medium Color")]
        [Description("The Color Of The Band Medium Region")]
        public string BandMedRangeColor
        {
            get { return _bandMedRangeColor; }
            set { _bandMedRangeColor = value; NotifyPropertyChanged("BandMedRangeColor"); }
        }

        [PropertyOrder(90)]
        [EditorCategory("Appearance", 3)]
        [DisplayName("Band Maximum Color")]
        [Description("The Color Of The Band High Region")]
        public string BandHighRangeColor
        {
            get { return _bandHighRangeColor; }
            set { _bandHighRangeColor = value; NotifyPropertyChanged("BandHighRangeColor"); }
        }

     

        [PropertyOrder(120)]
        [EditorCategory("Appearance", 3)]
        [DisplayName("FallOff Color")]
        [Description("Color Of The FallOff")]
        public string FallOffColor
        {
            get { return _fallOffColor; }
            set { _fallOffColor = value; NotifyPropertyChanged("FallOffColor"); }
        }

        [PropertyOrder(130)]
        [EditorCategory("Appearance", 3)]
        [DisplayName("FallOff BorderColor")]
        [Description("Color Of The FallOff Border")]
        public string FallOffBorderColor
        {
            get { return _fallOffBorderColor; }
            set { _fallOffBorderColor = value; NotifyPropertyChanged("FallOffBorderColor"); }
        }
     

    
     
    }

    public enum XmlLEDShape
    {
        Rectangle,
        RoundedRectangle,
        Circle
    }

    public enum XmlEQStyle
    {
        SingleLED,
        DoubleLED,
        SingleBar,
        DoubleBar
    }
}
