﻿using System;
using System.IO;
using System.Xml.Serialization;
using Common.Log;

namespace Common.Helpers
{
    public static class SerializationHelper
    {
        private static readonly Log.Log Log = LoggingManager.GetLog(typeof(SerializationHelper));

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
                var ns = new XmlSerializerNamespaces();
                ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

                var mySerializer = new XmlSerializer(typeof(T));
                using (var myWriter = new StreamWriter(filename))
                {
                    mySerializer.Serialize(myWriter, obj, ns);
                   
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception("[Serialize] - An exception occured serializing file, FileName: " + filename, ex);
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
                var mySerializer = new XmlSerializer(typeof(T));
                using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var obj = (T)mySerializer.Deserialize(fileStream);
              
                    return obj;

                }
            }
            catch (Exception ex)
            {
                Log.Exception("[Deserialize] - An exception occured deserializing file, FileName: " + filename, ex);
            }
            return default(T);
        }

        /// <summary>
        /// Creates a copy of a serailizable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static T CreateCopy<T>(this T obj)
        {
            try
            {
                //Create our own namespaces for the output
                var ns = new XmlSerializerNamespaces();
                ns.Add("x", "http://www.w3.org/2001/XMLSchema-instance");

                var mySerializer = new XmlSerializer(typeof(T));
                using (var myWriter = new MemoryStream())
                {
                    mySerializer.Serialize(myWriter, obj, ns);
                    myWriter.Seek(0, SeekOrigin.Begin);
                    var copy = (T)mySerializer.Deserialize(myWriter);
                
                    return copy;
                }
            }
            catch (Exception ex)
            {
                 Log.Exception("[CreateCopy] - An exception occured creating object copy", ex);
            }
            return default(T);
        }
    }
}
