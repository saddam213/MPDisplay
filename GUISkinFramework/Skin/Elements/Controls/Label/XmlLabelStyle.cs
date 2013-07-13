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
    [XmlType(TypeName = "LabelStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [ExpandableObject]
    public class XmlLabelStyle : XmlControlStyle
    {
        private XmlBrush _fontBrush;
        private int _fontSize = 20;
        private string _fontWeight = "Normal";
        private string _fontType = "Microsoft Sans Serif";
  
        [DefaultValue("Microsoft Sans Serif"), Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        [PropertyOrder(10)]
        public string FontType
        {
            get { return _fontType; }
            set { _fontType = value; NotifyPropertyChanged("FontType"); }
        }

        [DefaultValue("Normal"), Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        [PropertyOrder(20)]
        public string FontWeight
        {
            get { return _fontWeight; }
            set { _fontWeight = value; NotifyPropertyChanged("FontWeight"); }
        }

        [DefaultValue(10)]
        [PropertyOrder(30)]
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; NotifyPropertyChanged("FontSize"); }
        }

        [DefaultValue(null)]
        [PropertyOrder(40)]
        public XmlBrush FontBrush
        {
            get { return _fontBrush; }
            set { _fontBrush = value; NotifyPropertyChanged("FontBrush"); }
        }

        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            FontBrush = style.GetStyle<XmlBrush>(FontBrush);
        }
    }

 
}
