using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GUISkinFramework.Skin;

namespace GUISkinFramework
{
    public static class XmlExtensions
    {



        public static IEnumerable<XmlControl> GetControls(this IEnumerable<XmlControl> controls)
        {
            foreach (var control in controls)
            {
                yield return control;
                if (control is XmlGroup)
                {
                    foreach (var grpControl in (control as XmlGroup).Controls.GetControls())
                    {
                        yield return grpControl;
                    }
                }
            }
        }



         
     
        

    
        public static void SetDefaultValues<T>(this T obj)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var attribute = property
                    .GetCustomAttributes(typeof(DefaultValueAttribute), true)
                    .Cast<DefaultValueAttribute>()
                    .FirstOrDefault();
                if (attribute != null)
                {
                    try
                    {
                        property.SetValue(obj, attribute.Value, null);
                    }
                    catch
                    {
                        property.SetValue(obj,default(T), null);
                    }
                }
            }
        }

    }
}
