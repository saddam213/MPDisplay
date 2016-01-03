using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace MPDisplay.Common.Controls.Core
{
    static class ColorUtilities
    {
        public static readonly Dictionary<string, Color> KnownColors = GetKnownColors();
        private const double Tolerance = 0.001;

        public static string GetColorName(this Color color)
        {
            var colorName = KnownColors.Where(kvp => kvp.Value.Equals(color)).Select(kvp => kvp.Key).FirstOrDefault();

            if (String.IsNullOrEmpty(colorName))
                colorName = color.ToString();

            return colorName;
        }

        private static Dictionary<string, Color> GetKnownColors()
        {
            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            return colorProperties.ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));
        }

        /// <summary>
        /// Converts an RGB color to an HSV color.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static HsvColor ConvertRgbToHsv(int r, int b, int g)
        {
            double h = 0, s;

            double min = Math.Min(Math.Min(r, g), b);
            double v = Math.Max(Math.Max(r, g), b);
            var delta = v - min;

            if (Math.Abs(v) < Tolerance)
            {
                s = 0;
            }
            else
                s = delta / v;

            if (Math.Abs(s) < Tolerance)
                h = 0.0;

            else
            {
                if (Math.Abs(r - v) < Tolerance)
                    h = (g - b) / delta;
                else if (Math.Abs(g - v) < Tolerance)
                    h = 2 + (b - r) / delta;
                else if (Math.Abs(b - v) < Tolerance)
                    h = 4 + (r - g) / delta;

                h *= 60;
                if (h < 0.0)
                    h = h + 360;

            }

            return new HsvColor { H = h, S = s, V = v / 255 };
        }

        /// <summary>
        ///  Converts an HSV color to an RGB color.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color ConvertHsvToRgb(double h, double s, double v)
        {
            double r, g, b;

            if (Math.Abs(s) < Tolerance)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                if (Math.Abs(h - 360) < Tolerance)
                    h = 0;
                else
                    h = h / 60;

                var i = (int)Math.Truncate(h);
                var f = h - i;

                var p = v * (1.0 - s);
                var q = v * (1.0 - s * f);
                var t = v * (1.0 - s * (1.0 - f));

                switch (i)
                {
                    case 0:
                        {
                            r = v;
                            g = t;
                            b = p;
                            break;
                        }
                    case 1:
                        {
                            r = q;
                            g = v;
                            b = p;
                            break;
                        }
                    case 2:
                        {
                            r = p;
                            g = v;
                            b = t;
                            break;
                        }
                    case 3:
                        {
                            r = p;
                            g = q;
                            b = v;
                            break;
                        }
                    case 4:
                        {
                            r = t;
                            g = p;
                            b = v;
                            break;
                        }
                    default:
                        {
                            r = v;
                            g = p;
                            b = q;
                            break;
                        }
                }

            }

            return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        /// <summary>
        /// Generates a list of colors with hues ranging from 0 360 and a saturation and value of 1. 
        /// </summary>
        /// <returns></returns>
        public static List<Color> GenerateHsvSpectrum()
        {
            var colorsList = new List<Color>(8);

            for (var i = 0; i < 29; i++)
            {
                colorsList.Add(ConvertHsvToRgb(i * 12, 1, 1));
            }

            colorsList.Add(ConvertHsvToRgb(0, 1, 1));

            return colorsList;
        }
    }
}
