using System;
using System.Globalization;
using System.Windows.Data;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls;

namespace GUISkinFramework.Converters
{
    public class XmlEQStyleEnumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is XmlEQStyle)
            {
                return (EQStyle)((int)(XmlEQStyle)value);
            }
            return EQStyle.SingleBar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
