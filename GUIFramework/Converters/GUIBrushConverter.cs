using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GUIFramework.Managers;
using GUISkinFramework.ExtensionMethods;
using GUISkinFramework.Skin;

namespace GUIFramework.Converters
{
    /// <summary>
    /// Converts an XmlBrush to a Brush
    /// </summary>
    public class GUIBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
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
            var imageBrush = value as XmlImageBrush;
            if (imageBrush != null)
            {
                return GUIImageManager.GetSkinImage(imageBrush);
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
