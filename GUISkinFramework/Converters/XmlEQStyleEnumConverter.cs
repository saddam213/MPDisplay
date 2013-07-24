using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GUISkinFramework.Controls;
using MPDisplay.Common.Controls;

namespace GUISkinFramework.Converters
{
    public class XmlEQStyleEnumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is XmlEQStyle)
            {
                return (EQStyle)((int)(XmlEQStyle)value);
            }
            return EQStyle.SingleBar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
