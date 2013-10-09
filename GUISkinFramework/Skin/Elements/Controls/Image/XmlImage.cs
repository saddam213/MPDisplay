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

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlImageStyle>(ControlStyle);
            CoverImage = style.GetStyle<XmlBrush>(CoverImage);
        }
    }
}
