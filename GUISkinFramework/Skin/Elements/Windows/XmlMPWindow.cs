using System;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "MediaPortalWindow")]
    public class XmlMPWindow : XmlWindow
    {
        public XmlMPWindow()
        {
            this.SetDefaultValues();
        }

   


        public override string DisplayType
        {
            get { return "MediaPortal Window"; }
        }
    }
}
