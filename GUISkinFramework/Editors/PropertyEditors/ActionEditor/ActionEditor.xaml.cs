using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class ActionEditor : ITypeEditor
    {
    
        private PropertyItem _item;

        public ActionEditor()
        {
            InitializeComponent();
          
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ObservableCollection<XmlAction>), typeof(ActionEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<XmlAction> Value
        {
            get { return (ObservableCollection<XmlAction>)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string ActionInfo
        {
            get { return (string)GetValue(ActionInfoProperty); }
            set { SetValue(ActionInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionInfoProperty =
            DependencyProperty.Register("ActionInfo", typeof(string), typeof(ActionEditor), new PropertyMetadata("(Empty)"));

        public string ActionToolTip

        {
            get { return (string)GetValue(ActionToolTipProperty); }
            set { SetValue(ActionToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionToolTipProperty =
            DependencyProperty.Register("ActionToolTip", typeof(string), typeof(ActionEditor), new PropertyMetadata("(Empty)"));

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ActionEditorDialog(_item.Instance);
            editor.SetItems(_item.Value as ObservableCollection<XmlAction> ?? new ObservableCollection<XmlAction>());
            if (editor.ShowDialog() == true)
            {
                _item.Value = editor.GetItems();
            }
            ActionInfo = GetText();
            ActionToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            ActionInfo = GetText();
            ActionToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            var list = _item?.Value as IList;
            if (list != null)
            {
                return list.Count > 0 ? "(Collection)" : "(Empty)";
            }
            return "(Empty)";
        }

        private string GetToolTipText()
        {
            if (!(_item?.Value is IList) || ((IList) _item.Value).Count <= 0) return "(Empty)";
            var returnValue = "Actions:" + Environment.NewLine;
            return ((IList) _item.Value).OfType<XmlAction>().Aggregate(returnValue, (current, xmlAction) => current + xmlAction.DisplayName + Environment.NewLine);
        }
    }


}
