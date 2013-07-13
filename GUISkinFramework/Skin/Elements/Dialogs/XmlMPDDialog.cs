using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Controls;
using GUISkinFramework.Windows;

namespace GUISkinFramework.Dialogs
{
    [Serializable]
    [XmlType(TypeName = "MPDisplayDialog")]
    public class XmlMPDDialog : XmlDialog
    {
        public override string DisplayType
        {
            get { return "MPDisplay Dialog"; }
        }
    }
}
