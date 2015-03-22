using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;
using GUISkinFramework.Skin;
using GUISkinFramework.Styles;

namespace GUISkinFramework.Controls
{
    [Serializable]
    [XmlType(TypeName = "ProgressBarStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [ExpandableObject]
    public class XmlProgressBarStyle : XmlControlStyle
    {
        private string _borderThickness = "0,0,0,0";
        private string _borderCornerRadius = "0,0,0,0";
        private XmlBrush _backgroundBrush;
        private XmlBrush _shadeBrush;
        private XmlBrush _glossBrush;
        private XmlBrush _borderBrush;
        private XmlBrush _barBorderBrush;
        private XmlBrush _barBackgroundBrush;
        private string _barBorderCornerRadius = "0,0,0,0";
        private string _barBorderThickness = "0,0,0,0";
        private string _barMargin = "0,0,0,0";

        private bool _enableProgressbar = true;

        private string _labelMovingFontType = "Microsoft Sans Serif";
        private string _labelMovingFontWeight = "Normal";
        private int _labelMovingFontSize = 30;
        private string _labelMovingMargin = "0,0,0,0";
        private VerticalAlignment _labelMovingVerticalAlignment = VerticalAlignment.Center;
        private string _labelFixedFontType = "Microsoft Sans Serif";
        private string _labelFixedFontWeight = "Normal";
        private int _labelFixedFontSize = 30;
        private string _labelFixedMargin = "0,0,0,0";
        private HorizontalAlignment _labelFixedHorizontalAlignment = HorizontalAlignment.Center;
        private VerticalAlignment _labelFixedVerticalAlignment = VerticalAlignment.Center;
        private XmlBrush _labelMovingFontBrush;
        private XmlBrush _labelFixedFontBrush;
        private bool _enableLabelMoving = false;
        private bool _enableLabelFixed = false;

        [DefaultValue("0,0,0,0")]
        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; NotifyPropertyChanged("BorderThickness"); }
        }

        [DefaultValue("0,0,0,0")]
        [PropertyOrder(61)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string CornerRadius
        {
            get { return _borderCornerRadius; }
            set { _borderCornerRadius = value; NotifyPropertyChanged("CornerRadius"); }
        }

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; NotifyPropertyChanged("BackgroundBrush"); }
        }

        [PropertyOrder(63)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BorderBrush
        {
            get { return _borderBrush; }
            set { _borderBrush = value; NotifyPropertyChanged("BorderBrush"); }
        }

        [PropertyOrder(66)]
        [EditorCategory("Shading", 4)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush ShadeBrush
        {
            get { return _shadeBrush; }
            set { _shadeBrush = value; NotifyPropertyChanged("ShadeBrush"); }
        }

        [PropertyOrder(67)]
        [EditorCategory("Shading", 4)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush GlossBrush
        {
            get { return _glossBrush; }
            set { _glossBrush = value; NotifyPropertyChanged("GlossBrush"); }
        }

        [DefaultValue("0,0,0,0")]
        [PropertyOrder(80)]
        [DisplayName("Bar Margin")]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BarMargin
        {
            get { return _barMargin; }
            set { _barMargin = value; NotifyPropertyChanged("BarMargin"); }
        }

        [DefaultValue("0,0,0,0")]
        [PropertyOrder(82)]
        [DisplayName("Bar BorderThickness")]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BarBorderThickness
        {
            get { return _barBorderThickness; }
            set { _barBorderThickness = value; NotifyPropertyChanged("BarBorderThickness"); }
        }

        [DefaultValue("0,0,0,0")]
        [PropertyOrder(84)]
        [DisplayName("Bar CornerRadius")]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BarCornerRadius
        {
            get { return _barBorderCornerRadius; }
            set { _barBorderCornerRadius = value; NotifyPropertyChanged("BarCornerRadius"); }
        }

        [PropertyOrder(86)]
        [DisplayName("Bar BorderBrush")]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BarBorderBrush
        {
            get { return _barBorderBrush; }
            set { _barBorderBrush = value; NotifyPropertyChanged("BarBorderBrush"); }
        }

        [PropertyOrder(88)]
        [DisplayName("Bar Background")]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BarBackgroundBrush
        {
            get { return _barBackgroundBrush; }
            set { _barBackgroundBrush = value; NotifyPropertyChanged("BarBackgroundBrush"); }
        }

        #region progress bar

        [PropertyOrder(91)]
        [DefaultValue(true)]
        [EditorCategory("ProgressBar", 5)]
        public bool EnableProgressbar
        {
            get { return _enableProgressbar; }
            set { _enableProgressbar = value; NotifyPropertyChanged("EnableProgressbar"); }
        }

        #endregion

        #region LabelMoving

        [PropertyOrder(110)]
        [DefaultValue(false)]
        [EditorCategory("LabelMoving", 6)]
        public bool EnableLabelMoving
        {
            get { return _enableLabelMoving; }
            set { _enableLabelMoving = value; NotifyPropertyChanged("EnableLabelMoving"); }
        }

        [DefaultValue("Microsoft Sans Serif")]
        [PropertyOrder(120)]
        [EditorCategory("LabelMoving", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string LabelMovingFontType
        {
            get { return _labelMovingFontType; }
            set { _labelMovingFontType = value; NotifyPropertyChanged("LabelMovingFontType"); }
        }

        [DefaultValue("Normal")]
        [PropertyOrder(130)]
        [EditorCategory("LabelMoving", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string LabelMovingFontWeight
        {
            get { return _labelMovingFontWeight; }
            set { _labelMovingFontWeight = value; NotifyPropertyChanged("LabelMovingFontWeight"); }
        }

        [DefaultValue(30)]
        [PropertyOrder(140)]
        [EditorCategory("LabelMoving", 6)]
        public int LabelMovingFontSize
        {
            get { return _labelMovingFontSize; }
            set { _labelMovingFontSize = value; NotifyPropertyChanged("LabelMovingFontSize"); }
        }

        [PropertyOrder(150)]
        [EditorCategory("LabelMoving", 6)]
        public XmlBrush LabelMovingFontBrush
        {
            get { return _labelMovingFontBrush; }
            set { _labelMovingFontBrush = value; NotifyPropertyChanged("LabelMovingFontBrush"); }
        }

         [PropertyOrder(170)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("LabelMoving", 6)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string LabelMovingMargin
        {
            get { return _labelMovingMargin; }
            set { _labelMovingMargin = value; NotifyPropertyChanged("LabelMovingMargin"); }
        }

        [PropertyOrder(180)]
        [EditorCategory("LabelMoving", 6)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment LabelMovingVerticalAlignment
        {
            get { return _labelMovingVerticalAlignment; }
            set { _labelMovingVerticalAlignment = value; NotifyPropertyChanged("LabelMovingVerticalAlignment"); }
        }

        #endregion

        #region LabelFixed

        [PropertyOrder(210)]
        [DefaultValue(false)]
        [EditorCategory("LabelFixed", 6)]
        public bool EnableLabelFixed
        {
            get { return _enableLabelFixed; }
            set { _enableLabelFixed = value; NotifyPropertyChanged("EnableLabelFixed"); }
        }

        [DefaultValue("Microsoft Sans Serif")]
        [PropertyOrder(220)]
        [EditorCategory("LabelFixed", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string LabelFixedFontType
        {
            get { return _labelFixedFontType; }
            set { _labelFixedFontType = value; NotifyPropertyChanged("LabelFixedFontType"); }
        }

        [DefaultValue("Normal")]
        [PropertyOrder(230)]
        [EditorCategory("LabelFixed", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string LabelFixedFontWeight
        {
            get { return _labelFixedFontWeight; }
            set { _labelFixedFontWeight = value; NotifyPropertyChanged("LabelFixedFontWeight"); }
        }

        [DefaultValue(30)]
        [PropertyOrder(240)]
        [EditorCategory("LabelFixed", 6)]
        public int LabelFixedFontSize
        {
            get { return _labelFixedFontSize; }
            set { _labelFixedFontSize = value; NotifyPropertyChanged("LabelFixedFontSize"); }
        }

        [PropertyOrder(250)]
        [EditorCategory("LabelFixed", 6)]
        public XmlBrush LabelFixedFontBrush
        {
            get { return _labelFixedFontBrush; }
            set { _labelFixedFontBrush = value; NotifyPropertyChanged("LabelFixedFontBrush"); }
        }

        [PropertyOrder(270)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("LabelFixed", 6)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string LabelFixedMargin
        {
            get { return _labelFixedMargin; }
            set { _labelFixedMargin = value; NotifyPropertyChanged("LabelFixedMargin"); }
        }

        [PropertyOrder(280)]
        [EditorCategory("LabelFixed", 6)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment LabelFixedHorizontalAlignment
        {
            get { return _labelFixedHorizontalAlignment; }
            set { _labelFixedHorizontalAlignment = value; NotifyPropertyChanged("LabelFixedHorizontalAlignment"); }
        }

        [PropertyOrder(290)]
        [EditorCategory("LabelFixed", 6)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment LabelFixedVerticalAlignment
        {
            get { return _labelFixedVerticalAlignment; }
            set { _labelFixedVerticalAlignment = value; NotifyPropertyChanged("LabelFixedVerticalAlignment"); }
        }

        #endregion

        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            GlossBrush = style.GetStyle<XmlBrush>(GlossBrush);
            ShadeBrush = style.GetStyle<XmlBrush>(ShadeBrush);
            BorderBrush = style.GetStyle<XmlBrush>(BorderBrush);
            BackgroundBrush = style.GetStyle<XmlBrush>(BackgroundBrush);
            LabelMovingFontBrush = style.GetStyle<XmlBrush>(LabelMovingFontBrush);
            LabelFixedFontBrush = style.GetStyle<XmlBrush>(LabelFixedFontBrush);
        }
    }

 
}
