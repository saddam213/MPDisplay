using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GUISkinFramework.Common;

namespace SkinEditor.BindingConverters
{
    public class SkinEditorXmlImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var valueImage = value.ToString();
               

                if (valueImage.Contains('+'))
                {
                    string path = string.Empty;
                    foreach (var item in valueImage.Split('+'))
                    {
                        if (item.StartsWith("@"))
                        {
                            path += SkinEditorInfoManager.GetLanguageValue(item);
                            continue;
                        }

                        else if (item.StartsWith("#"))
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
                }
                else
                {
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
                    if (image != null)
                    {
                        return image.FileName;
                    }
                }
                return valueImage;
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
