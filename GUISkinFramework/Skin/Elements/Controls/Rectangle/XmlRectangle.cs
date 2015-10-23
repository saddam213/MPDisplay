using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Rectangle")]
    public class XmlRectangle : XmlControl
    {
        private string _borderThickness  = "0,0,0,0";
        private string _borderCornerRadius  = "0,0,0,0";
        private XmlBrush _backgroundBrush;
        private XmlBrush _shadeBrush;
        private XmlBrush _glossBrush;
        private XmlBrush _borderBrush;

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
    }
}
