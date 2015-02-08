﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GUISkinFramework;
using System.Windows;

namespace GUIFramework.Converters
{
  
    public class ProgressLabelConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness margin = new Thickness(0);
            if (values != null && values.Count() == 4)
            {
                try
                {
                    double percentage = (double)values[1];
                    var width = values[0] != null ? double.Parse(values[0].ToString()) : 0.0;
                    var barmargin = values[2] != null ? values[2].ToString().ToThickness() : new Thickness(0);
                    margin = values[3] != null ? values[3].ToString().ToThickness() : new Thickness(0);

                    var actualWidth = (width - (barmargin.Left + barmargin.Right));
                    margin.Left += Math.Min(actualWidth, Math.Max(0.0, (actualWidth / 100.0) * percentage));
                }
                catch { }
            }
            return margin;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
