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
            int value = 0;
            return int.TryParse(str, out value);
        }

        public static bool IsDouble(this string str)
        {
            double value = 0;
            return double.TryParse(str, out value);
        }

        /// <summary>
        /// Converts the string value to Thickness property.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns></returns>
        public static Thickness ToThickness(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains(','))
                {
                    var values = value.Split(',');
                    if (values.Count() == 4 && !values.Any(s => !s.IsNumber()))
                    {
                        return new Thickness(double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3]));
                    }
                }
                else if (IsNumber(value))
                {
                    return new Thickness(double.Parse(value));
                }
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
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains(','))
                {
                    var values = value.Split(',');
                    if (values.Count() == 4 && !values.Any(s => !s.IsNumber()))
                    {
                        return new CornerRadius(double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]), double.Parse(values[3]));
                    }
                }
                else if (IsNumber(value))
                {
                    return new CornerRadius(double.Parse(value));
                }
            }
            return new CornerRadius(0);
        }

        public static Color ToColor(this string color)
        {
            var getcolor = Colors.Transparent;
            try
            {
                getcolor = (Color)ColorConverter.ConvertFromString(color);
            }
            catch { }
            return getcolor;
        }

        public static Point ToPoint(this string value)
        {
            return (Point)new PointConverter().ConvertFromInvariantString(value);
        }

        public static string ToXmlString(this Point value)
        {

            if (value != null)
            {
                return value.ToString(CultureInfo.CurrentCulture);// string.Format("{0},{1}", value.X, value.Y);
            }
            return new Point().ToString(CultureInfo.CurrentCulture);
        }
    }
}
