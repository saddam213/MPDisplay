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
using GUISkinFramework.Styles;

namespace GUISkinFramework.Controls
{
    [Serializable]
    [XmlType(TypeName = "Guide")]
    public class XmlGuide : XmlControl
    {
        private XmlGuideStyle _controlStyle;


        [XmlElement("GuideStyle")]
        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlGuideStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlGuideStyle>(ControlStyle);
        }
    }
}
