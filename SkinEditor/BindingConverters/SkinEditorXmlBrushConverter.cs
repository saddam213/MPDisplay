using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUISkinFramework.ExtensionMethods;
using GUISkinFramework.Skin;

namespace SkinEditor.BindingConverters
{
    public class SkinEditorXmlBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as XmlColorBrush;
            if (brush != null)
            {
                return new SolidColorBrush(brush.Color.ToColor());
            }
            var gradientBrush = value as XmlGradientBrush;
            if (gradientBrush != null)
            {
                var background = gradientBrush;
                var gradient = new LinearGradientBrush();
                switch (background.Angle)
                {
                    case XmlGradientAngle.Vertical:
                        gradient.StartPoint = new Point(0.5, 0);
                        gradient.EndPoint = new Point(0.5, 1);
                        break;
                    case XmlGradientAngle.Horizontal:
                        gradient.StartPoint = new Point(0, 0.5);
                        gradient.EndPoint = new Point(1, 0.5);
                        break;
                }
            
                foreach (var stop in background.GradientStops)
                {
                    gradient.GradientStops.Add(new GradientStop(stop.Color.ToColor(), stop.Offset));
                }
                return gradient;
            }
            else
            {
                var imageBrush = value as XmlImageBrush;
                if (imageBrush == null) return null;

                var background = imageBrush;
                var imageName = SkinEditorInfoManager.SkinInfo.Images.FirstOrDefault(i => i.XmlName.Equals(background.ImageName));
                if (imageName != null && !string.IsNullOrEmpty(imageName.FileName))
                {
                    return new ImageBrush(new BitmapImage(new Uri(imageName.FileName, UriKind.RelativeOrAbsolute))) { Stretch = background.ImageStretch };
                }
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
