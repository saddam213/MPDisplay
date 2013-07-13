using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPDisplay.Common.Settings
{
    public static class SettingsManager
    {
        public static bool Save<T>(T obj, string filename) where T : SettingsBase
        {
            try
            {
                //Create our own namespaces for the output
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                using (StreamWriter myWriter = new StreamWriter(filename))
                {
                    mySerializer.Serialize(myWriter, obj, ns);
                }
                return true;
            }
            catch { }
            return false;
        }

        public static T Load<T>(string filename) where T : SettingsBase
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var obj = (T)mySerializer.Deserialize(fileStream);
                    return obj;
                }
            }
            catch { }
            return default(T);
        }
    }
}
