using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common.Controls.PropertyGrid.Editors;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class ImageEditor : UserControl, ITypeEditor
    {
    
        private PropertyItem _Item;

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
            var editor = new ImageEditorDialog(_Item.Instance, _Item.PropertyGrid.Tag as XmlSkinInfo);
            editor.CurrentLabel = _Item.Value.ToString();
            if (editor.ShowDialog() == true)
            {
                Value = editor.CurrentLabel;
                _Item.Value = editor.CurrentLabel;
            }
            ImageInfo = GetText();
            ImageToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            ImageInfo = GetText();
            ImageToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (_Item != null && _Item.Value is string)
            {
                string val = _Item.Value.ToString();
                return string.IsNullOrEmpty(val) ? "(Empty)"
                    : val.Contains("+") ? "(Custom)"
                    : val.StartsWith("#") ? "(Property)"
                    : "(Image)";
            }
            return "(Empty)";
        }

        private string GetToolTipText()
        {
            if (_Item != null && _Item.Value is string && !string.IsNullOrEmpty(_Item.Value.ToString()))
            {
                return _Item.Value.ToString();
            }
            return "(Empty)";
        }
    }


}
