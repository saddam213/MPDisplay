using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GUISkinFramework.Common;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Converters
{
    public class XmlFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (targetType == typeof(FontWeight))
                {
                    return (FontWeight)(new FontWeightConverter().ConvertFromString(value.ToString()) ?? FontWeights.Normal);
                }

                if (targetType == typeof(FontFamily))
                {
                    return (FontFamily)new FontFamilyConverter().ConvertFromString(value.ToString());
                }
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
