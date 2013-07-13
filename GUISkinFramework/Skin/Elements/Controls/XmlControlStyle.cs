using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Styles;

namespace GUISkinFramework.Controls
{
   [XmlType(TypeName = "ControlStyle")]
    public class XmlControlStyle : XmlStyle
    {
        public virtual void LoadSubStyles(XmlStyleCollection style)
        {
        }
    }
}
