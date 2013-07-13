using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Editor.PropertyEditors;
using GUISkinFramework.Styles;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Controls
{
    [Serializable]
    [XmlType(TypeName = "Equalizer")]
    public class XmlEqualizer : XmlControl
    {
        private int _bandCount;
        private int _minRangeValue;
        private int _medRangeValue;
        private int _bandSpacing;
        private int _bandBorderSize;
        private int _falloffSpeed;
        private string _fallOffHeight;
        private XmlEQStyle _eqStyle;
        private XmlLEDShape _ledShape;
        private int _ledHeight;
        private int _ledSpacing;

        [XmlIgnore]
        [Browsable(false)]
        public string DisplayType
        {
            get { return "Equalizer"; }
        }

        private XMLEqualizerStyle _controlStyle;

        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        [XmlElement("EqualizerStyle")]
        public XMLEqualizerStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        [PropertyOrder(10)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band Count")]
        [Description("The Number Of Bands In The EQ")]
        public int BandCount
        {
            get { return _bandCount; }
            set { _bandCount = value; NotifyPropertyChanged("BandCount"); }
        }

        [PropertyOrder(20)]
        [EditorCategory("Band", 10)]
        [DisplayName("Minimum RangeValue")]
        [Description("The % of the EQ Height to color with the 'Low Color'")]
        public int MinRangeValue
        {
            get { return _minRangeValue; }
            set { _minRangeValue = value; NotifyPropertyChanged("MinRangeValue"); }
        }

        [PropertyOrder(30)]
        [EditorCategory("Band", 10)]
        [DisplayName("Medium RangeValue")]
        [Description("The % of the EQ Height to color with the 'Middle Color'")]
        public int MedRangeValue
        {
            get { return _medRangeValue; }
            set { _medRangeValue = value; NotifyPropertyChanged("MedRangeValue"); }
        }

        [PropertyOrder(40)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band Spacing")]
        [Description("The Space Between Bands")]
        public int BandSpacing
        {
            get { return _bandSpacing; }
            set { _bandSpacing = value; NotifyPropertyChanged("BandSpacing"); }
        }

        [PropertyOrder(50)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band Border Size")]
        [Description("The Size Of The Band Border")]
        public int BandBorderSize
        {
            get { return _bandBorderSize; }
            set { _bandBorderSize = value; NotifyPropertyChanged("BandBorderSize"); }
        }

        [PropertyOrder(100)]
        [EditorCategory("FallOff", 11)]
        [DisplayName("FallOff Speed")]
        [Description("The Speed Of The FallOff Drop")]
        public int FalloffSpeed
        {
            get { return _falloffSpeed; }
            set { _falloffSpeed = value; NotifyPropertyChanged("FalloffSpeed"); }
        }

        [PropertyOrder(110)]
        [EditorCategory("FallOff", 11)]
        [DisplayName("FallOff Height")]
        [Description("The Height Of The FallOff")]
        public string FallOffHeight
        {
            get { return _fallOffHeight; }
            set { _fallOffHeight = value; NotifyPropertyChanged("FallOffHeight"); }
        }

        [DefaultValue(XmlEQStyle.SingleLED)]
        [PropertyOrder(140)]
        [EditorCategory("EQ Style", 3)]
        [DisplayName("Equalizer Style")]
        [Description("The style of the Equalizer")]
        public XmlEQStyle EQStyle
        {
            get { return _eqStyle; }
            set { _eqStyle = value; NotifyPropertyChanged("EQStyle"); }
        }


        [DefaultValue(XmlLEDShape.Rectangle)]
        [PropertyOrder(150)]
        [EditorCategory("L.E.D", 3)]
        [DisplayName("L.E.D Shape")]
        [Description("The shape of the L.E.D's")]
        public XmlLEDShape LEDShape
        {
            get { return _ledShape; }
            set { _ledShape = value; NotifyPropertyChanged("LEDShape"); }
        }

        [PropertyOrder(160)]
        [EditorCategory("L.E.D", 3)]
        [DisplayName("L.E.D Height")]
        [Description("The height Of L.E.D's In a Band")]
        public int LEDHeight
        {
            get { return _ledHeight; }
            set { _ledHeight = value; NotifyPropertyChanged("LEDHeight"); }
        }

        [PropertyOrder(170)]
        [EditorCategory("L.E.D", 3)]
        [DisplayName("L.E.D Spacing")]
        [Description("The Space Between LED's")]
        public int LEDSpacing
        {
            get { return _ledSpacing; }
            set { _ledSpacing = value; NotifyPropertyChanged("LEDSpacing"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XMLEqualizerStyle>(ControlStyle);
        }
    }
}
