using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SkinEditor.BindingConverters
{
    public class SkinEditorXmlLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var valueLabel = value.ToString();
                var displayLabel = string.Empty;

                var labelItems = valueLabel.Contains("+")
                    ? new List<string>(valueLabel.Split('+'))
                    : new List<string> { valueLabel };

                foreach (var item in labelItems)
                {
                    if (item.StartsWith("@"))
                    {
                        displayLabel += SkinEditorInfoManager.GetLanguageValue(item);
                        continue;
                    }
                    else if (item.StartsWith("#"))
                    {
                        var prop = SkinEditorInfoManager.SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == item);
                        if (prop != null)
                        {
                            displayLabel += prop.DesignerValue;
                            continue;
                        }
                    }
                    displayLabel += item;
                }
                return displayLabel;
            }
            return value;

        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
