using System;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "MPDisplayDialog")]
    public class XmlMPDDialog : XmlDialog
    {
        public override string DisplayType => "MPDisplay Dialog";
    }
}
