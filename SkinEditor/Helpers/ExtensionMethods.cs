using System.Windows;
using System.Windows.Data;

namespace SkinEditor.Helpers
{
    public static class ExtensionMethods
    {
        public static void BindTo(this DependencyObject target, DependencyProperty targetProperty, object source, string sourceProperty, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            BindingOperations.SetBinding(target, targetProperty, new Binding(sourceProperty)
            {
                Source = source,
                Mode = mode,
                Converter = converter
            });
        }
    }
}
