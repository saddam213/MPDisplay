using System;
using System.ComponentModel;
using System.Xml.Serialization;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Animation")]
    [XmlInclude(typeof(XmlSlideAnimation))]
    [XmlInclude(typeof(XmlFadeAnimation))]
    [XmlInclude(typeof(XmlZoomAnimation))]
    [XmlInclude(typeof(XmlRotateAnimation))]
    [XmlInclude(typeof(XmlAnimationCondition))]
    public class XmlAnimation
    {
        public XmlAnimation()
        {
            this.SetDefaultValues();
        }

        [DefaultValue(XmlAnimationCondition.None)]
        [XmlAttribute]
        [Browsable(false)]
        public XmlAnimationCondition Condition { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Anmation", 0)]
        public int Duration { get; set; }

        [DefaultValue(0)]
        [XmlAttribute]
        [EditorCategory("Anmation", 0)]
        public int Delay { get; set; }

        [DefaultValue(XmlAnimationEaseMode.None)]
        [XmlAttribute]
        [EditorCategory("Anmation", 0)]
        public XmlAnimationEaseMode Easing { get; set; }

        [DefaultValue(false)]
        [XmlAttribute]
        [EditorCategory("Anmation", 0)]
        public bool Reverse { get; set; }

        [DefaultValue(1)]
        [XmlAttribute]
        [EditorCategory("Anmation", 0)]
        public int Repeat { get; set; }

       [Browsable(false)]
        public virtual string DisplayName => "Animation";
    }

    public enum XmlAnimationEaseMode
    {
        None = 0,
        EaseIn,
        EaseOut,
        EaseInOut
    }

   
    public enum XmlAnimationCondition
    {
        None = 0,
         [XmlEnum]
        WindowOpen = 1,
        WindowClose = 2,
        TouchDown = 3,
        TouchUp = 4,
        VisibleTrue = 5,
        VisibleFalse = 6,
        FocusTrue = 7,
        FocusFalse = 8,
        PropertyChanging = 9,
        PropertyChanged = 10
    }
}
