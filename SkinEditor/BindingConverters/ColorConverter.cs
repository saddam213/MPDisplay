using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SkinEditor.BindingConverters
{

    public class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(value.ToString());
                    if (targetType == typeof(Brush))
                    {
                        return new SolidColorBrush(color);
                    }
                    return color;
                }
                catch { }

                if (targetType == typeof(Brush))
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

