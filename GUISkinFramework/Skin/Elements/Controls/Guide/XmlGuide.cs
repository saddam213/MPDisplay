using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Guide")]
    public class XmlGuide : XmlControl
    {
        private XmlGuideStyle _controlStyle;
        private XmlGuideProgramStyle _guideProgramStyle;
        private XmlGuideChannelStyle _guideChannelStyle;
        private double _timelineMultiplier;
        private int _channelListWidth;
        private int _guideItemHeight;
        private int _timelineHeight;
        private string _guideItemMargin = "0,0,0,0";
        private int _createDialogId = 0;
        private int _cancelDialogId = 0;

        public XmlGuide()
        {
            this.SetDefaultValues();
        }

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


        [PropertyOrder(68)]
        [EditorCategory("Guide", 5)]
        [DefaultValue(200)]
        public int ChannelListWidth
        {
            get { return _channelListWidth; }
            set { _channelListWidth = value; NotifyPropertyChanged("ChannelListWidth"); }
        }

        [PropertyOrder(68)]
        [EditorCategory("Guide", 5)]
        [DefaultValue(40)]
        public int GuideItemHeight
        {
            get { return _guideItemHeight; }
            set { _guideItemHeight = value; NotifyPropertyChanged("GuideItemHeight"); }
        }

        [PropertyOrder(69)]
        [EditorCategory("Guide", 5)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string GuideItemMargin
        {
            get { return _guideItemMargin; }
            set { _guideItemMargin = value; NotifyPropertyChanged("GuideItemMargin"); }
        }

        [PropertyOrder(100)]
        [EditorCategory("Guide", 5)]
        [DefaultValue(40)]
        public int TimelineHeight
        {
            get { return _timelineHeight; }
            set { _timelineHeight = value; NotifyPropertyChanged("TimelineHeight"); }
        }

        [PropertyOrder(101)]
        [EditorCategory("Guide", 5)]
        [DefaultValue(6)]
        public double TimelineMultiplier
        {
            get { return _timelineMultiplier; }
            set { _timelineMultiplier = value; NotifyPropertyChanged("TimelineMultiplier"); }
        }


        [PropertyOrder(60)]
        [EditorCategory("Appearance", 7)]
        [DefaultValue(null)]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlGuideChannelStyle ChannelStyle
        {
            get { return _guideChannelStyle; }
            set { _guideChannelStyle = value; NotifyPropertyChanged("ChannelStyle"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Appearance", 7)]
        [DefaultValue(null)]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlGuideProgramStyle ProgramStyle
        {
            get { return _guideProgramStyle; }
            set { _guideProgramStyle = value; NotifyPropertyChanged("ProgramStyle"); }
        }


        [PropertyOrder(200)]
        [EditorCategory("Schedule", 9)]
        [DefaultValue(0)]
        public int CreateDialogId
        {
            get { return _createDialogId; }
            set { _createDialogId = value; NotifyPropertyChanged("CreateDialogId"); }
        }

        [PropertyOrder(210)]
        [EditorCategory("Schedule", 9)]
        [DefaultValue(0)]
        public int CancelDialogId
        {
            get { return _cancelDialogId; }
            set { _cancelDialogId = value; NotifyPropertyChanged("CreateDialogId"); }
        }


        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlGuideStyle>(ControlStyle);
            ChannelStyle = style.GetControlStyle<XmlGuideChannelStyle>(ChannelStyle);
            ProgramStyle = style.GetControlStyle<XmlGuideProgramStyle>(ProgramStyle);
        }
    }
}
