using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "GuideProgram")]
    public class XmlGuideProgramStyle : XmlControlStyle
    {
        #region Variables

        private string _borderThickness = "0,0,0,0";
        private string _borderCornerRadius = "0,0,0,0";
        private XmlBrush _glossBrush = null;
        private XmlBrush _shadeBrush = null;
        private XmlBrush _noFocusBrush = null;
        private XmlBrush _noFocusBorderBrush = null;
        private XmlBrush _focusBrush = null;
        private XmlBrush _focusBorderBrush = null;
        private XmlBrush _focusFontBrush;
        private XmlBrush _noFocusFontBrush;
        private string _fontType = "Microsoft Sans Serif";
        private int _fontSize = 30;
        private string _fontWeight = "Normal";
        private HorizontalAlignment _labelHorizontalAlignment = HorizontalAlignment.Center;
        private VerticalAlignment _labelVerticalAlignment = VerticalAlignment.Center;
        private string _labelMargin = "0,0,0,0";
        private XmlBrush _recordingBrush;
        private XmlBrush _recordingBorderBrush;
        private XmlBrush _recordingFontBrush;
        private XmlBrush _onNowFontBrush;
        private XmlBrush _onNowBorderBrush;
        private XmlBrush _onNowBrush;

        #endregion

        public XmlGuideProgramStyle()
        {
            this.SetDefaultValues();
        }

        #region Base

        [PropertyOrder(20)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; NotifyPropertyChanged("BorderThickness"); }
        }

        [PropertyOrder(30)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string CornerRadius
        {
            get { return _borderCornerRadius; }
            set { _borderCornerRadius = value; NotifyPropertyChanged("CornerRadius"); }
        }

        [PropertyOrder(40)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush NoFocusBrush
        {
            get { return _noFocusBrush; }
            set { _noFocusBrush = value; NotifyPropertyChanged("NoFocusBrush"); }
        }

        [PropertyOrder(50)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush NoFocusBorderBrush
        {
            get { return _noFocusBorderBrush; }
            set { _noFocusBorderBrush = value; NotifyPropertyChanged("NoFocusBorderBrush"); }
        }

        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush FocusBrush
        {
            get { return _focusBrush; }
            set { _focusBrush = value; NotifyPropertyChanged("FocusBrush"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush FocusBorderBrush
        {
            get { return _focusBorderBrush; }
            set { _focusBorderBrush = value; NotifyPropertyChanged("FocusBorderBrush"); }
        }

        [PropertyOrder(80)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush RecordingBrush
        {
            get { return _recordingBrush; }
            set { _recordingBrush = value; NotifyPropertyChanged("RecordingBrush"); }
        }

        [PropertyOrder(90)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush RecordingBorderBrush
        {
            get { return _recordingBorderBrush; }
            set { _recordingBorderBrush = value; NotifyPropertyChanged("RecordingBorderBrush"); }
        }

        [PropertyOrder(100)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush OnNowBrush
        {
            get { return _onNowBrush; }
            set { _onNowBrush = value; NotifyPropertyChanged("OnNowBrush"); }
        }

        [PropertyOrder(110)]
        [EditorCategory("Appearance", 3)]
        public XmlBrush OnNowBorderBrush
        {
            get { return _onNowBorderBrush; }
            set { _onNowBorderBrush = value; NotifyPropertyChanged("OnNowBorderBrush"); }
        }

        [PropertyOrder(10)]
        [EditorCategory("Shading", 4)]
        public XmlBrush ShadeBrush
        {
            get { return _shadeBrush; }
            set { _shadeBrush = value; NotifyPropertyChanged("ShadeBrush"); }
        }

        [PropertyOrder(20)]
        [EditorCategory("Shading", 4)]
        public XmlBrush GlossBrush
        {
            get { return _glossBrush; }
            set { _glossBrush = value; NotifyPropertyChanged("GlossBrush"); }
        }

        #endregion

        #region Label

        [DefaultValue("Microsoft Sans Serif")]
        [PropertyOrder(120)]
        [EditorCategory("Label", 5)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string FontType
        {
            get { return _fontType; }
            set { _fontType = value; NotifyPropertyChanged("FontType"); }
        }

        [DefaultValue("Normal")]
        [PropertyOrder(130)]
        [EditorCategory("Label", 5)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string FontWeight
        {
            get { return _fontWeight; }
            set { _fontWeight = value; NotifyPropertyChanged("FontWeight"); }
        }

        [DefaultValue(30)]
        [PropertyOrder(140)]
        [EditorCategory("Label", 5)]
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; NotifyPropertyChanged("FontSize"); }
        }

        [PropertyOrder(150)]
        [EditorCategory("Label", 5)]
        public XmlBrush FocusFontBrush
        {
            get { return _focusFontBrush; }
            set { _focusFontBrush = value; NotifyPropertyChanged("FocusFontBrush"); }
        }

        [PropertyOrder(160)]
        [EditorCategory("Label", 5)]
        public XmlBrush NoFocusFontBrush
        {
            get { return _noFocusFontBrush; }
            set { _noFocusFontBrush = value; NotifyPropertyChanged("NoFocusFontBrush"); }
        }

        [PropertyOrder(162)]
        [EditorCategory("Label", 5)]
        public XmlBrush RecordingFontBrush
        {
            get { return _recordingFontBrush; }
            set { _recordingFontBrush = value; NotifyPropertyChanged("RecordingFontBrush"); }
        }

        [PropertyOrder(163)]
        [EditorCategory("Label", 5)]
        public XmlBrush OnNowFontBrush
        {
            get { return _onNowFontBrush; }
            set { _onNowFontBrush = value; NotifyPropertyChanged("OnNowFontBrush"); }
        }

        [PropertyOrder(170)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Label", 5)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string LabelMargin
        {
            get { return _labelMargin; }
            set { _labelMargin = value; NotifyPropertyChanged("LabelMargin"); }
        }

        [PropertyOrder(180)]
        [EditorCategory("Label", 5)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return _labelHorizontalAlignment; }
            set { _labelHorizontalAlignment = value; NotifyPropertyChanged("LabelHorizontalAlignment"); }
        }

        [PropertyOrder(190)]
        [EditorCategory("Label", 5)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return _labelVerticalAlignment; }
            set { _labelVerticalAlignment = value; NotifyPropertyChanged("LabelVerticalAlignment"); }
        }

        #endregion

        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            GlossBrush = style.GetStyle<XmlBrush>(GlossBrush);
            ShadeBrush = style.GetStyle<XmlBrush>(ShadeBrush);
            NoFocusBrush = style.GetStyle<XmlBrush>(NoFocusBrush);
            NoFocusBorderBrush = style.GetStyle<XmlBrush>(NoFocusBorderBrush);
            FocusBrush = style.GetStyle<XmlBrush>(FocusBrush);
            FocusBorderBrush = style.GetStyle<XmlBrush>(FocusBorderBrush);
            FocusFontBrush = style.GetStyle<XmlBrush>(FocusFontBrush);
            NoFocusFontBrush = style.GetStyle<XmlBrush>(NoFocusFontBrush);
            RecordingBrush = style.GetStyle<XmlBrush>(RecordingBrush);
            RecordingBorderBrush = style.GetStyle<XmlBrush>(RecordingBorderBrush);
            RecordingFontBrush = style.GetStyle<XmlBrush>(RecordingFontBrush);
            OnNowBrush = style.GetStyle<XmlBrush>(OnNowBrush);
            OnNowBorderBrush = style.GetStyle<XmlBrush>(OnNowBorderBrush);
            OnNowFontBrush = style.GetStyle<XmlBrush>(OnNowFontBrush);
        }

      

     

       
    }
}

