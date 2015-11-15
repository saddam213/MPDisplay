using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GUISkinFramework.ExtensionMethods
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the specified string is a number.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is a number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumber(this string str)
        {
            int value;
            return int.TryParse(str, out value);
        }

        public static bool IsDouble(this string str)
        {
            double value;
            return double.TryParse(str, out value);
        }

        /// <summary>
        /// Converts the string value to Thickness property.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns></returns>
        public static Thickness ToThickness(this string value)
        {
            if (string.IsNullOrEmpty(value)) return new Thickness(0);
            if (value.Contains(','))
            {
                var values = value.Split(',');
                if (values.Length == 4 && values.All(s => s.IsNumber()))
                {
                    return new Thickness(double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3]));
                }
            }
            else if (IsNumber(value))
            {
                return new Thickness(double.Parse(value));
            }
            return new Thickness(0);
        }

        /// <summary>
        /// Converts the string value to CornerRadius property.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns></returns>
        public static CornerRadius ToCornerRadius(this string value)
        {
            if (string.IsNullOrEmpty(value)) return new CornerRadius(0);

            if (value.Contains(','))
            {
                var values = value.Split(',');
                if (values.Length == 4 && values.All(s => s.IsNumber()))
                {
                    return new CornerRadius(double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3]));
                }
            }
            else if (IsNumber(value))
            {
                return new CornerRadius(double.Parse(value));
            }
            return new CornerRadius(0);
        }

        public static Color ToColor(this string color)
        {
            var getcolor = Colors.Transparent;
            try
            {
                var convertFromString = ColorConverter.ConvertFromString(color);
                if (convertFromString != null)
                    getcolor = (Color)convertFromString;
            }
            catch
            {
                // ignored
            }
            return getcolor;
        }

        public static Point ToPoint(this string value)
        {
            var convertFromInvariantString = new PointConverter().ConvertFromInvariantString(value);
            if (convertFromInvariantString != null)
                return (Point)convertFromInvariantString;
            return new Point();
        }

        public static string ToXmlString(this Point value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
