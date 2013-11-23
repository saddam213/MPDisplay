using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Styles;
using GUISkinFramework;
using GUIFramework.Managers;
using System.Windows;

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
            if (value is XmlColorBrush)
            {
                return new SolidColorBrush((value as XmlColorBrush).Color.ToColor());
            }
            else if (value is XmlGradientBrush)
            {
                var background = value as XmlGradientBrush;
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
                    default:
                        break;
                }

                foreach (var stop in background.GradientStops)
                {
                    gradient.GradientStops.Add(new GradientStop(stop.Color.ToColor(), stop.Offset));
                }
                return gradient;
            }
            else if (value is XmlImageBrush)
            {
                return GUIImageManager.GetSkinImage(value as XmlImageBrush);
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
