using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class PrimitiveTypeCollectionEditor : TypeEditor<Controls.PrimitiveTypeCollectionEditor>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new Thickness(0);
            Editor.Content = "(Collection)";
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = Controls.PrimitiveTypeCollectionEditor.ItemsSourceProperty;
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            Editor.ItemsSourceType = propertyItem.PropertyType;
            Editor.ItemType = propertyItem.PropertyType.GetGenericArguments()[0];
            base.ResolveValueBinding(propertyItem);
        }
    }
}
