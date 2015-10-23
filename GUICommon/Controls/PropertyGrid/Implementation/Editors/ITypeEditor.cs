using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public interface ITypeEditor
    {
        FrameworkElement ResolveEditor(PropertyItem propertyItem);
    }
}
