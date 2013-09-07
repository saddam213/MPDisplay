using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Helpers
{
    public static class SerializationHelper
    {
     

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="filename">The filename.</param>
        public static bool Serialize<T>(T obj, string filename)
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
            catch (Exception ex)
            {
               
            }
            return false;
        }

        /// <summary>
        /// Deserializes the specified object from a file.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns>The object from the file</returns>
        public static T Deserialize<T>(string filename)
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
            catch (Exception ex)
            {
               
            }
            return default(T);
        }


        public static T CreateCopy<T>(this T obj)
        {
            try
            {
                //Create our own namespaces for the output
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                using (MemoryStream myWriter = new MemoryStream())
                {
                    mySerializer.Serialize(myWriter, obj, ns);
                    myWriter.Seek(0, SeekOrigin.Begin);
                    var copy = (T)mySerializer.Deserialize(myWriter);
                
                    return copy;
                }
            }
            catch (Exception ex)
            {
               
            }
            return default(T);
        }
    }
}
