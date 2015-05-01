using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Image")]
    public class XmlImage : XmlControl
    {
        private XmlBrush _coverImage;
        private string _imageMargin = "0,0,0,0";
        private int _imageCornerRadius;
        private Stretch _imageStretch;
        private string _image = string.Empty;
        private string _defaultImage = string.Empty;
        private string _coverCornerRadius = "0,0,0,0";
        private XmlImageStyle _controlStyle;
        private string _mapData = string.Empty;
        private bool _showMapControls;
        private int _defaultMapZoom = 15;
      
        [XmlElement("ImageStyle")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        [DisplayName("Style"), EditorCategory("Appearance", 3)]
        public XmlImageStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        [DefaultValue("")]
        [PropertyOrder(10)]
        [EditorCategory("Image", 8)]
        [Editor(typeof(ImageEditor), typeof(ITypeEditor))]
        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        [DefaultValue("")]
        [PropertyOrder(11)]
        [EditorCategory("Image", 8)]
        [Editor(typeof(ImageEditor), typeof(ITypeEditor))]
        public string DefaultImage
        {
            get { return _defaultImage; }
            set { _defaultImage = value; NotifyPropertyChanged("DefaultImage"); }
        }

        [PropertyOrder(20)]
        //[DefaultValue(Stretch.Uniform)]
        [EditorCategory("Image", 8)]
        public Stretch ImageStretch
        {
            get { return _imageStretch; }
            set { _imageStretch = value; NotifyPropertyChanged("ImageStretch"); }
        }

        [PropertyOrder(30)]
        [DefaultValue(0)]
        [EditorCategory("Image", 8)]
        public int ImageCornerRadius
        {
            get { return _imageCornerRadius; }
            set { _imageCornerRadius = value; NotifyPropertyChanged("ImageCornerRadius"); }
        }

        [PropertyOrder(40)]
        [DefaultValue("0,0,0,0")]
        [EditorCategory("Image", 8)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ImageMargin
        {
            get { return _imageMargin; }
            set { _imageMargin = value; NotifyPropertyChanged("ImageMargin"); }
        }


        [DefaultValue(null)]
        [PropertyOrder(50)]
        [EditorCategory("Image", 8)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush CoverImage
        {
            get { return _coverImage; }
            set { _coverImage = value; NotifyPropertyChanged("CoverImage"); }
        }

        [DefaultValue("0")]
        [PropertyOrder(60)]
        [EditorCategory("Image", 8)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string CoverCornerRadius
        {
            get { return _coverCornerRadius; }
            set { _coverCornerRadius = value; NotifyPropertyChanged("CoverCornerRadius"); }
        }

        [DefaultValue("")]
        [PropertyOrder(100)]
        [EditorCategory("GPSMap", 9)]
        [Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        public string MapData
        {
            get { return _mapData; }
            set { _mapData = value; NotifyPropertyChanged("MapData"); }
        }

        [PropertyOrder(101)]
        [DefaultValue(false)]
        [EditorCategory("GPSMap", 9)]
        public bool ShowMapControls
        {
            get { return _showMapControls; }
            set { _showMapControls = value; NotifyPropertyChanged("ShowMapControls"); }
        }

        [PropertyOrder(102)]
        [DefaultValue(15)]
        [EditorCategory("GPSMap", 9)]
        public int DefaultMapZoom
        {
            get { return _defaultMapZoom; }
            set { _defaultMapZoom = value; NotifyPropertyChanged("DefaultMapZoom"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle(ControlStyle);
            CoverImage = style.GetStyle(CoverImage);
        }
    }
}
