using System;
using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class EditorDefinition
    {
        public DataTemplate EditorTemplate { get; set; }

        public PropertyDefinitionCollection PropertiesDefinitions { get; set; } = new PropertyDefinitionCollection();

        public Type TargetType { get; set; }
    }
}
