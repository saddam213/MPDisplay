using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GUISkinFramework.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = (bool)value;
            if (parameter is string && parameter.ToString() == "!")
            {
                return visible ? Visibility.Collapsed : Visibility.Visible;
            }
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = (Visibility)value;
            if (parameter is string && parameter.ToString() == "!")
            {
                return !(visible == Visibility.Visible);
            }
            return visible == Visibility.Visible;
        }

        #endregion
    }
}
