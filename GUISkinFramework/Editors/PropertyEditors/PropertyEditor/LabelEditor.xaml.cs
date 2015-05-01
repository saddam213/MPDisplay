using System;
using System.Windows;
using System.Windows.Controls;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class LabelEditor : UserControl, ITypeEditor
    {
    
        private PropertyItem _Item;

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
            var editor = new LabelEditorDialog(_Item.Instance, _Item.PropertyGrid.Tag as XmlSkinInfo);
            editor.CurrentLabel = _Item.Value.ToString();
            if (editor.ShowDialog() == true)
            {
                Value = editor.CurrentLabel;
                _Item.Value = editor.CurrentLabel;
            }
            LabelInfo = GetText();
            LabelToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            LabelInfo = GetText();
            LabelToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (_Item != null && _Item.Value is string)
            {
                return !string.IsNullOrEmpty(_Item.Value.ToString()) ? "(Label)" : "(Empty)";
            }
            return "(Empty)";
        }

        private string GetToolTipText()
        {
            if (_Item != null && _Item.Value is string && !string.IsNullOrEmpty(_Item.Value.ToString()))
            {
                return string.Format("Visible Condition(s):{0}{1}",Environment.NewLine, _Item.Value.ToString());
            }
            return "(Empty)";
        }
    }


}
