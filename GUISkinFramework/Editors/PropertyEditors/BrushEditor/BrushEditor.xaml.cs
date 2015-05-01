using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : UserControl, ITypeEditor
    {
    
        private PropertyItem _Item;

        public BrushEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(XmlBrush), typeof(BrushEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public XmlBrush Value
        {
            get { return (XmlBrush)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string BrushTypeInfo
        {
            get { return (string)GetValue(BrushTypeInfoProperty); }
            set { SetValue(BrushTypeInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrushTypeInfoProperty =
            DependencyProperty.Register("BrushTypeInfo", typeof(string), typeof(BrushEditor), new PropertyMetadata("(Empty)"));

        public string BrushToolTip
        {
            get { return (string)GetValue(BrushToolTipProperty); }
            set { SetValue(BrushToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrushTypeInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrushToolTipProperty =
            DependencyProperty.Register("BrushToolTip", typeof(string), typeof(BrushEditor), new PropertyMetadata("(Empty)"));



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_Item != null)
            {
                var brushEditor = new BrushEditorDialog(Value, _Item.PropertyGrid.Tag as XmlSkinInfo);
                if (brushEditor.ShowDialog() == true)
                {
                    Value = brushEditor.NewValue;
                    _Item.Value = Value;
                }
            }
            BrushTypeInfo = GetText();
            BrushToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            if (_Item != null)
            {
                Value = _Item.Value != null ? _Item.Value as XmlBrush : new XmlBrush();
            }
            BrushTypeInfo = GetText();
            BrushToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (_Item != null)
            {
                if (_Item.Value is XmlBrush && !string.IsNullOrEmpty((_Item.Value as XmlBrush).StyleId))
                {
                    return (_Item.Value as XmlBrush).StyleId.ToString();
                }

                if (_Item.Value is XmlColorBrush)
                {
                    if ((_Item.Value as XmlColorBrush).Color == "Transparent")
                    {
                        return "(Empty)";
                    }
                    return "(Color)";
                }

                if (_Item.Value is XmlGradientBrush)
                {
                    return "(Gradient)";
                }

                if (_Item.Value is XmlImageBrush)
                {
                    return "(Image)";
                }
            }
            return "(Empty)";
        }

        private string GetToolTipText()
        {
            if (_Item != null)
            {
                if (_Item.Value is XmlColorBrush)
                {
                    var brush = _Item.Value as XmlColorBrush;
                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(brush.StyleId))
                    {
                        sb.AppendLine(string.Format("Style: {0}", brush.StyleId));
                        sb.AppendLine("-----------------------");
                    }
                    sb.AppendLine(string.Format("Color: {0}", (_Item.Value as XmlColorBrush).Color));
                    return sb.ToString();
                }

                if (_Item.Value is XmlGradientBrush)
                {
                    var brush = _Item.Value as XmlGradientBrush;
                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(brush.StyleId))
                    {
                        sb.AppendLine(string.Format("Style: {0}", brush.StyleId));
                        sb.AppendLine("-----------------------");
                    }
                    sb.AppendLine("Gradient:");
                    sb.AppendLine(string.Format("Angle: {0}", brush.Angle));
                  //  sb.AppendLine(string.Format("EndPoint: {0}", brush.EndPoint));
                    int count = 1;
                    foreach (var stop in brush.GradientStops)
                    {
                        sb.AppendLine(string.Format("Color{0}: {1}, Offset: {2}", count, stop.Color, stop.Offset));
                        count++;
                    }
                    return sb.ToString();
                }

                if (_Item.Value is XmlImageBrush)
                {
                    var brush = _Item.Value as XmlImageBrush;
                    StringBuilder sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(brush.StyleId))
                    {
                        sb.AppendLine(string.Format("Style: {0}", brush.StyleId));
                        sb.AppendLine("-----------------------");
                    }
                    sb.AppendLine("Image:");
                    sb.AppendLine(string.Format("Filename: {0}", brush.ImageName));
                    sb.AppendLine(string.Format("Stretch: {0}", brush.ImageStretch));
                }
            }
            return "(Empty)";
        }


    }
}
