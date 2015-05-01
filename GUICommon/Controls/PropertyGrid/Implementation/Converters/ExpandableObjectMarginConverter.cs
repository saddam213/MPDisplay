using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class ExpandableObjectMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int childLevel = (int)value;
            return new Thickness(childLevel * 15, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
