using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class ColorEditor : TypeEditor<ColorPicker>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new Thickness(0);
            Editor.DisplayColorAndName = true;
        }
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = ColorPicker.SelectedColorProperty;
        }
    }
}
