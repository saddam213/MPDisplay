using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;
using GUISkinFramework.Styles;

namespace GUISkinFramework.Controls
{
    [Serializable]
    [XmlType(TypeName = "ProgressBar")]
    public class XmlProgressBar : XmlControl
    {
        private XmlProgressBarStyle _controlStyle;
        private string _progressValue = "";
        private string _labelMovingText = "";
        private string _defaultLabelMovingText = "";
        private string _labelFixedText = "";
        private string _defaultLabelFixedText = "";
 
        [XmlElement("ProgressBarStyle")]
        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        public XmlProgressBarStyle ControlStyle
        {
            get { return _controlStyle; } 
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        [DefaultValue("")]
        [PropertyOrder(110)]
        [EditorCategory("ProgressBar", 7)]
        [DisplayName("Progress Value")]
        [Editor(typeof(NumberEditor), typeof(ITypeEditor))]
        public string ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; NotifyPropertyChanged("ProgressValue"); }
        }

        [DefaultValue("")]
        [PropertyOrder(150)]
        [EditorCategory("Label", 8)]
        [Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        public string LabelMovingText
        {
            get { return _labelMovingText; }
            set { _labelMovingText = value; NotifyPropertyChanged("LabelMovingText"); }
        }

        [DefaultValue("")]
        [PropertyOrder(155)]
        [EditorCategory("Label", 7)]
        [Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        public string DefaultLabelMovingText
        {
            get { return _defaultLabelMovingText; }
            set { _defaultLabelMovingText = value; NotifyPropertyChanged("DefaultLabelMovingText"); }
        }

        [DefaultValue("")]
        [PropertyOrder(160)]
        [EditorCategory("Label", 8)]
        [Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        public string LabelFixedText
        {
            get { return _labelFixedText; }
            set { _labelFixedText = value; NotifyPropertyChanged("LabelFixedText"); }
        }

        [DefaultValue("")]
        [PropertyOrder(165)]
        [EditorCategory("Label", 7)]
        [Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        public string DefaultLabelFixedText
        {
            get { return _defaultLabelFixedText; }
            set { _defaultLabelFixedText = value; NotifyPropertyChanged("DefaultLabelFixedText"); }
        }

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlProgressBarStyle>(ControlStyle);
        }
    }
}
