using System.Collections.Generic;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public interface IItemsSource
    {
        ItemCollection GetValues();
    }

    public class Item
    {
        public string DisplayName { get; set; }
        public object Value { get; set; }        
    }

    public class ItemCollection : List<Item>
    {
        public void Add(object value)
        {
            Item item = new Item {DisplayName = value.ToString(), Value = value};
            base.Add(item);
        }

        public void Add(object value, string displayName)
        {
            Item newItem = new Item {DisplayName = displayName, Value = value};
            base.Add(newItem);
        }

    }
}
