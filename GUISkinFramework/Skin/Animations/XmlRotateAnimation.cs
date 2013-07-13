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
    [XmlType(TypeName = "Rotate")]
    public class XmlRotateAnimation : XmlAnimation
    {
        public XmlRotateAnimation()
        {
            this.SetDefaultValues();
        }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("X Axis", 1), PropertyOrder(1)]
        [DisplayName("From")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DXFrom { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("X Axis", 1), PropertyOrder(2)]
        [DisplayName("To")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DXTo { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("X Axis", 1), PropertyOrder(3)]
        [DisplayName("Center From")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DCenterXFrom { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("X Axis", 1), PropertyOrder(4)]
        [DisplayName("Center To")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DCenterXTo { get; set; }






        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Y Axis", 2), PropertyOrder(1)]
        [DisplayName("From")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DYFrom { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Y Axis", 2), PropertyOrder(2)]
        [DisplayName("To")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DYTo { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Y Axis", 2), PropertyOrder(3)]
        [DisplayName("Center From")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DCenterYFrom { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Y Axis", 2), PropertyOrder(4)]
        [DisplayName("Center To")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DCenterYTo { get; set; }







        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Z Axis", 3), PropertyOrder(1)]
        [DisplayName("From")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DZFrom { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Z Axis", 3), PropertyOrder(2)]
        [DisplayName("To")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DZTo { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Z Axis", 3), PropertyOrder(3)]
        [DisplayName("Center From")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DCenterZFrom { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Z Axis", 3), PropertyOrder(4)]
        [DisplayName("Center To")]
        [Editor(typeof(AnimationValueEditor), typeof(ITypeEditor)), PropertyRange(-360, 360)]
        public int Pos3DCenterZTo { get; set; }



      

        public override string DisplayName
        {
            get
            {
                return "RotateAnimation";
            }
        }
    }
}
