using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class EnumComboBoxEditor : ComboBoxEditor
    {
        protected override IList<object> CreateItemsSource(PropertyItem propertyItem)
        {
            return GetValues(propertyItem.PropertyType);
        }

        private static object[] GetValues(Type enumType)
        {
            var values = new List<object>();            

            var fields = enumType.GetFields().Where(x => x.IsLiteral);
            foreach (var field in fields)
            {
                // Get array of BrowsableAttribute attributes
                var attrs = field.GetCustomAttributes(typeof(BrowsableAttribute), false);
                if (attrs.Length == 1)
                {
                    // If attribute exists and its value is false continue to the next field...
                    var brAttr = (BrowsableAttribute)attrs[0];
                    if (brAttr.Browsable == false) continue;
                }

                values.Add(field.GetValue(enumType));
            }

            return values.ToArray();
        }
    }
}
