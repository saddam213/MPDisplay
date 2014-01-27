using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Editor.PropertyEditors;
using GUISkinFramework.Styles;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Controls
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
        private System.Windows.VerticalAlignment _verticalAlignment;
        private System.Windows.HorizontalAlignment _horizontalAlignment;
        private string _itemMargin;
        private int _selectedZoomX = 100;
        private int _selectedZoomY = 100;
        private int _selectedZoomDuration = 250;
        private string _label2FontType;
        private string _label2FontWeight;
        private int _label2FontSize;
        private string _label2Margin;
        private HorizontalAlignment _label2HorizontalAlignment;
        private VerticalAlignment _label2VerticalAlignment;
        private string _label3FontType;
        private string _label3FontWeight;
        private int _label3FontSize;
        private string _label3Margin;
        private HorizontalAlignment _label3HorizontalAlignment;
        private VerticalAlignment _label3VerticalAlignment;
        private XmlBrush _label2FocusFontBrush;
        private XmlBrush _label2NoFocusFontBrush;
        private XmlBrush _label3NoFocusFontBrush;
        private XmlBrush _label3FocusFontBrush;
        private bool _enableLabel2;
        private bool _enableLabel3;
        private bool _enableImage2;
        private string _Image2Margin;
        private System.Windows.HorizontalAlignment _Image2HorizontalAlignment;
        private System.Windows.VerticalAlignment _Image2VerticalAlignment;
        private Stretch _Image2Stretch;
        private int _Image2CornerRadius;
        private bool _enableImage3;
        private string _Image3Margin;
        private System.Windows.HorizontalAlignment _Image3HorizontalAlignment;
        private System.Windows.VerticalAlignment _Image3VerticalAlignment;
        private Stretch _Image3Stretch;
        private int _Image3CornerRadius;
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
            get { return _Image2Margin; }
            set { _Image2Margin = value; NotifyPropertyChanged("Image2Margin"); }
        }

        [PropertyOrder(220)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment Image2HorizontalAlignment
        {
            get { return _Image2HorizontalAlignment; }
            set { _Image2HorizontalAlignment = value; NotifyPropertyChanged("Image2HorizontalAlignment"); }
        }

        [PropertyOrder(230)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment Image2VerticalAlignment
        {
            get { return _Image2VerticalAlignment; }
            set { _Image2VerticalAlignment = value; NotifyPropertyChanged("Image2VerticalAlignment"); }
        }

        [PropertyOrder(240)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(Stretch.Uniform)]
        public Stretch Image2Stretch
        {
            get { return _Image2Stretch; }
            set { _Image2Stretch = value; NotifyPropertyChanged("Image2Stretch"); }
        }

        [PropertyOrder(250)]
        [EditorCategory("Image2", 12)]
        [DefaultValue(0)]
        public int Image2CornerRadius
        {
            get { return _Image2CornerRadius; }
            set { _Image2CornerRadius = value; NotifyPropertyChanged("Image2CornerRadius"); }
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
            get { return _Image3Margin; }
            set { _Image3Margin = value; NotifyPropertyChanged("Image3Margin"); }
        }

        [PropertyOrder(220)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment Image3HorizontalAlignment
        {
            get { return _Image3HorizontalAlignment; }
            set { _Image3HorizontalAlignment = value; NotifyPropertyChanged("Image3HorizontalAlignment"); }
        }

        [PropertyOrder(230)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment Image3VerticalAlignment
        {
            get { return _Image3VerticalAlignment; }
            set { _Image3VerticalAlignment = value; NotifyPropertyChanged("Image3VerticalAlignment"); }
        }

        [PropertyOrder(240)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(Stretch.Uniform)]
        public Stretch Image3Stretch
        {
            get { return _Image3Stretch; }
            set { _Image3Stretch = value; NotifyPropertyChanged("Image3Stretch"); }
        }

        [PropertyOrder(250)]
        [EditorCategory("Image3", 13)]
        [DefaultValue(0)]
        public int Image3CornerRadius
        {
            get { return _Image3CornerRadius; }
            set { _Image3CornerRadius = value; NotifyPropertyChanged("Image3CornerRadius"); }
        }

        #endregion

        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            Label2FocusFontBrush = style.GetStyle<XmlBrush>(Label2FocusFontBrush);
            Label2NoFocusFontBrush = style.GetStyle<XmlBrush>(Label2NoFocusFontBrush);
            Label3FocusFontBrush = style.GetStyle<XmlBrush>(Label3FocusFontBrush);
            Label3NoFocusFontBrush = style.GetStyle<XmlBrush>(Label3NoFocusFontBrush);
            DefaultImage = style.GetStyle<XmlBrush>(DefaultImage);
            DefaultImage2 = style.GetStyle<XmlBrush>(DefaultImage2);
            DefaultImage3 = style.GetStyle<XmlBrush>(DefaultImage3);
        }
    }
}
