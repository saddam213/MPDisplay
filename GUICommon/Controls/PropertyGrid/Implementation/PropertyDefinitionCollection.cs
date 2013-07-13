﻿using System.Collections.ObjectModel;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class PropertyDefinitionCollection : ObservableCollection<PropertyDefinition>
    {
        public PropertyDefinition this[string propertyName]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.Name == propertyName)
                        return item;
                }

                return null;
            }
        }
    }
}
