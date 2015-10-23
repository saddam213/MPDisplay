using System;
using System.Globalization;
using System.Windows.Data;

namespace MPDisplay.Common.BindingConverters
{
    public class ObjectTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.GetType().Name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
