using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GUIFramework.Managers;
using MessageFramework.DataObjects;

namespace GUIFramework.Converters
{
    /// <summary>
    /// Converts APIImage to BitmapImage
    /// </summary>
    public class GUIImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var apiImage = value as APIImage;
            if (apiImage == null) return new BitmapImage();

            var image = apiImage;
            return image.IsFile 
                ? GUIImageManager.GetImage(image.FileName)
                : GUIImageManager.GetImage(image.FileBytes);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
