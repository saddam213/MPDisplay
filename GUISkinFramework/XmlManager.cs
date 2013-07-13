using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MPDisplay.Common.Log;

namespace GUISkinFramework
{
    public static class XmlManager
    {
        private static Log _log = LoggingManager.GetLog(typeof(XmlManager));

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
                    mySerializer.Serialize(myWriter, obj,ns);
                    _log.Message(LogLevel.Verbose, "Successfully serialized '{0}', Filename: {1}", typeof(T).Name, filename);
                }
                return true;
            }
            catch (Exception ex)
            {
                 _log.Message(LogLevel.Error, "An exception occured serializing '{0}', Filename: {1}{2}{2}{3}", typeof(T).Name, filename,Environment.NewLine, ex.ToString());
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
                    _log.Message(LogLevel.Verbose, "Successfully deserialized '{0}', Filename: {1}", typeof(T).Name, filename);
                    return obj;

                }
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "An exception occured deserializing '{0}', Filename: {1}{2}{2}{3}", typeof(T).Name, filename, Environment.NewLine, ex.ToString());
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
                    var copy =  (T)mySerializer.Deserialize(myWriter);
                    _log.Message(LogLevel.Verbose, "Successfully copied '{0}'", typeof(T).Name);
                    return copy;
                }
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "An exception occured creating '{0}' copy{1}{1}{2}", typeof(T).Name, Environment.NewLine, ex.ToString());
            }
            return default(T);
        }
    }
}
