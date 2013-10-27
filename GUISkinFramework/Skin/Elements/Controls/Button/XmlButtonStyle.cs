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
    [XmlType(TypeName = "ButtonStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [XmlInclude(typeof(XmlListItemStyle))]
    [ExpandableObject]
    public class XmlButtonStyle : XmlControlStyle
    {
        #region Variables

        private string _borderThickness = "0,0,0,0";
        private string _borderCornerRadius = "0,0,0,0";
        private XmlBrush _glossBrush = null;//new XmlColorBrush();
        private XmlBrush _shadeBrush = null;//new XmlColorBrush();
        private XmlBrush _noFocusBrush = null;//new XmlColorBrush();
        private XmlBrush _noFocusBorderBrush = null;//new XmlColorBrush();
        private XmlBrush _focusBrush = null;//new XmlColorBrush();
        private XmlBrush _focusBorderBrush = null;//new XmlColorBrush();
        private XmlBrush _focusFontBrush;
        private XmlBrush _noFocusFontBrush; 
        private string _fontType = "Microsoft Sans Serif";
        private int _fontSize = 30;
        private string _fontWeight = "Normal";
        private HorizontalAlignment _labelHorizontalAlignment = HorizontalAlignment.Center;
        private VerticalAlignment _labelVerticalAlignment =  VerticalAlignment.Center;
        private string _imageMargin = "0,0,0,0";
        private int _imageCornerRadius = 0;
        private Stretch _imageStretch = Stretch.Uniform;
        private string _labelMargin = "0,0,0,0";
        private VerticalAlignment _imageVerticalAlignment;
        private HorizontalAlignment _imageHorizontalAlignment;
        private bool _enableLabel;
        private bool _enableImage;

        #endregion

        public XmlButtonStyle()
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
        [EditorCategory("Shading", 4)]
        public XmlBrush ShadeBrush
        {
            get { return _shadeBrush; }
            set { _shadeBrush = value; NotifyPropertyChanged("ShadeBrush"); }
        }

        [PropertyOrder(90)]
        [EditorCategory("Shading", 4)]
        public XmlBrush GlossBrush
        {
            get { return _glossBrush; }
            set { _glossBrush = value; NotifyPropertyChanged("GlossBrush"); }
        }

        #endregion

        #region Label

        [PropertyOrder(110)]
        [DefaultValue(true)]
        [EditorCategory("Label", 5)]
        public bool EnableLabel
        {
            get { return _enableLabel; }
            set { _enableLabel = value; NotifyPropertyChanged("EnableLabel"); }
        }
        

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

        #region Image

        

        [PropertyOrder(200)]
        [DefaultValue(false)]
        [EditorCategory("Image", 10)]
        public bool EnableImage
        {
            get { return _enableImage; }
            set { _enableImage = value; NotifyPropertyChanged("EnableImage"); }
        }

        [PropertyOrder(210)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Image", 10)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ImageMargin
        {
            get { return _imageMargin; }
            set { _imageMargin = value; NotifyPropertyChanged("ImageMargin"); }
        }

        [PropertyOrder(220)]
        [EditorCategory("Image", 10)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment ImageHorizontalAlignment
        {
            get { return _imageHorizontalAlignment; }
            set { _imageHorizontalAlignment = value; NotifyPropertyChanged("ImageHorizontalAlignment"); }
        }

        [PropertyOrder(230)]
        [EditorCategory("Image", 10)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment ImageVerticalAlignment
        {
            get { return _imageVerticalAlignment; }
            set { _imageVerticalAlignment = value; NotifyPropertyChanged("ImageVerticalAlignment"); }
        }

        [PropertyOrder(240)]
        [EditorCategory("Image", 10)]
        [DefaultValue(Stretch.Uniform)]
        public Stretch ImageStretch
        {
            get { return _imageStretch; }
            set { _imageStretch = value; NotifyPropertyChanged("ImageStretch"); }
        }

        [PropertyOrder(250)]
        [EditorCategory("Image", 10)]
        [DefaultValue(0)]
        public int ImageCornerRadius
        {
            get { return _imageCornerRadius; }
            set { _imageCornerRadius = value; NotifyPropertyChanged("ImageCornerRadius"); }
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
        }




       
    }


}
