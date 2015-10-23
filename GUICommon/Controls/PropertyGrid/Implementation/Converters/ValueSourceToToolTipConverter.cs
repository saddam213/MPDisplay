using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class ValueSourceToToolTipConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bvs = (BaseValueSource)value;
            var toolTip = "Advanced Properties";

            switch (bvs)
            {
                case BaseValueSource.Inherited:
                case BaseValueSource.DefaultStyle:
                case BaseValueSource.ImplicitStyleReference:
                    toolTip = "Inheritance";
                    break;
                case BaseValueSource.Style:
                    toolTip = "Style Setter";
                    break;

                case BaseValueSource.Local:
                    toolTip = "Local";
                    break;
            }

            return toolTip;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
