using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GUISkinFramework;
using System.Windows;

namespace GUIFramework.Converters
{
  
    public class ProgressValueConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count() == 3)
            {
                try
                {
                    double percentage = (double)values[1];
                    var width = values[0] != null ? double.Parse(values[0].ToString()) : 0.0;
                    var margin = values[2] != null ? values[2].ToString().ToThickness() : new Thickness(0);
                    var actualWidth = (width - (margin.Left + margin.Right));
                    return Math.Min(actualWidth, Math.Max(0.0, (actualWidth / 100.0) * percentage));
                }
                catch { }
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
