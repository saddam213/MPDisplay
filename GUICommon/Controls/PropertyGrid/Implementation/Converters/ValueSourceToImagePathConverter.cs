using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class ValueSourceToImagePathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bvs = (BaseValueSource)value;

            const string uriPrefix = "/GUISkinFramework;component/Editor/WPFControls/PropertyGrid/Images/";
            var imageName = "AdvancedProperties11";

            switch (bvs)
            {
                case BaseValueSource.Inherited:
                case BaseValueSource.DefaultStyle:
                case BaseValueSource.ImplicitStyleReference:
                    imageName = "Inheritance11";
                    break;
                case BaseValueSource.DefaultStyleTrigger:                    
                    break;
                case BaseValueSource.Style:
                    imageName = "Style11";
                    break;

                case BaseValueSource.Local:
                    imageName = "Local11";
                    break;
            }


            return new BitmapImage(new Uri($"{uriPrefix}{imageName}.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
