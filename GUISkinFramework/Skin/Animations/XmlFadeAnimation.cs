using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Fade")]
    public class XmlFadeAnimation : XmlAnimation
    {
        public XmlFadeAnimation()
        {
            this.SetDefaultValues();
        }

        [DefaultValue(0)]
        [XmlAttribute]
        [PropertyOrder(100)]
        [EditorCategory("Fade", 1)]
        [DisplayName("From %")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(0, 100)]
        public int From { get; set; }

        [DefaultValue(100)]
        [XmlAttribute]
        [EditorCategory("Fade", 1)]
        [DisplayName("To %")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(0, 100)]
        public int To { get; set; }

        public override string DisplayName => "FadeAnimation";
    }
}
