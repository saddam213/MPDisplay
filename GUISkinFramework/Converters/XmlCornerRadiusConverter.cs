using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GUISkinFramework.Converters
{
    public class XmlCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue;
            var pointValue = value?.ToString() ?? string.Empty;
            if (!pointValue.Contains(','))
                return int.TryParse(pointValue, out intValue) ? new CornerRadius(intValue) : new CornerRadius(0);
            var points = pointValue.Split(',');
            if (points.Length == 4 && points.All(v => int.TryParse(v, out intValue)))
            {
                return new CornerRadius(double.Parse(points[0]), double.Parse(points[1]), double.Parse(points[2]), double.Parse(points[3]));
            }

            return int.TryParse(pointValue, out intValue) ? new CornerRadius(intValue) : new CornerRadius(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
