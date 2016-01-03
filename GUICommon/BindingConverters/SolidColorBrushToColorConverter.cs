using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MPDisplay.Common.BindingConverters
{
    public class SolidColorBrushToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            return brush?.Color ?? default(Color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return default(SolidColorBrush);

            var color = (Color)value;
            return new SolidColorBrush(color);
        }

        #endregion
    }
}
