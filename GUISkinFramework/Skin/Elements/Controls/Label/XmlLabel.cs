using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Label")]
    public class XmlLabel : XmlControl
    {
        private string _labelText = "";
        private string _defaultLabelText = "";
        private string _labelNumberFormat = "";
        private int _lineHeight = 0;
        private TextAlignment _labelTextAlignment = TextAlignment.Left;
        private bool _isScrollingEnabled = true;
        private XmlLabelStyle _controlStyle;
        private bool _isScrollWrapEnabled = false;
        private int _scrollDelay = 3;
        private int _scrollSpeed = 2;
        private string _scrollSeperator = " | ";

        public XmlLabel()
        {
            this.SetDefaultValues();
        }

    
         [EditorCategory("Appearance", 3)]
         [DisplayName("Style")]
         [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        [XmlElement("LabelStyle")]
        public XmlLabelStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }
        
        [DefaultValue(""), Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        [PropertyOrder(10)]
        [EditorCategory("Label", 4)]
        public string LabelText
        {
            get { return _labelText; }
            set { _labelText = value; NotifyPropertyChanged("LabelText"); }
        }
      
        [DefaultValue(""), Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        [PropertyOrder(11)]
        [EditorCategory("Label", 4)]
        public string DefaultLabelText
        {
            get { return _defaultLabelText; }
            set { _defaultLabelText = value; NotifyPropertyChanged("DefaultLabelText"); }
        }

        [DefaultValue("")]
        [PropertyOrder(12)]
        [EditorCategory("Label", 4)]
        public string LabelNumberFormat
        {
            get { return _labelNumberFormat; }
            set { _labelNumberFormat = value; NotifyPropertyChanged("LabelNumberFormat"); }
        }

        [DefaultValue(TextAlignment.Left)]
        [PropertyOrder(20)]
        [EditorCategory("Label", 4)]
        public TextAlignment LabelTextAlignment
        {
            get { return _labelTextAlignment; }
            set { _labelTextAlignment = value; NotifyPropertyChanged("LabelTextAlignment"); }
        }

        [DefaultValue(0)]
        [PropertyOrder(30)]
        [EditorCategory("Label", 4)]
        public int LineHeight
        {
            get { return _lineHeight; }
            set { _lineHeight = value; NotifyPropertyChanged("LineHeight"); }
        }

        private bool _isVertical;

        [DefaultValue(false)]
        [PropertyOrder(40)]
        [EditorCategory("Label", 4)]
        public bool IsVertical
        {
            get { return _isVertical; }
            set { _isVertical = value;NotifyPropertyChanged("IsVertical"); }
        }


        [DefaultValue(true)]
        [PropertyOrder(10)]
        [EditorCategory("Scrolling", 5)]
        public bool IsScrollingEnabled
        {
            get { return _isScrollingEnabled; }
            set { _isScrollingEnabled = value; NotifyPropertyChanged("IsScrollingEnabled"); }
        }


        [DefaultValue(3)]
        [PropertyOrder(11)]
        [EditorCategory("Scrolling", 5)]
        public int ScrollDelay
        {
            get { return _scrollDelay; }
            set { _scrollDelay = value; NotifyPropertyChanged("ScrollDelay"); }
        }

        [DefaultValue(3)]
        [PropertyOrder(12)]
        [EditorCategory("Scrolling", 5)]
        public int ScrollSpeed
        {
            get { return _scrollSpeed; }
            set { _scrollSpeed = value; NotifyPropertyChanged("ScrollSpeed"); }
        }

        [DefaultValue(true)]
        [PropertyOrder(13)]
        [EditorCategory("Scrolling", 5)]
        public bool IsScrollWrapEnabled
        {
            get { return _isScrollWrapEnabled; }
            set { _isScrollWrapEnabled = value; NotifyPropertyChanged("IsScrollWrapEnabled"); }
        }

        [DefaultValue(" | ")]
        [PropertyOrder(14)]
        [EditorCategory("Scrolling", 5)]
        public string ScrollSeperator
        {
            get { return _scrollSeperator; }
            set { _scrollSeperator = value; NotifyPropertyChanged("ScrollSeperator"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlLabelStyle>(ControlStyle);
        }
    }
}
