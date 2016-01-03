using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class ItemsSourceAttributeEditor : TypeEditor<ComboBox>
    {
        private readonly ItemsSourceAttribute _attribute;

        public ItemsSourceAttributeEditor(ItemsSourceAttribute attribute)
        {
            _attribute = attribute;
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = Selector.SelectedValueProperty;
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            SetItemsSource();
            base.ResolveValueBinding(propertyItem);
        }

        protected override void SetControlProperties()
        {
            Editor.DisplayMemberPath = "DisplayName";
            Editor.SelectedValuePath = "Value";
        }

        private void SetItemsSource()
        {
            Editor.ItemsSource = CreateItemsSource();
        }

        private IEnumerable CreateItemsSource()
        {
            var instance = Activator.CreateInstance(_attribute.Type);
            var itemsSource = instance as IItemsSource;
            return itemsSource?.GetValues();
        }
    }
}
