using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Animations
{
    [Serializable]
    [XmlType(TypeName = "Slide")]
    public class XmlSlideAnimation : XmlAnimation
    {
        public XmlSlideAnimation()
        {
            this.SetDefaultValues();
        }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Slide Start", 1), PropertyOrder(1)]
        [DisplayName("Start PosX")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor))]
        public int StartX { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Slide Start", 1), PropertyOrder(2)]
        [DisplayName("Start PosY")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor))]
        public int StartY { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Slide Start", 1), PropertyOrder(3)]
        [DisplayName("Start PosZ")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor))]
        public int StartZ { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Slide End", 2), PropertyOrder(4)]
        [DisplayName("End PosX")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor))]
        public int EndX { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Slide End", 2), PropertyOrder(5)]
        [DisplayName("End PosY")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor))]
        public int EndY { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Slide End", 2), PropertyOrder(6)]
        [DisplayName("End PosZ")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor))]
        public int EndZ { get; set; }

        public override string DisplayName
        {
            get
            {
                return "SlideAnimation";
            }
        }
    }
}
