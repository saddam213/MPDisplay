using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public abstract class ComboBoxEditor : TypeEditor<ComboBox>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = Selector.SelectedItemProperty;
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            SetItemsSource(propertyItem);
            base.ResolveValueBinding(propertyItem);
        }

        protected abstract IList<object> CreateItemsSource(PropertyItem propertyItem);

        private void SetItemsSource(PropertyItem propertyItem)
        {
            Editor.ItemsSource = CreateItemsSource(propertyItem);
        }
    }
}
