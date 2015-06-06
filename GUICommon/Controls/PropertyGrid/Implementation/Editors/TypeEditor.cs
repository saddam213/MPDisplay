using System.Windows;
using System.Windows.Data;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public abstract class TypeEditor<T> : ITypeEditor
        where T : FrameworkElement, new()
    {
        #region Properties

        protected T Editor { get; set; }
        protected DependencyProperty ValueProperty { get; set; }

        #endregion //Properties

        #region ITypeEditor Members

        public virtual FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Editor = new T();
            SetValueDependencyProperty();
            SetControlProperties();
            ResolveValueBinding(propertyItem);
            return Editor;
        }

        #endregion //ITypeEditor Members

        #region Methods

        protected virtual IValueConverter CreateValueConverter()
        {
            return null;
        }

        protected virtual void ResolveValueBinding(PropertyItem propertyItem)
        {
            var binding = new Binding("Value")
            {
                Source = propertyItem,
                ValidatesOnExceptions = true,
                ValidatesOnDataErrors = true,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                Converter = CreateValueConverter()
            };
            BindingOperations.SetBinding(Editor, ValueProperty, binding);
        }

        protected virtual void SetControlProperties()
        {
            //TODO: implement in derived class
        }

        protected abstract void SetValueDependencyProperty();

        #endregion //Methods
    }
}
