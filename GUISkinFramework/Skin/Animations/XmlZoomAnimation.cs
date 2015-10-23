using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Zoom")]
    public class XmlZoomAnimation : XmlAnimation
    {
        public XmlZoomAnimation()
        {
            this.SetDefaultValues();
        }


        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Zoom", 1)]
        [DisplayName("From %")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(0, int.MaxValue)]
        public int From { get; set; }

        [DefaultValue(100)]
        [XmlAttribute]
        [EditorCategory("Zoom", 1)]
        [DisplayName("To %")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(0, int.MaxValue)]
        public int To { get; set; }

        public override string DisplayName
        {
            get
            {
                return "ZoomAnimation";
            }
        }
    }
}
