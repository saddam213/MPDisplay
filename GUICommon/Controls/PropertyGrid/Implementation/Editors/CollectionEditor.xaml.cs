using System.Windows;
using System.Windows.Data;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    /// <summary>
    /// Interaction logic for CollectionEditor.xaml
    /// </summary>
    public partial class CollectionEditor : ITypeEditor
    {
        PropertyItem _item;

        public CollectionEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CollectionEditorDialog editor = new CollectionEditorDialog(_item.PropertyType);
            Binding binding = new Binding("Value")
            {
                Source = _item,
                Mode = _item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            BindingOperations.SetBinding(editor, CollectionEditorDialog.ItemsSourceProperty, binding);
            editor.ShowDialog();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            return this;
        }
    }
}
