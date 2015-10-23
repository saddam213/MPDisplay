using System.Windows;
using System.Windows.Controls;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class TextBoxEditor : TypeEditor<WatermarkTextBox>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new Thickness(0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }
    }
}
