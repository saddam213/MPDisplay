using System.Windows;

namespace MPDisplay.Common.Controls.PropertyGrid.Editors
{
    public interface ITypeEditor
    {
        FrameworkElement ResolveEditor(PropertyItem propertyItem);
    }
}
