using System;
using System.Windows;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "ImageStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [ExpandableObject]
    public class XmlImageStyle : XmlControlStyle
    {
        private XmlBrush _borderBrush;
        private XmlBrush _backgroundBrush;
        private string _borderCornerRadius = "0,0,0,0";
        private string _borderThickness = "0,0,0,0";
        private VerticalAlignment _imageVerticalAlignment = VerticalAlignment.Center;
        private HorizontalAlignment _imageHorizontalAlignment = HorizontalAlignment.Center;

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

        [PropertyOrder(70)]
        [EditorCategory("Alignment", 10)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment ImageHorizontalAlignment
        {
            get { return _imageHorizontalAlignment; }
            set { _imageHorizontalAlignment = value; NotifyPropertyChanged("ImageHorizontalAlignment"); }
        }

        [PropertyOrder(71)]
        [EditorCategory("Alignment", 10)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment ImageVerticalAlignment
        {
            get { return _imageVerticalAlignment; }
            set { _imageVerticalAlignment = value; NotifyPropertyChanged("ImageVerticalAlignment"); }
        }
        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            BackgroundBrush = style.GetStyle<XmlBrush>(BackgroundBrush);
            BorderBrush = style.GetStyle<XmlBrush>(BorderBrush);
        }
   
    }

 
}
