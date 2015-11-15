using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SkinEditor.BindingConverters
{
    public class SkinEditorXmlImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return null;

            var valueImage = value.ToString();              

            if (valueImage.Contains('+'))
            {
                var path = string.Empty;
                foreach (var item in valueImage.Split('+'))
                {
                    if (item.StartsWith("@"))
                    {
                        path += SkinEditorInfoManager.GetLanguageValue(item);
                        continue;
                    }

                    if (item.StartsWith("#"))
                    {
                        var prop = SkinEditorInfoManager.SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == item);
                        if (prop != null)
                        {
                            path += prop.DesignerValue;
                            continue;
                        }
                    }
                    path += item;
                }
                return path;
            }
            if (valueImage.StartsWith("#"))
            {
                var prop = SkinEditorInfoManager.SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == valueImage);
                if (prop != null)
                {
                    return prop.DesignerValue;
                }
            }

            var image = SkinEditorInfoManager.SkinInfo.Images.FirstOrDefault(i => i.XmlName.Equals(valueImage, StringComparison.OrdinalIgnoreCase) 
                                                                                  || i.FileName.Equals(valueImage, StringComparison.OrdinalIgnoreCase));
            return image != null ? image.FileName : valueImage;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
