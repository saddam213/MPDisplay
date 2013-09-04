using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Editor.PropertyEditors;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;
using System.ComponentModel;

namespace GUISkinFramework.Windows
{
    [Serializable]
    [XmlType(TypeName = "MPDisplayWindow")]
    public partial class XmlMPDWindow : XmlWindow
    {

        public override string DisplayType
        {
            get { return "MPDisplay Window"; }
        }


      
    }
}
