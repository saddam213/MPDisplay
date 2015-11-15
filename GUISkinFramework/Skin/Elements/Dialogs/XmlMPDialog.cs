using System;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "MediaPortalDialog")]
    public class XmlMPDialog : XmlDialog
    {
        public XmlMPDialog()
        {
            this.SetDefaultValues();
        }

   

        public override string DisplayType => "MediaPortal Dialog";
    }
}
