using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GUISkinFramework;
using GUISkinFramework.Controls;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Windows;

namespace SkinEditor.BindingConverters
{
    public class TypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new BitmapImage(new Uri(string.Format(@"/Images/{0}.png", value.GetType().Name), UriKind.Relative));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class TypeToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is IXmlControlHost)
                {
                    return FontWeights.Bold;
                }
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
