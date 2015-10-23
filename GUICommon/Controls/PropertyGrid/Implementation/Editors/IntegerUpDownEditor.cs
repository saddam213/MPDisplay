using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class IntegerUpDownEditor : TypeEditor<IntegerUpDown>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new Thickness(0);
        }
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = IntegerUpDown.ValueProperty;
        }
    }
}
