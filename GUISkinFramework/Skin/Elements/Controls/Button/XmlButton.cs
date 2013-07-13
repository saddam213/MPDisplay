using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [XmlType(TypeName = "Button")]
    public class XmlButton : XmlControl
    {
        private string _labelText;
        private XmlButtonStyle _controlStyle;
        private string _image = "";
       
        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Appearance", 6)]
        [DisplayName("Style")]
        [Editor(typeof(StyleEditor), typeof(ITypeEditor))]
        [XmlElement("ButtonStyle")]
        public XmlButtonStyle ControlStyle
        {
            get { return _controlStyle; }
            set { _controlStyle = value; NotifyPropertyChanged("ControlStyle"); }
        }

        [DefaultValue("")]
        [PropertyOrder(304)]
        [EditorCategory("Image", 8)]
        [Editor(typeof(ImageEditor), typeof(ITypeEditor))]
        public string Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        [DefaultValue("")]
        [PropertyOrder(204)]
        [EditorCategory("Label", 7)]
        [Editor(typeof(LabelEditor), typeof(ITypeEditor))]
        public string LabelText
        {
            get { return _labelText; }
            set { _labelText = value; NotifyPropertyChanged("LabelText"); }
        }

        [DefaultValue(null)]
        [PropertyOrder(100)]
        [EditorCategory("Actions", 10)]
        [XmlArrayItem(ElementName = "Action")]
        [Editor(typeof(ActionEditor), typeof(ITypeEditor))]
        public ObservableCollection<XmlAction> Actions { get; set; }


        public override void ApplyStyle(XmlStyleCollection style)
        {
            base.ApplyStyle(style);
            ControlStyle = style.GetControlStyle<XmlButtonStyle>(ControlStyle);
        }
    }
}
