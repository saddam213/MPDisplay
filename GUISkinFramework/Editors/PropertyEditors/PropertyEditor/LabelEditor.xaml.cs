using System;
using System.Windows;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class LabelEditor : ITypeEditor
    {
    
        private PropertyItem _item;

        public LabelEditor()
        {
            InitializeComponent();
          
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(LabelEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string LabelInfo
        {
            get { return (string)GetValue(LabelInfoProperty); }
            set { SetValue(LabelInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelInfoProperty =
            DependencyProperty.Register("LabelInfo", typeof(string), typeof(LabelEditor), new PropertyMetadata("(Empty)"));

        public string LabelToolTip

        {
            get { return (string)GetValue(LabelToolTipProperty); }
            set { SetValue(LabelToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelToolTipProperty =
            DependencyProperty.Register("LabelToolTip", typeof(string), typeof(LabelEditor), new PropertyMetadata("(Empty)"));

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var editor = new LabelEditorDialog(_item.Instance, _item.PropertyGrid.Tag as XmlSkinInfo)
            {
                CurrentLabel = _item.Value.ToString()
            };
            if (editor.ShowDialog() == true)
            {
                Value = editor.CurrentLabel;
                _item.Value = editor.CurrentLabel;
            }
            LabelInfo = GetText();
            LabelToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            LabelInfo = GetText();
            LabelToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (_item != null && _item.Value is string)
            {
                return !string.IsNullOrEmpty(_item.Value.ToString()) ? "(Label)" : "(Empty)";
            }
            return "(Empty)";
        }

        private string GetToolTipText()
        {
            if (_item != null && _item.Value is string && !string.IsNullOrEmpty(_item.Value.ToString()))
            {
                return string.Format("Visible Condition(s):{0}{1}",Environment.NewLine, _item.Value);
            }
            return "(Empty)";
        }
    }


}
