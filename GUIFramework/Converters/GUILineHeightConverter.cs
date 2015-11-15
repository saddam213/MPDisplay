using System;
using System.Globalization;
using System.Windows.Data;

namespace GUIFramework.Converters
{
    public class GUILineHeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var height = (double)value;
            if (height < 0.0034) height = double.NaN;
            if (height > 160000.0) height = 160000.0;
            return height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}