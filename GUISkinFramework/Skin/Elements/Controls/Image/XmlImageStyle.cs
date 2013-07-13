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
    [XmlType(TypeName = "ImageStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [ExpandableObject]
    public class XmlImageStyle : XmlControlStyle
    {
        private XmlBrush _borderBrush;
        private XmlBrush _backgroundBrush;
        private string _borderCornerRadius;
        private string _borderThickness;
        [DefaultValue("0")]
        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; NotifyPropertyChanged("BorderThickness"); }
        }

        [DefaultValue("0")]
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

        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            BackgroundBrush = style.GetStyle<XmlBrush>(BackgroundBrush);
            BorderBrush = style.GetStyle<XmlBrush>(BorderBrush);
        }
   
    }

 
}
