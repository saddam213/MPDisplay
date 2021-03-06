﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GUISkinFramework.ExtensionMethods;

namespace SkinEditor.BindingConverters
{
    public class SkinEditorProgressLabelConverter : IMultiValueConverter
    {

        // ReSharper disable once FunctionComplexityOverflow
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var margin = new Thickness(0);

            if (values == null || values.Length != 5) return margin;

            var width = values[0] != null ? double.Parse(values[0].ToString()) : 0.0;
            var property = values[1]?.ToString() ?? "0.0";
            var percentage = 0.0;
            if (property.StartsWith("#"))
            {
                var prop = SkinEditorInfoManager.SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == property);
                if (prop != null)
                {
                    double.TryParse(prop.DesignerValue, out percentage);
                }
            }
            else
            {
                double.TryParse(property, out percentage);
            }

            try
            {
                var barmargin = values[2]?.ToString().ToThickness() ?? new Thickness(0);
                margin = values[3]?.ToString().ToThickness() ?? new Thickness(0);
                var labelwidth = values[4] != null ? double.Parse(values[4].ToString()) : 0.0;

                var actualWidth = width - (barmargin.Left + barmargin.Right);
                margin.Left += Math.Min(actualWidth, Math.Max(0.0, actualWidth / 100.0 * percentage));
                margin.Left += barmargin.Left;
                if (margin.Left < 0.0) margin.Left = 0.0;
                if (margin.Left > width - labelwidth) margin.Left = width - labelwidth;
            }
            catch
            {
                // ignored
            }
            return margin;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
