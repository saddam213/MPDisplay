using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class DoubleUpDownEditor : TypeEditor<DoubleUpDown>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new Thickness(0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = DoubleUpDown.ValueProperty;
        }
    }
}
