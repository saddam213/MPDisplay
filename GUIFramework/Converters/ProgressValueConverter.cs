using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GUISkinFramework.ExtensionMethods;

namespace GUIFramework.Converters
{
  
    public class ProgressValueConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3) return 0.0;

            try
            {
                var percentage = (double)values[1];
                var width = values[0] != null ? double.Parse(values[0].ToString()) : 0.0;
                var margin = values[2]?.ToString().ToThickness() ?? new Thickness(0);
                var actualWidth = (width - (margin.Left + margin.Right));
                if (actualWidth < 0.0) actualWidth = 0.0;
                return Math.Min(actualWidth, Math.Max(0.0, (actualWidth / 100.0) * percentage));
            }
            catch
            {
                // ignored
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
