using System;
using System.Globalization;
using System.Windows.Data;

namespace GUIFramework.Converters
{
    public class CoverFlowGreaterThanSelectedIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var selectedIndex = int.Parse(values[0].ToString());
                var itemIndex = int.Parse(values[1].ToString());
                return itemIndex > selectedIndex;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
