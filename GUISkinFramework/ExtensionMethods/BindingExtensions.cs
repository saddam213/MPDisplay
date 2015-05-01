using System.Windows;
using System.Windows.Data;

namespace GUISkinFramework.ExtensionMethods
{
    public static class BindingExtensions
    {
        public static void BindTo(this UIElement target, DependencyProperty targetProperty, object source, string sourceProperty, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
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
