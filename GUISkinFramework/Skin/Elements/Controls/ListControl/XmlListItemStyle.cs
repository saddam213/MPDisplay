using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public XmlListItemStyle()
        {
            this.SetDefaultValues();
        }

        private int _height;
        private int _width;
        private System.Windows.VerticalAlignment _verticalAlignment;
        private System.Windows.HorizontalAlignment _horizontalAlignment;
        private string _itemMargin;

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
    }
}
