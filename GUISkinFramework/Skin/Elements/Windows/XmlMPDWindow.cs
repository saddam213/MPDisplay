using System;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "MPDisplayWindow")]
    public class XmlMPDWindow : XmlWindow
    {

        public override string DisplayType
        {
            get { return "MPDisplay Window"; }
        }


      
    }
}
