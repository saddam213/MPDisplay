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
        private XmlEqualizerStyle _controlStyle;
        private XmlEQStyle _eqStyle = XmlEQStyle.SingleBar;
        private int _lowRangeValue = 100;
        private int _medRangeValue = 200;
        private int _bandCount = 20;
        private int _bandSpacing = 1;
        private int _bandBorderSize = 1;
        private int _bandCornerRadius = 2;
        private int _falloffSpeed = 10;
        private int _fallOffHeight = 3;
        private bool _showDummyData = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEqualizer"/> class.
        /// </summary>
        public XmlEqualizer()
        {
            this.SetDefaultValues();
        }
    
        [XmlIgnore]
        [Browsable(false)]
        public string DisplayType
        {
            get { return "Equalizer"; }
        }

        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        [XmlElement("EqualizerStyle")]
        public XmlEqualizerStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        [DefaultValue(XmlEQStyle.SingleBar)]
        [PropertyOrder(200)]
        [EditorCategory("Equalizer", 8)]
        [DisplayName("EQStyle")]
        public XmlEQStyle EQStyle
        {
            get { return _eqStyle; }
            set { _eqStyle = value; NotifyPropertyChanged("EQStyle"); }
        }

        [DefaultValue(100)]
        [PropertyOrder(210)]
        [EditorCategory("Equalizer", 8)]
        [DisplayName("LowRange Value")]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(0, 255)]
        public int LowRangeValue
        {
            get { return _lowRangeValue; }
            set { _lowRangeValue = value; NotifyPropertyChanged("LowRangeValue"); }
        }

        [DefaultValue(200)]
        [PropertyOrder(211)]
        [EditorCategory("Equalizer", 8)]
        [DisplayName("MedRange Value")]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(0, 255)]
        public int MedRangeValue
        {
            get { return _medRangeValue; }
            set { _medRangeValue = value; NotifyPropertyChanged("MedRangeValue"); }
        }

        [DefaultValue(20)]
        [PropertyOrder(300)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band Count")]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(0, 100)]
        public int BandCount
        {
            get { return _bandCount; }
            set { _bandCount = value; NotifyPropertyChanged("BandCount"); }
        }

        [DefaultValue(2)]
        [PropertyOrder(301)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band Spacing")]
        public int BandSpacing
        {
            get { return _bandSpacing; }
            set { _bandSpacing = value; NotifyPropertyChanged("BandSpacing"); }
        }

        [DefaultValue(1)]
        [PropertyOrder(302)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band BorderSize")]
        public int BandBorderSize
        {
            get { return _bandBorderSize; }
            set { _bandBorderSize = value; NotifyPropertyChanged("BandBorderSize"); }
        }

        [DefaultValue(2)]
        [PropertyOrder(303)]
        [EditorCategory("Band", 10)]
        [DisplayName("Band CornerRadius")]
        public int BandCornerRadius
        {
            get { return _bandCornerRadius; }
            set { _bandCornerRadius = value; NotifyPropertyChanged("BandCornerRadius"); }
        }

        [DefaultValue(8)]
        [PropertyOrder(400)]
        [EditorCategory("FallOff", 12)]
        [DisplayName("Falloff Speed")]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(0, 20)]
        public int FalloffSpeed
        {
            get { return _falloffSpeed; }
            set { _falloffSpeed = value; NotifyPropertyChanged("FalloffSpeed"); }
        }

        [DefaultValue(3)]
        [PropertyOrder(401)]
        [EditorCategory("FallOff", 12)]
        [DisplayName("FallOff Height")]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(0, 30)]
        public int FallOffHeight
        {
            get { return _fallOffHeight; }
            set { _fallOffHeight = value; NotifyPropertyChanged("FallOffHeight"); }
        }

        [XmlIgnore]
        [DisplayName("Show DummyData")]
        public bool ShowDummyData
        {
            get { return _showDummyData; }
            set { _showDummyData = value; NotifyPropertyChanged("ShowDummyData"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlEqualizerStyle>(ControlStyle);
        }
    }

    public enum XmlEQStyle
    {
        SingleRectangle = 0,
        SingleRoundedRectangle = 1,
        SingleCircle = 2,
        SingleBar = 3,
        DoubleRectangle = 4,
        DoubleRoundedRectangle = 5,
        DoubleCircle = 6,
        DoubleBar = 7
    }
}
