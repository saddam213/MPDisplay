using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Group")]
    public class XmlGroup : XmlControl, IXmlControlHost
    {
        [XmlIgnore]
        [Browsable(false)]
        public string DisplayType => "Group";

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
            ControlStyle = style.GetControlStyle(ControlStyle);
        }
    }
}
