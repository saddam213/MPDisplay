using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Controls;
using GUISkinFramework.Editor.PropertyEditors;
using GUISkinFramework.Windows;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Dialogs
{
    [Serializable]
    [XmlType(TypeName = "MediaPortalDialog")]
    public class XmlMPDialog : XmlDialog
    {
        public XmlMPDialog()
        {
            this.SetDefaultValues();
        }

   

        public override string DisplayType
        {
            get { return "MediaPortal Dialog"; }
        }
    }
}
