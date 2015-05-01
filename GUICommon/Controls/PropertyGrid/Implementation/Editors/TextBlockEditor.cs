using System.Windows;
using System.Windows.Controls;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class TextBlockEditor : TypeEditor<TextBlock>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBlock.TextProperty;
        }

        protected override void SetControlProperties()
        {
            Editor.Margin = new Thickness(5, 0, 0, 0);
        }
    }
}
