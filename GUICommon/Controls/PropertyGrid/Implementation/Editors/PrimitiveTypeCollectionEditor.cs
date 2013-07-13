using System;

namespace MPDisplay.Common.Controls.PropertyGrid.Editors
{
    public class PrimitiveTypeCollectionEditor : TypeEditor<MPDisplay.Common.Controls.PrimitiveTypeCollectionEditor>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new System.Windows.Thickness(0);
            Editor.Content = "(Collection)";
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = MPDisplay.Common.Controls.PrimitiveTypeCollectionEditor.ItemsSourceProperty;
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            Editor.ItemsSourceType = propertyItem.PropertyType;
            Editor.ItemType = propertyItem.PropertyType.GetGenericArguments()[0];
            base.ResolveValueBinding(propertyItem);
        }
    }
}
