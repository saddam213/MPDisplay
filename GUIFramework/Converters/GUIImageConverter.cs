using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Styles;
using GUISkinFramework;
using GUIFramework.Managers;
using System.Windows;
using MessageFramework.DataObjects;

namespace GUIFramework.Converters
{
    public class GUIImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is APIImage)
            {
                var image = (value as APIImage);
                return image.IsFile 
                    ? GUIImageManager.GetImage(image.FileName)
                    : GUIImageManager.GetImage(image.FileBytes);
            }

            return new BitmapImage();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
