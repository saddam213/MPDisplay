using System;
using System.ComponentModel;
using System.Xml.Serialization;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "MPDisplayWindow")]
    public class XmlMPDWindow : XmlWindow
    {

        private bool _autoCloseWindow = true;

        public override string DisplayType => "MPDisplay Window";

        [PropertyOrder(10)]
        [DefaultValue(true)]
        [EditorCategory("Window", 0)]
        public bool AutoCloseWindow
        {
            get { return _autoCloseWindow; }
            set { _autoCloseWindow = value; NotifyPropertyChanged("AutoCloseWindow"); }
        }

      
    }
}
