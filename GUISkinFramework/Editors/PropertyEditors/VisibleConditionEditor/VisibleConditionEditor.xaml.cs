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
    public partial class VisibleConditionEditor : UserControl, ITypeEditor
    {
    
        private PropertyItem _Item;

        public VisibleConditionEditor()
        {
            InitializeComponent();
          
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(VisibleConditionEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string VisibleConditionInfo
        {
            get { return (string)GetValue(VisibleConditionInfoProperty); }
            set { SetValue(VisibleConditionInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleConditionInfoProperty =
            DependencyProperty.Register("VisibleConditionInfo", typeof(string), typeof(VisibleConditionEditor), new PropertyMetadata("(Empty)"));

        public string VisibleConditionToolTip

        {
            get { return (string)GetValue(VisibleConditionToolTipProperty); }
            set { SetValue(VisibleConditionToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleConditionToolTipProperty =
            DependencyProperty.Register("VisibleConditionToolTip", typeof(string), typeof(VisibleConditionEditor), new PropertyMetadata("(Empty)"));

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var editor = new VisibleConditionEditorDialog(_Item.Instance, _Item.PropertyGrid.Tag as XmlSkinInfo);
            editor.CurrentCondition = _Item.Value.ToString();
            if (editor.ShowDialog() == true)
            {
                Value = editor.CurrentCondition;
                _Item.Value = editor.CurrentCondition;
            }
            VisibleConditionInfo = GetText();
            VisibleConditionToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            VisibleConditionInfo = GetText();
            VisibleConditionToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (_Item != null && _Item.Value is string)
            {
                return !string.IsNullOrEmpty(_Item.Value.ToString()) ? "(Visible Condition)" : "(Empty)";
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
