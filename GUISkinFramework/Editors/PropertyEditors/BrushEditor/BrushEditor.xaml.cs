using System.Text;
using System.Windows;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : ITypeEditor
    {
    
        private PropertyItem _item;

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
            if (_item != null)
            {
                var brushEditor = new BrushEditorDialog(Value, _item.PropertyGrid.Tag as XmlSkinInfo);
                if (brushEditor.ShowDialog() == true)
                {
                    Value = brushEditor.NewValue;
                    _item.Value = Value;
                }
            }
            BrushTypeInfo = GetText();
            BrushToolTip = GetToolTipText();
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            if (_item != null)
            {
                Value = _item.Value != null ? _item.Value as XmlBrush : new XmlBrush();
            }
            BrushTypeInfo = GetText();
            BrushToolTip = GetToolTipText();
            return this;
        }

        private string GetText()
        {
            if (_item == null) return "(Empty)";
            var brush = _item.Value as XmlBrush;
            if (brush != null && !string.IsNullOrEmpty(brush.StyleId))
            {
                return brush.StyleId;
            }

            if (_item.Value is XmlColorBrush)
            {
                return (_item.Value as XmlColorBrush).Color == "Transparent" ? "(Empty)" : "(Color)";
            }

            if (_item.Value is XmlGradientBrush)
            {
                return "(Gradient)";
            }

            if (_item.Value is XmlImageBrush)
            {
                return "(Image)";
            }
            return "(Empty)";
        }

        private string GetToolTipText()
        {
            if (_item == null) return "(Empty)";
            var colorBrush = _item.Value as XmlColorBrush;
            if (colorBrush != null)
            {
                var brush = colorBrush;
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(brush.StyleId))
                {
                    sb.AppendLine(string.Format("Style: {0}", brush.StyleId));
                    sb.AppendLine("-----------------------");
                }
                sb.AppendLine(string.Format("Color: {0}", colorBrush.Color));
                return sb.ToString();
            }

            var gradientBrush = _item.Value as XmlGradientBrush;
            if (gradientBrush != null)
            {
                var brush = gradientBrush;
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(brush.StyleId))
                {
                    sb.AppendLine(string.Format("Style: {0}", brush.StyleId));
                    sb.AppendLine("-----------------------");
                }
                sb.AppendLine("Gradient:");
                sb.AppendLine(string.Format("Angle: {0}", brush.Angle));
                //  sb.AppendLine(string.Format("EndPoint: {0}", brush.EndPoint));
                var count = 1;
                foreach (var stop in brush.GradientStops)
                {
                    sb.AppendLine(string.Format("Color{0}: {1}, Offset: {2}", count, stop.Color, stop.Offset));
                    count++;
                }
                return sb.ToString();
            }

            var imageBrush = _item.Value as XmlImageBrush;
            if (imageBrush == null) return "(Empty)";

            var brush1 = imageBrush;
            var sb1 = new StringBuilder();
            if (!string.IsNullOrEmpty(brush1.StyleId))
            {
                sb1.AppendLine(string.Format("Style: {0}", brush1.StyleId));
                sb1.AppendLine("-----------------------");
            }
            sb1.AppendLine("Image:");
            sb1.AppendLine(string.Format("Filename: {0}", brush1.ImageName));
            sb1.AppendLine(string.Format("Stretch: {0}", brush1.ImageStretch));
            return "(Empty)";
        }


    }
}
