using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class EditorDefinitionCollection : ObservableCollection<EditorDefinition>
    {
        public EditorDefinition this[string propertyName]
        {
            get
            { return Items.FirstOrDefault(item => item.PropertiesDefinitions.Any(x => x.Name == propertyName)); }
        }

        public EditorDefinition this[Type targetType]
        {
            get
            { return Items.FirstOrDefault(item => item.TargetType == targetType); }
        }
    }
}
