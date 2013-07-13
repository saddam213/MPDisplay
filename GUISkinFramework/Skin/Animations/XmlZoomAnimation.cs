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
