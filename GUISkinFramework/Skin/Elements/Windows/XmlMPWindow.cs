using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Windows
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
