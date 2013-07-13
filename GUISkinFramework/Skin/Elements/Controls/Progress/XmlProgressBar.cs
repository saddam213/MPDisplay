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

        private string _progressValue;
        [DefaultValue("")]
        [PropertyOrder(100)]
        [EditorCategory("ProgressBar", 7)]
        [DisplayName("Progress Value")]
        [Editor(typeof(NumberEditor), typeof(ITypeEditor))]
        public string ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; NotifyPropertyChanged("ProgressValue"); }
        }
        

        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlProgressBarStyle>(ControlStyle);
        }
    }
}
