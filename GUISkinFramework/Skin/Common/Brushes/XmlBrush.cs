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
using GUISkinFramework.Styles;

namespace GUISkinFramework.Common.Brushes
{
    [Serializable]
    [XmlInclude(typeof(XmlColorBrush))]
    [XmlInclude(typeof(XmlGradientBrush))]
    [XmlInclude(typeof(XmlImageBrush))]
    [XmlType(TypeName = "Brush")]
    [Editor(typeof(BrushEditor), typeof(ITypeEditor))]
    public class XmlBrush : XmlStyle
    {

    

      
       
    }
}
