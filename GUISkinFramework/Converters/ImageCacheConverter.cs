using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GUISkinFramework.Converters
{
    public class ImageCacheConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            int width = 0;
            var path = (string)value;
            // load the image, specify CacheOption so the file is not locked
            var image = new BitmapImage();
            try
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                if (parameter != null && int.TryParse(parameter.ToString(), out width))
                {
                    image.DecodePixelWidth = width;
                }
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(path);
                image.EndInit();

                return image;
            }
            catch
            {
                return new BitmapImage();
            }

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Not implemented.");
        }
    } 

}
