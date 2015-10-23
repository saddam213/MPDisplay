using System;
using System.ComponentModel;
using System.Xml.Serialization;
using GUISkinFramework.Editors;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Skin
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
