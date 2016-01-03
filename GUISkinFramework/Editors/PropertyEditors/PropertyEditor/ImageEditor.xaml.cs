using System.Windows;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class ImageEditor : ITypeEditor
    {
    
        private PropertyItem _item;

        public ImageEditor()
        {
            InitializeComponent();
          
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(ImageEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string ImageInfo
        {
            get { return (string)GetValue(ImageInfoProperty); }
            set { SetValue(ImageInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageInfoProperty =
            DependencyProperty.Register("ImageInfo", typeof(string), typeof(ImageEditor), new PropertyMetadata("(Empty)"));

        public string ImageToolTip
        {
            get { return (string)GetValue(ImageToolTipProperty); }
            set { SetValue(ImageToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageToolTipProperty =
            DependencyProperty.Register("ImageToolTip", typeof(string), typeof(ImageEditor), new PropertyMetadata(null));

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ImageEditorDialog(_item.Instance, _item.PropertyGrid.Tag as XmlSkinInfo)
            {
                CurrentLabel = _item.Value.ToString()
            };
            if (editor.ShowDialog() == true)
            {
                Value = editor.CurrentLabel;
                _item.Value = editor.CurrentLabel;
            }
            ImageInfo = GetText();
            ImageToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            ImageInfo = GetText();
            ImageToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (!(_item?.Value is string)) return "(Empty)";

            var val = _item.Value.ToString();
            return string.IsNullOrEmpty(val) ? "(Empty)"
                : val.Contains("+") ? "(Custom)"
                    : val.StartsWith("#") ? "(Property)"
                        : "(Image)";
        }

        private string GetToolTipText()
        {
            if (_item?.Value is string && !string.IsNullOrEmpty(_item.Value.ToString()))
            {
                return _item.Value.ToString();
            }
            return "(Empty)";
        }
    }


}
