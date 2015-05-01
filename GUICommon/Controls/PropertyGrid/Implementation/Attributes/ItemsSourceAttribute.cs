using System;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class ItemsSourceAttribute : Attribute
    {
        public Type Type { get; set; }

        public ItemsSourceAttribute(Type type)
        {
            var valueSourceInterface = type.GetInterface("MPDisplay.Common.Controls.PropertyGrid.Attributes.IItemsSource");
            if (valueSourceInterface == null)
                throw new ArgumentException("Type must implement the IItemsSource interface.", "type");

            Type = type;
        }
    }
}