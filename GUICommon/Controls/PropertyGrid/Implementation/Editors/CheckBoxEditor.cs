using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class CheckBoxEditor : TypeEditor<CheckBox>
    {
        protected override void SetControlProperties()
        {
            Editor.Margin = new Thickness(5, 0, 0, 0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = ToggleButton.IsCheckedProperty;
        }
    }
}
