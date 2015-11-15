using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{

    [XmlType(TypeName = "ControlStyle")]
    public class XmlControlStyle : XmlStyle
    {
        public virtual void LoadSubStyles(XmlStyleCollection style)
        {
        }
    }
}
