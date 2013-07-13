using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    [XmlType(TypeName = "Group")]
    public class XmlGroup : XmlControl, IXmlControlHost
    {
        [XmlIgnore]
        [Browsable(false)]
        public string DisplayType
        {
            get { return "Group"; }
        }

        private XmlGroupStyle _controlStyle;

        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        [XmlElement("GroupStyle")]
        public XmlGroupStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }


        [Browsable(false)]
        [XmlArray(ElementName="GroupControls")]
        public ObservableCollection<XmlControl> Controls { get; set; }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlGroupStyle>(ControlStyle);
        }
    }
}
