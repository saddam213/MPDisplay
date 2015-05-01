﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace SkinEditor.BindingConverters
{
    public class SnapToGridConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSnapToGrid = (bool)values[0];
            int gridSize = (int)values[1];
            double value = (double)values[2];

            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
