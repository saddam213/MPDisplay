using System.Collections.ObjectModel;
using System.Linq;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class PropertyDefinitionCollection : ObservableCollection<PropertyDefinition>
    {
        public PropertyDefinition this[string propertyName]
        {
            get
            { return Items.FirstOrDefault(item => item.Name == propertyName); }
        }
    }
}
