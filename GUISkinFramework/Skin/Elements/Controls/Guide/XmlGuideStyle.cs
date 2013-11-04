using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
    [XmlType(TypeName = "GuideStyle")]
    [XmlInclude(typeof(XmlBrush))]
    [ExpandableObject]
    public class XmlGuideStyle : XmlControlStyle
    {
        private string _borderThickness;
        private string _borderCornerRadius;
        private XmlBrush _backgroundBrush;
        private XmlBrush _shadeBrush;
        private XmlBrush _glossBrush;
        private XmlBrush _borderBrush;

        public XmlGuideStyle()
        {
            this.SetDefaultValues();
        }

        [DefaultValue("0")]
        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; NotifyPropertyChanged("BorderThickness"); }
        }

        [DefaultValue("0")]
        [PropertyOrder(61)]
        [EditorCategory("Appearance", 3)]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string CornerRadius
        {
            get { return _borderCornerRadius; }
            set { _borderCornerRadius = value; NotifyPropertyChanged("CornerRadius"); }
        }

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; NotifyPropertyChanged("BackgroundBrush"); }
        }

        [PropertyOrder(63)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush BorderBrush
        {
            get { return _borderBrush; }
            set { _borderBrush = value; NotifyPropertyChanged("BorderBrush"); }
        }

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush ChannelListBackgroundBrush
        {
            get { return _channelListBackgroundBrush; }
            set { _channelListBackgroundBrush = value; NotifyPropertyChanged("ChannelListBackgroundBrush"); }
        }

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush ChannelListBorderBrush
        {
            get { return _channelListBorderBrush; }
            set { _channelListBorderBrush = value; NotifyPropertyChanged("ChannelListBorderBrush"); }
        }

        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ChannelListBorderThickness
        {
            get { return _channelListBorderThickness; }
            set { _channelListBorderThickness = value; NotifyPropertyChanged("ChannelListBorderThickness"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ChannelListCornerRadius
        {
            get { return _channelListCornerRadius; }
            set { _channelListCornerRadius = value; NotifyPropertyChanged("ChannelListCornerRadius"); }
        }

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush ProgramListBackgroundBrush
        {
            get { return _programListBackgroundBrush; }
            set { _programListBackgroundBrush = value; NotifyPropertyChanged("ProgramListBackgroundBrush"); }
        }

      

        [PropertyOrder(62)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush ProgramListBorderBrush
        {
            get { return _programListBorderBrush; }
            set { _programListBorderBrush = value; NotifyPropertyChanged("ProgramListBorderBrush"); }
        }

        [PropertyOrder(60)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ProgramListBorderThickness
        {
            get { return _programListBorderThickness; }
            set { _programListBorderThickness = value; NotifyPropertyChanged("ProgramListBorderThickness"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Appearance", 3)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string ProgramListCornerRadius
        {
            get { return _programListCornerRadius; }
            set { _programListCornerRadius = value; NotifyPropertyChanged("ProgramListCornerRadius"); }
        }
      

        [PropertyOrder(66)]
        [EditorCategory("Shading", 4)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush ShadeBrush
        {
            get { return _shadeBrush; }
            set { _shadeBrush = value; NotifyPropertyChanged("ShadeBrush"); }
        }

        [PropertyOrder(67)]
        [EditorCategory("Shading", 4)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush GlossBrush
        {
            get { return _glossBrush; }
            set { _glossBrush = value; NotifyPropertyChanged("GlossBrush"); }
        }



     
        private XmlBrush _timelineBackground;
        private XmlBrush _timelineMarkerBrush;
        private int _timelineMarkerThickness;
        private string _timelineFontType;
        private string _timelineFontWeight;
        private int _timelineFontSize;
        private XmlBrush _timelineFontBrush;
        private HorizontalAlignment _timelineLabelHorizontalAlignment;
        private VerticalAlignment _timelineLabelVerticalAlignment;
        private string _timelineItemBorderThickness;
        private string _timelineItemCornerRadius;
        private XmlBrush _timelineItemBorderBrush;
     
        private XmlBrush _channelListBackgroundBrush;
        private XmlBrush _channelListBorderBrush;
        private string _channelListCornerRadius;
        private XmlBrush _programListBackgroundBrush;
        private XmlBrush _programListBorderBrush;
        private string _programListCornerRadius;
        private string _channelListBorderThickness;
        private string _programListBorderThickness;
        private XmlBrush _timelineBorderBrush;
        private string _timelineBorderThickness;
        private string _timelineCornerRadius;


       

        #region Timeline

        [PropertyOrder(20)]
        [DefaultValue(null)]
        [EditorCategory("Timeline", 6)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush TimelineBackground
        {
            get { return _timelineBackground; }
            set { _timelineBackground = value; NotifyPropertyChanged("TimelineBackground"); }
        }

        [PropertyOrder(50)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush TimelineBorderBrush
        {
            get { return _timelineBorderBrush; }
            set { _timelineBorderBrush = value; NotifyPropertyChanged("TimelineBorderBrush"); }
        }

        [PropertyOrder(60)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string TimelineBorderThickness
        {
            get { return _timelineBorderThickness; }
            set { _timelineBorderThickness = value; NotifyPropertyChanged("TimelineBorderThickness"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string TimelineCornerRadius
        {
            get { return _timelineCornerRadius; }
            set { _timelineCornerRadius = value; NotifyPropertyChanged("TimelineCornerRadius"); }
        }


        [PropertyOrder(30)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue(null)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush TimelineMarkerBrush
        {
            get { return _timelineMarkerBrush; }
            set { _timelineMarkerBrush = value; NotifyPropertyChanged("TimelineMarkerBrush"); }
        }

        [PropertyOrder(40)]
        [EditorCategory("Timeline", 6)]
        public int TimelineMarkerThickness
        {
            get { return _timelineMarkerThickness; }
            set { _timelineMarkerThickness = value; NotifyPropertyChanged("TimelineMarkerThickness"); }
        }

        [PropertyOrder(50)]
        [EditorCategory("Timeline", 6)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush TimelineItemBorderBrush
        {
            get { return _timelineItemBorderBrush; }
            set { _timelineItemBorderBrush = value; NotifyPropertyChanged("TimelineItemBorderBrush"); }
        }

        [PropertyOrder(60)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string TimelineItemBorderThickness
        {
            get { return _timelineItemBorderThickness; }
            set { _timelineItemBorderThickness = value; NotifyPropertyChanged("TimelineItemBorderThickness"); }
        }

        [PropertyOrder(70)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue("0,0,0,0")]
        [Editor(typeof(FourPointValueEditor), typeof(ITypeEditor))]
        public string TimelineItemCornerRadius
        {
            get { return _timelineItemCornerRadius; }
            set { _timelineItemCornerRadius = value; NotifyPropertyChanged("TimelineItemCornerRadius"); }
        }

        [DefaultValue("Microsoft Sans Serif")]
        [PropertyOrder(90)]
        [EditorCategory("Timeline", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string TimelineFontType
        {
            get { return _timelineFontType; }
            set { _timelineFontType = value; NotifyPropertyChanged("TimelineFontType"); }
        }

        [DefaultValue("Normal")]
        [PropertyOrder(100)]
        [EditorCategory("Timeline", 6)]
        [Editor(typeof(FontComboBoxEditor), typeof(ITypeEditor))]
        public string TimelineFontWeight
        {
            get { return _timelineFontWeight; }
            set { _timelineFontWeight = value; NotifyPropertyChanged("TimelineFontWeight"); }
        }

        [DefaultValue(30)]
        [PropertyOrder(110)]
        [EditorCategory("Timeline", 6)]
        public int TimelineFontSize
        {
            get { return _timelineFontSize; }
            set { _timelineFontSize = value; NotifyPropertyChanged("TimelineFontSize"); }
        }

        [PropertyOrder(120)]
        [EditorCategory("Timeline", 6)]
        [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
        public XmlBrush TimelineFontBrush
        {
            get { return _timelineFontBrush; }
            set { _timelineFontBrush = value; NotifyPropertyChanged("TimelineFontBrush"); }
        }

        [PropertyOrder(130)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment TimelineLabelHorizontalAlignment
        {
            get { return _timelineLabelHorizontalAlignment; }
            set { _timelineLabelHorizontalAlignment = value; NotifyPropertyChanged("TimelineLabelHorizontalAlignment"); }
        }

        [PropertyOrder(140)]
        [EditorCategory("Timeline", 6)]
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment TimelineLabelVerticalAlignment
        {
            get { return _timelineLabelVerticalAlignment; }
            set { _timelineLabelVerticalAlignment = value; NotifyPropertyChanged("TimelineLabelVerticalAlignment"); }
        }

        #endregion



        public override void LoadSubStyles(XmlStyleCollection style)
        {
            base.LoadSubStyles(style);
            GlossBrush = style.GetStyle<XmlBrush>(GlossBrush);
            ShadeBrush = style.GetStyle<XmlBrush>(ShadeBrush);
            BorderBrush = style.GetStyle<XmlBrush>(BorderBrush);
            BackgroundBrush = style.GetStyle<XmlBrush>(BackgroundBrush);
            TimelineBackground = style.GetStyle<XmlBrush>(TimelineBackground);
            TimelineBorderBrush = style.GetStyle<XmlBrush>(TimelineBorderBrush);
            TimelineMarkerBrush = style.GetStyle<XmlBrush>(TimelineMarkerBrush);
            TimelineFontBrush = style.GetStyle<XmlBrush>(TimelineFontBrush);
            TimelineItemBorderBrush = style.GetStyle<XmlBrush>(TimelineItemBorderBrush);
            ChannelListBackgroundBrush = style.GetStyle<XmlBrush>(ChannelListBackgroundBrush);
            ChannelListBorderBrush = style.GetStyle<XmlBrush>(ChannelListBorderBrush);
            ProgramListBackgroundBrush = style.GetStyle<XmlBrush>(ProgramListBackgroundBrush);
            ProgramListBorderBrush = style.GetStyle<XmlBrush>(ProgramListBorderBrush);
        }
    }

 
    
}
