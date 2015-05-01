using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class DecimalUpDownEditor : TypeEditor<DecimalUpDown>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new Thickness(0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = DecimalUpDown.ValueProperty;
        }
    }
}
