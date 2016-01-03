using System;
using System.Globalization;
using System.Windows.Data;

namespace GUIConfig.Settings
{
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = parameter?.ToString() ?? string.Empty;
            return LanguageHelper.GetLanguageValue(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
