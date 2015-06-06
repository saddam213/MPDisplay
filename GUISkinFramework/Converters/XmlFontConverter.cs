using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GUISkinFramework.Converters
{
    public class XmlFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return null;
            if (targetType == typeof(FontWeight))
            {
                return (FontWeight)(new FontWeightConverter().ConvertFromString(value.ToString()) ?? FontWeights.Normal);
            }

            if (targetType == typeof(FontFamily))
            {
                return (FontFamily)new FontFamilyConverter().ConvertFromString(value.ToString());
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
