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
        private string _borderThickness;
        private string _borderCornerRadius;
        private XmlBrush _backgroundBrush;
        private XmlBrush _shadeBrush;
        private XmlBrush _glossBrush;
        private XmlBrush _borderBrush;
        private XmlBrush _barBorderBrush;
        private XmlBrush _barBackgroundBrush;
        private string _barBorderCornerRadius;
        private string _barBorderThickness;
        private string _barMargin;

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




        [DefaultValue("0")]
        [PropertyOrder(80)]
        [DisplayName("Bar Margin")]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BarMargin
        {
            get { return _barMargin; }
            set { _barMargin = value; NotifyPropertyChanged("BarMargin"); }
        }


        [DefaultValue("0")]
        [PropertyOrder(82)]
        [DisplayName("Bar BorderThickness")]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BarBorderThickness
        {
            get { return _barBorderThickness; }
            set { _barBorderThickness = value; NotifyPropertyChanged("BarBorderThickness"); }
        }

        [DefaultValue("0")]
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

     
















        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            GlossBrush = style.GetStyle<XmlBrush>(GlossBrush);
            ShadeBrush = style.GetStyle<XmlBrush>(ShadeBrush);
            BorderBrush = style.GetStyle<XmlBrush>(BorderBrush);
            BackgroundBrush = style.GetStyle<XmlBrush>(BackgroundBrush);
        }
    }

 
}
