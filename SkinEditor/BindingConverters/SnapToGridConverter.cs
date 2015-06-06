using System;
using System.Globalization;
using System.Windows.Data;

namespace SkinEditor.BindingConverters
{
    public class SnapToGridConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = (double)values[2];

            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
