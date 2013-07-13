using System;
using System.Windows.Data;
using System.Windows.Media;

namespace MPDisplay.Common.BindingConverters
{
    public class ColorToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a Color to a SolidColorBrush.
        /// </summary>
        /// <param name="value">The Color produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted SolidColorBrush. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value is Color)
                {
                    return ((Color)value).ToString();
                }
                if (value is string)
                {
                    var getcolor = Colors.Transparent;
                    try
                    {
                        getcolor = (Color)ColorConverter.ConvertFromString(value.ToString());
                    }
                    catch { }
                    return getcolor;
                }
            }

            return value;
        }


        /// <summary>
        /// Converts a SolidColorBrush to a Color.
        /// </summary>
        /// <remarks>Currently not used in toolkit, but provided for developer use in their own projects</remarks>
        /// <param name="value">The SolidColorBrush that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value is string)
                {
                    var getcolor = Colors.Transparent;
                    try
                    {
                        getcolor = (Color)ColorConverter.ConvertFromString(value.ToString());
                    }
                    catch { }
                    return getcolor;
                }
                if (value is Color)
                {
                    value.ToString();
                }
            }

            return value;
        }

        #endregion
    }
}
