using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Controls
{
    [Serializable]
    [XmlType(TypeName = "EqualizerStyle")]
    [ExpandableObject]
    public class XmlEqualizerStyle : XmlControlStyle
    {
        private string _borderThickness;
        private string _borderCornerRadius;
        private XmlBrush _backgroundBrush;
        private XmlBrush _borderBrush;
        private XmlBrush _lowRangeColor;
        private XmlBrush _medRangeColor;
        private XmlBrush _maxRangeColor;
        private XmlBrush _bandBorderColor;
        private XmlBrush _fallOffColor;
     

        [DefaultValue("0")]
        [PropertyOrder(50)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; NotifyPropertyChanged("BorderThickness"); }
        }

        [DefaultValue("0")]
        [PropertyOrder(51)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string CornerRadius
        {
            get { return _borderCornerRadius; }
            set { _borderCornerRadius = value; NotifyPropertyChanged("CornerRadius"); }
        }

        [PropertyOrder(52)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; NotifyPropertyChanged("BackgroundBrush"); }
        }

        [PropertyOrder(53)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BorderBrush
        {
            get { return _borderBrush; }
            set { _borderBrush = value; NotifyPropertyChanged("BorderBrush"); }
        }


        [PropertyOrder(70)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush LowRangeColor
        {
            get { return _lowRangeColor; }
            set { _lowRangeColor = value; NotifyPropertyChanged("LowRangeColor"); }
        }

        [PropertyOrder(71)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush MedRangeColor
        {
            get { return _medRangeColor; }
            set { _medRangeColor = value; NotifyPropertyChanged("MedRangeColor"); }
        }

        [PropertyOrder(72)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush MaxRangeColor
        {
            get { return _maxRangeColor; }
            set { _maxRangeColor = value; NotifyPropertyChanged("MaxRangeColor"); }
        }

        [PropertyOrder(73)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BandBorderColor
        {
            get { return _bandBorderColor; }
            set { _bandBorderColor = value; NotifyPropertyChanged("BandBorderColor"); }
        }

        [PropertyOrder(74)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush FallOffColor
        {
            get { return _fallOffColor; }
            set { _fallOffColor = value; NotifyPropertyChanged("FallOffColor"); }
        }
     
    }

  
  
}
