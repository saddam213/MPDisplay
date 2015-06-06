using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SkinEditor.BindingConverters
{

    public class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return Colors.Transparent;

            try
            {
                var convertFromString = ColorConverter.ConvertFromString(value.ToString());
                if (convertFromString != null)
                {
                    var color = (Color)convertFromString;
                    if (targetType == typeof(Brush))
                    {
                        return new SolidColorBrush(color);
                    }
                    return color;
                }
            }
            catch
            {
                // ignored
            }

            if (targetType == typeof(Brush))
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

