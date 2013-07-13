using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GUIFramework.Converters
{
    public class CoverFlowGreaterThanSelectedIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var selectedIndex = int.Parse(values[0].ToString());
                var itemIndex = int.Parse(values[1].ToString());
                return itemIndex > selectedIndex;
            }
            catch { }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
