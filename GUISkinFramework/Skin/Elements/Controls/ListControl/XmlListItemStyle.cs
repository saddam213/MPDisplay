using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "ListItemStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [ExpandableObject]
    public class XmlListItemStyle : XmlButtonStyle
    {
        #region Fields

        private int _height;
        private int _width;
        private VerticalAlignment _verticalAlignment;
        private HorizontalAlignment _horizontalAlignment;
        private string _itemMargin;
        private int _selectedZoomX = 100;
        private int _selectedZoomY = 100;
        private int _selectedZoomDuration = 250;
        private string _label2FontType = "Microsoft Sans Serif";
        private string _label2FontWeight = "Normal";
        private int _label2FontSize = 16;
        private string _label2Margin = "0,0,0,0";
        private HorizontalAlignment _label2HorizontalAlignment;
        private VerticalAlignment _label2VerticalAlignment;
        private string _label3FontType = "Microsoft Sans Serif";
        private string _label3FontWeight = "Normal";
        private int _label3FontSize = 16;
        private string _label3Margin = "0,0,0,0";
        private HorizontalAlignment _label3HorizontalAlignment;
        private VerticalAlignment _label3VerticalAlignment;
        private XmlBrush _label2FocusFontBrush;
        private XmlBrush _label2NoFocusFontBrush;
        private XmlBrush _label3NoFocusFontBrush;
        private XmlBrush _label3FocusFontBrush;
        private bool _enableLabel2;
        private bool _enableLabel3;
        private bool _enableImage2;
        private string _image2Margin = "0,0,0,0";
        private HorizontalAlignment _image2HorizontalAlignment;
        private VerticalAlignment _image2VerticalAlignment;
        private Stretch _image2Stretch;
        private int _image2CornerRadius;
        private bool _enableImage3;
        private string _image3Margin = "0,0,0,0";
        private HorizontalAlignment _image3HorizontalAlignment;
        private VerticalAlignment _image3VerticalAlignment;
        private Stretch _image3Stretch;
        private int _image3CornerRadius;
        private XmlBrush _defaultImage;
        private XmlBrush _defaultImage2;
        private XmlBrush _defaultImage3; 

        #endregion

        #region Constructor

        public XmlListItemStyle()
        {
            this.SetDefaultValues();
        } 

        #endregion

        #region Item

        [PropertyOrder(1)]
        [DefaultValue(0)]
        [EditorCategory("Item", 1)]
        public int Width
        {
            get { return _width; }
            set { _width = value; NotifyPropertyChanged("Width"); }
        }

        [PropertyOrder(2)]
        [DefaultValue(0)]
        [EditorCategory("Item", 1)]
        public int Height
        {
            get { return _height; }
            set { _height = value; NotifyPropertyChanged("Height"); }
        }

        [PropertyOrder(3)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Item", 1)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ItemMargin
        {
            get { return _itemMargin; }
            set { _itemMargin = value; NotifyPropertyChanged("ItemMargin"); }
        }

        [PropertyOrder(4)]
        [EditorCategory("Item", 1)]
        [DisplayName("Horizontal Alignment")]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set { _horizontalAlignment = value; NotifyPropertyChanged("HorizontalAlignment"); }
        }

        [PropertyOrder(5)]
        [EditorCategory("Item", 1)]
        [DisplayName("Vertical Alignment")]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { _verticalAlignment = value; NotifyPropertyChanged("VerticalAlignment"); }
        }

        [PropertyOrder(1)]
        [EditorCategory("Selection", 2)]
        [DisplayName("Zoom X %")]
        [DefaultValue(100)]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(100, 200)]
        public int SelectionZoomX
        {
            get { return _selectedZoomX; }
            set { _selectedZoomX = value; NotifyPropertyChanged("SelectionZoomX"); }
        }

        [PropertyOrder(2)]
        [EditorCategory("Selection", 2)]
        [DisplayName("Zoom Y %")]
        [DefaultValue(100)]
        [Editor(typeof(IntMinMaxValueEditor), typeof(ITypeEditor)), PropertyRange(100, 200)]
        public int SelectionZoomY
        {
            get { return _selectedZoomY; }
            set { _selectedZoomY = value; NotifyPropertyChanged("SelectionZoomY"); }
        }

        [PropertyOrder(3)]
        [EditorCategory("Selection", 2)]
        [DisplayName("Zoom Duration")]
        [DefaultValue(250)]
        public int SelectionZoomDuration
        {
            get { return _selectedZoomDuration; }
            set { _selectedZoomDuration = value; NotifyPropertyChanged("SelectionZoomDuration"); }
        }

        [DefaultValue("")]
        [PropertyOrder(201)]
        [EditorCategory("Image", 10)]
        public XmlBrush DefaultImage
        {
            get { return _defaultImage; }
            set { _defaultImage = value; NotifyPropertyChanged("DefaultImage"); }
        }

        #endregion

        #region Label2

        [PropertyOrder(110)]
        [DefaultValue(false)]
        [EditorCategory("Label2", 6)]
        public bool EnableLabel2
        {
            get { return _enableLabel2; }
            set { _enableLabel2 = value; NotifyPropertyChanged("EnableLabel2"); }
        }

        [DefaultValue("Microsoft Sans Serif")]
        [PropertyOrder(120)]
        [EditorCategory("Label2", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string Label2FontType
        {
            get { return _label2FontType; }
            set { _label2FontType = value; NotifyPropertyChanged("Label2FontType"); }
        }

        [DefaultValue("Normal")]
        [PropertyOrder(130)]
        [EditorCategory("Label2", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string Label2FontWeight
        {
            get { return _label2FontWeight; }
            set { _label2FontWeight = value; NotifyPropertyChanged("Label2FontWeight"); }
        }

        [DefaultValue(30)]
        [PropertyOrder(140)]
        [EditorCategory("Label2", 6)]
        public int Label2FontSize
        {
            get { return _label2FontSize; }
            set { _label2FontSize = value; NotifyPropertyChanged("Label2FontSize"); }
        }

        [PropertyOrder(150)]
        [EditorCategory("Label2", 6)]
        public XmlBrush Label2FocusFontBrush
        {
            get { return _label2FocusFontBrush; }
            set { _label2FocusFontBrush = value; NotifyPropertyChanged("Label2FocusFontBrush"); }
        }

        [PropertyOrder(160)]
        [EditorCategory("Label2", 6)]
        public XmlBrush Label2NoFocusFontBrush
        {
            get { return _label2NoFocusFontBrush; }
            set { _label2NoFocusFontBrush = value; NotifyPropertyChanged("Label2NoFocusFontBrush"); }
        }

        [PropertyOrder(170)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Label2", 6)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string Label2Margin
        {
            get { return _label2Margin; }
            set { _label2Margin = value; NotifyPropertyChanged("Label2Margin"); }
        }

        [PropertyOrder(180)]
        [EditorCategory("Label2", 6)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment Label2HorizontalAlignment
        {
            get { return _label2HorizontalAlignment; }
            set { _label2HorizontalAlignment = value; NotifyPropertyChanged("Label2HorizontalAlignment"); }
        }

        [PropertyOrder(190)]
        [EditorCategory("Label2", 6)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment Label2VerticalAlignment
        {
            get { return _label2VerticalAlignment; }
            set { _label2VerticalAlignment = value; NotifyPropertyChanged("Label2VerticalAlignment"); }
        } 

        #endregion

        #region Label3

        [PropertyOrder(110)]
        [DefaultValue(false)]
        [EditorCategory("Label3", 7)]
        public bool EnableLabel3
        {
            get { return _enableLabel3; }
            set { _enableLabel3 = value; NotifyPropertyChanged("EnableLabel3"); }
        }

        [DefaultValue("Microsoft Sans Serif")]
        [PropertyOrder(130)]
        [EditorCategory("Label3", 7)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string Label3FontType
        {
            get { return _label3FontType; }
            set { _label3FontType = value; NotifyPropertyChanged("Label3FontType"); }
        }

        [DefaultValue("Normal")]
        [PropertyOrder(130)]
        [EditorCategory("Label3", 7)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string Label3FontWeight
        {
            get { return _label3FontWeight; }
            set { _label3FontWeight = value; NotifyPropertyChanged("Label3FontWeight"); }
        }

        [DefaultValue(30)]
        [PropertyOrder(140)]
        [EditorCategory("Label3", 7)]
        public int Label3FontSize
        {
            get { return _label3FontSize; }
            set { _label3FontSize = value; NotifyPropertyChanged("Label3FontSize"); }
        }

        [PropertyOrder(150)]
        [EditorCategory("Label3", 7)]
        public XmlBrush Label3FocusFontBrush
        {
            get { return _label3FocusFontBrush; }
            set { _label3FocusFontBrush = value; NotifyPropertyChanged("Label3FocusFontBrush"); }
        }

        [PropertyOrder(160)]
        [EditorCategory("Label3", 7)]
        public XmlBrush Label3NoFocusFontBrush
        {
            get { return _label3NoFocusFontBrush; }
            set { _label3NoFocusFontBrush = value; NotifyPropertyChanged("Label3NoFocusFontBrush"); }
        }

        [PropertyOrder(170)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Label3", 7)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string Label3Margin
        {
            get { return _label3Margin; }
            set { _label3Margin = value; NotifyPropertyChanged("Label3Margin"); }
        }

        [PropertyOrder(180)]
        [EditorCategory("Label3", 7)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment Label3HorizontalAlignment
        {
            get { return _label3HorizontalAlignment; }
            set { _label3HorizontalAlignment = value; NotifyPropertyChanged("Label3HorizontalAlignment"); }
        }

        [PropertyOrder(190)]
        [EditorCategory("Label3", 7)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment Label3VerticalAlignment
        {
            get { return _label3VerticalAlignment; }
            set { _label3VerticalAlignment = value; NotifyPropertyChanged("Label3VerticalAlignment"); }
        } 

        #endregion

        #region Image2

        [PropertyOrder(200)]
        [DefaultValue(false)]
        [EditorCategory("Image2", 12)]
        public bool EnableImage2
        {
            get { return _enableImage2; }
            set { _enableImage2 = value; NotifyPropertyChanged("EnableImage2"); }
        }

        [DefaultValue("")]
        [PropertyOrder(201)]
        [EditorCategory("Image2", 12)]
        public XmlBrush DefaultImage2
        {
            get { return _defaultImage2; }
            set { _defaultImage2 = value; NotifyPropertyChanged("DefaultImage2"); }
        }

        [PropertyOrder(210)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Image2", 12)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string Image2Margin
        {
            get { return _image2Margin; }
            set { _image2Margin = value; NotifyPropertyChanged("Image2Margin"); }
        }

        [PropertyOrder(220)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment Image2HorizontalAlignment
        {
            get { return _image2HorizontalAlignment; }
            set { _image2HorizontalAlignment = value; NotifyPropertyChanged("Image2HorizontalAlignment"); }
        }

        [PropertyOrder(230)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment Image2VerticalAlignment
        {
            get { return _image2VerticalAlignment; }
            set { _image2VerticalAlignment = value; NotifyPropertyChanged("Image2VerticalAlignment"); }
        }

        [PropertyOrder(240)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(Stretch.Uniform)]
        public Stretch Image2Stretch
        {
            get { return _image2Stretch; }
            set { _image2Stretch = value; NotifyPropertyChanged("Image2Stretch"); }
        }

        [PropertyOrder(250)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(0)]
        public int Image2CornerRadius
        {
            get { return _image2CornerRadius; }
            set { _image2CornerRadius = value; NotifyPropertyChanged("Image2CornerRadius"); }
        }

        #endregion

        #region Image3

    

        [PropertyOrder(200)]
        [DefaultValue(false)]
        [EditorCategory("Image3", 13)]
        public bool EnableImage3
        {
            get { return _enableImage3; }
            set { _enableImage3 = value; NotifyPropertyChanged("EnableImage3"); }
        }

        [DefaultValue("")]
        [PropertyOrder(201)]
        [EditorCategory("Image3", 13)]
        public XmlBrush DefaultImage3
        {
            get { return _defaultImage3; }
            set { _defaultImage3 = value; NotifyPropertyChanged("DefaultImage3"); }
        }

        [PropertyOrder(210)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Image3", 13)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string Image3Margin
        {
            get { return _image3Margin; }
            set { _image3Margin = value; NotifyPropertyChanged("Image3Margin"); }
        }

        [PropertyOrder(220)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment Image3HorizontalAlignment
        {
            get { return _image3HorizontalAlignment; }
            set { _image3HorizontalAlignment = value; NotifyPropertyChanged("Image3HorizontalAlignment"); }
        }

        [PropertyOrder(230)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment Image3VerticalAlignment
        {
            get { return _image3VerticalAlignment; }
            set { _image3VerticalAlignment = value; NotifyPropertyChanged("Image3VerticalAlignment"); }
        }

        [PropertyOrder(240)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(Stretch.Uniform)]
        public Stretch Image3Stretch
        {
            get { return _image3Stretch; }
            set { _image3Stretch = value; NotifyPropertyChanged("Image3Stretch"); }
        }

        [PropertyOrder(250)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(0)]
        public int Image3CornerRadius
        {
            get { return _image3CornerRadius; }
            set { _image3CornerRadius = value; NotifyPropertyChanged("Image3CornerRadius"); }
        }

        #endregion

        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            Label2FocusFontBrush = style.GetStyle(Label2FocusFontBrush);
            Label2NoFocusFontBrush = style.GetStyle(Label2NoFocusFontBrush);
            Label3FocusFontBrush = style.GetStyle(Label3FocusFontBrush);
            Label3NoFocusFontBrush = style.GetStyle(Label3NoFocusFontBrush);
            DefaultImage = style.GetStyle(DefaultImage);
            DefaultImage2 = style.GetStyle(DefaultImage2);
            DefaultImage3 = style.GetStyle(DefaultImage3);
        }
    }
}
