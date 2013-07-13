using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GUISkinFramework.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = (bool)value;
            if (parameter is string && parameter.ToString() == "!")
            {
                return visible ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
            return visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Visibility visible = (System.Windows.Visibility)value;
            if (parameter is string && parameter.ToString() == "!")
            {
                return !(visible == System.Windows.Visibility.Visible);
            }
            return visible == System.Windows.Visibility.Visible;
        }

        #endregion
    }
}
