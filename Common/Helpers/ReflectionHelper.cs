using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<string> FindStringValues(object obj)
        {
            return FindStringValues(obj, new List<object>());
        }

        public static bool FindStringValue(object obj, string valueToFind)
        {
            return FindStringValue(obj, valueToFind, new List<object>());
        }

        private static IEnumerable<string> FindStringValues(object obj, IList<object> visitedObjects)
        {
            if (obj == null)
                yield break;

            // Console.WriteLine(string.Join("; ", visitedObjects.Select(i => i.ToString()).ToArray()));
            if (visitedObjects.Any(item => Object.ReferenceEquals(item, obj)))
                yield break;

            if (!(obj is string))
                visitedObjects.Add(obj);

            Type type = obj.GetType();

            if (type == typeof(string))
            {
                yield return (obj).ToString();
                yield break;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var array = obj as IEnumerable;
                foreach (var item in array)
                    foreach (var str in FindStringValues(item, visitedObjects))
                        yield return str;

                yield break;
            }

            if (type.IsClass)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object item = field.GetValue(obj);

                    if (item == null)
                        continue;


                    foreach (var str in FindStringValues(item, visitedObjects))
                        yield return str;
                }

                yield break;
            }
        }

       

        private static bool FindStringValue(object obj, string valueToFind, IList<object> visitedObjects)
        {
            try
            {
                if (obj == null)
                    return false;

                // Console.WriteLine(string.Join("; ", visitedObjects.Select(i => i.ToString()).ToArray()));
                if (visitedObjects.Any(item => Object.ReferenceEquals(item, obj)))
                    return false;

                if (!(obj is string))
                    visitedObjects.Add(obj);

                Type type = obj.GetType();

                if (type == typeof(string))
                {
                    return (obj).ToString() == valueToFind;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    var array = obj as IEnumerable;
                    foreach (var item in array)
                    {
                        if (FindStringValue(item, valueToFind, visitedObjects))
                        {
                            return true;
                        }
                    }
                }
                else if (type.IsClass)
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (FieldInfo field in fields)
                    {
                        object item = field.GetValue(obj);
                        if (item != null)
                        {
                            if (FindStringValue(item, valueToFind, visitedObjects))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
            
            }
            return false;
        }


        #region Vars

        private static Hashtable AssemblyReferences = new Hashtable();
        private static Hashtable ClassReferences = new Hashtable();

        #endregion

        #region Public Members

        /// <summary>
        /// Execute An Object Method An Return The Value
        /// </summary>
        /// <typeparam name="T">The Return Type Of The Method</typeparam>
        /// <param name="assemblyPath">The Assembly FilePath/Name</param>
        /// <param name="className">The Assembly Class Name That Contains The Property</param>
        /// <param name="methodName">The Method Name To Execute</param>
        /// <param name="args">The Methods Arguments</param>
        /// <param name="defaultValue">Default Return Value If Method Not Found or Can't Be Accessed</param>
        /// <returns>The Method Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T InvokeObjectMethod<T>(string assemblyPath, string className, string methodName, object[] args, T defalutValue)
        {
            try
            {
                var classInfo = GetClassReference(assemblyPath, className);
                if (classInfo != null)
                {
                    return (T)classInfo.type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, classInfo.ClassObject, args);
                }
            }
            catch
            { 
            }
            return defalutValue;
        }

        /// <summary>
        /// Execute An Object Method An Return The Value
        /// </summary>
        /// <typeparam name="T">The Return Type Of The Method</typeparam>
        /// <param name="assemblyPath">The Assembly FilePath/Name</param>
        /// <param name="className">The Assembly Class Name That Contains The Property</param>
        /// <param name="methodName">The Method Name To Execute</param>
        /// <param name="defaultValue">Default Return Value If Method Not Found or Can't Be Accessed</param>
        /// <returns>The Method Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T InvokeObjectMethod<T>(string assemblyPath, string className, string methodName, T defalutValue)
        {
            try
            {
                var classInfo = GetClassReference(assemblyPath, className);
                if (classInfo != null)
                {
                    return (T)classInfo.type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, classInfo.ClassObject, null);
                }
            }
            catch { }
            return defalutValue;
        }

        /// <summary>
        /// Execute An Method
        /// </summary>
        /// <param name="assemblyPath">The Assembly FilePath/Name</param>
        /// <param name="className">The Assembly Class Name That Contains The Property</param>
        /// <param name="methodName">The Method Name To Execute</param>
        /// <param name="args">The Methods Arguments</param>
        public static void InvokeMethod(string assemblyPath, string className, string methodName, object[] args)
        {
            try
            {
                var classInfo = GetClassReference(assemblyPath, className);
                if (classInfo != null)
                {
                    classInfo.type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, classInfo.ClassObject, args);
                }
            }
            catch { }
        }

        /// <summary>
        /// Execute An Method
        /// </summary>
        /// <param name="assemblyPath">The Assembly FilePath/Name</param>
        /// <param name="className">The Assembly Class Name That Contains The Property</param>
        /// <param name="methodName">The Method Name To Execute</param>
        public static void InvokeMethod(string assemblyPath, string className, string methodName)
        {
            try
            {
                var classInfo = GetClassReference(assemblyPath, className);
                if (classInfo != null)
                {
                    classInfo.type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, classInfo.ClassObject, null);
                }
            }
            catch { }
        }

        /// <summary>
        /// Get A Property Value From An Assembly Class
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="assemblyPath">The Assembly FilePath/Name</param>
        /// <param name="className">The Assembly Class Name That Contains The Property</param>
        /// <param name="propertyName">The Property Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetPropertyValue<T>(string assemblyPath, string className, string propertyName, T defalutValue)
        {
            try
            {
                var classInfo = GetClassReference(assemblyPath, className);
                if (classInfo != null)
                {
                    return (T)classInfo.type.InvokeMember(propertyName, BindingFlags.GetProperty, null, classInfo.ClassObject, null);
                }
            }
            catch { }
            return defalutValue;
        }

        /// <summary>
        /// Get A Property Value From An Object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="property">The Property Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetPropertyValue<T>(object obj, string property, T defaultValue)
        {
            try
            {
                return (T)(obj.GetType().GetProperty(property).GetValue(obj, null));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get A Feild Value From An Object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="field">The Field Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetFieldValue<T>(object obj, string field, T defaultValue, BindingFlags bindingFlags = 0)
        {
            try
            {
                return (T)(obj.GetType().GetField(field, bindingFlags).GetValue(obj));
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }

        #endregion

        #region Private Methods

        private static DynaClassInfo GetClassReference(string assemblyPath, string className)
        {
            if (File.Exists(assemblyPath))
            {
                try
                {
                    Assembly assembly;
                    if (ClassReferences.ContainsKey(assemblyPath))
                    {
                        return (DynaClassInfo)ClassReferences[assemblyPath];
                    }
                    if (!AssemblyReferences.ContainsKey(assemblyPath))
                    {
                        AssemblyReferences.Add(assemblyPath, assembly = Assembly.LoadFrom(assemblyPath));
                    }
                    else
                    {
                        assembly = (Assembly)AssemblyReferences[assemblyPath];
                    }
                    if (assembly != null)
                    {
                        var classReference = assembly.GetTypes().ToList().Find(t => t.IsClass && t.FullName.EndsWith("." + className));
                        if (classReference != null)
                        {
                            if (classReference.IsAbstract)
                            {
                                return new DynaClassInfo(classReference, null);
                            }
                            var info = new DynaClassInfo(classReference, Activator.CreateInstance(classReference));
                            ClassReferences.Add(assemblyPath, info);
                            return info;
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        #endregion

        #region DynaClassInfo

        public class DynaClassInfo
        {
            public object ClassObject;
            public Type type;

            public DynaClassInfo() { }

            public DynaClassInfo(Type t, object c)
            {
                this.type = t;
                this.ClassObject = c;
            }
        }

        #endregion
    }
}
