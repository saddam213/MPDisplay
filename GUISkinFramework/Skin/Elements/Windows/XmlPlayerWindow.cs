using System;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "PlayerWindow")]
    public class XmlPlayerWindow : XmlWindow
    {

        public override string DisplayType
        {
            get { return "Now Playing Window"; }
        }


      
    }
}
