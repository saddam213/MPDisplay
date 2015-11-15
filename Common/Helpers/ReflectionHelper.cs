using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Log;

namespace Common.Helpers
{
    public static class ReflectionHelper
    {

        private static readonly Log.Log Log = LoggingManager.GetLog(typeof(ReflectionHelper));


        #region Method Invoke

        /// <summary>
        /// Invokes a method on the specified object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The args.</param>
        public static void InvokeMethod(object obj, string methodName, params object[] args)
        {
            try
            {
                obj.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, args);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[InvokeMethod] - An exception occured invoking method, Method: {1} - {0}", ex, methodName);
            }
        }

        /// <summary>
        /// Invokes a method on the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="defalutValue">The defalut value.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T InvokeMethod<T>(object obj, string methodName, T defalutValue, params object[] args)
        {
            try
            {
                return (T)obj.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, args);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[InvokeMethod] - An exception occured invoking method, Method: {1} - {0}", ex, methodName);
            }
            return defalutValue;
        }

        /// <summary>
        /// Invokes s static method on the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="defalutValue">The defalut value.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T InvokeStaticMethod<T>(Type obj, string methodName, T defalutValue, params object[] args)
        {
            try
            {
                return (T)obj.InvokeMember(methodName, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, args);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[InvokeStaticMethod] - An exception occured invoking method, Method: {1} - {0}", ex, methodName);
            }
            return defalutValue;
        }

        /// <summary>
        /// Invokes s static method on the specified type.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static void InvokeStaticMethod(Type obj, string methodName, params object[] args)
        {
            try
            {
                obj.InvokeMember(methodName, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, args);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[InvokeStaticMethod] - An exception occured invoking method, Method: {1} - {0}", ex, methodName);
            }
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets a property value from an object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="property">The Property Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <param name="index"></param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetPropertyValue<T>(object obj, string property, T defaultValue, object[] index = null)
        {
            try
            {
                var prop = obj?.GetType().GetProperty(property);
                if (prop != null ) return (T)(prop.GetValue(obj, index));
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[GetPropertyValue] - An exception occured getting property value, property: {1} - {0}", ex, property);
            }
            return defaultValue;
        }

        /// <summary>
        /// Sets a property value to an object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="property">The Property Name To Find</param>
        /// <param name="value">Index of Property</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static void SetPropertyValue<T>(object obj, string property, T value)
        {
            try
            {
                if (obj == null) return;
                var prop = obj.GetType().GetProperty(property);
                prop?.SetValue(obj, value);
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[SetPropertyValue] - An exception occured setting property value, property: {1} - {0}", ex, property);
            }
        }

        /// <summary>
        /// Gets a property value from an object
        /// </summary>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="property">The Property Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <param name="index">Index of Property</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static object GetPropertyValue(object obj, string property, object defaultValue, object[] index = null)
        {
            return GetPropertyValue<object>(obj, property, defaultValue, index);
        }

        /// <summary>
        /// Gets a static property value from an object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="property">The Property Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <param name="index">Index of property</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetStaticPropertyValue<T>(object obj, string property, T defaultValue, object[] index = null)
        {
            try
            {
                var prop = obj?.GetType().GetProperty(property);
                if (prop != null) return (T)(prop.GetValue(obj, index));
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[GetStaticPropertyValue] - An exception occured getting property value, property: {1} - {0}", ex, property);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a static property value from an object
        /// </summary>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="property">The Property Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <param name="index">Index of property</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static object GetStaticPropertyValue(object obj, string property, object defaultValue, object[] index = null)
        {
            return GetStaticPropertyValue<object>(obj, property, defaultValue, index);
        }

        #endregion

        #region Field

        /// <summary>
        /// Gets a field value from an object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="field">The Field Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <param name="bindingFlags">Binding flags</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetFieldValue<T>(object obj, string field, T defaultValue, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            try
            {
                var fieldinfo = obj?.GetType().GetField(field, bindingFlags);
                if (fieldinfo != null) return (T)(fieldinfo.GetValue(obj));
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[GetFieldValue] - An exception occured getting field value, Field: {1} - {0}", ex, field);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a field value from an object
        /// </summary>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="field">The Field Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <param name="bindingFlags">Binding flags</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static object GetFieldValue(object obj, string field, object defaultValue = null, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return GetFieldValue<object>(obj, field, defaultValue, bindingFlags);
        }

        /// <summary>
        /// Gets a static field value from an object
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="field">The Field Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static T GetStaticField<T>(object obj, string field, T defaultValue)
        {
            try
            {
                var fieldinfo = obj.GetType().GetField(field);
                if (fieldinfo != null) return (T)(fieldinfo.GetValue(null));
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[GetStaticField] - An exception occured getting field value, Field: {1} - {0}", ex, field);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a static field value from an object
        /// </summary>
        /// <param name="obj">The Object To Find Property In</param>
        /// <param name="field">The Field Name To Find</param>
        /// <param name="defaultValue">Default Value If Property Not Found or Can't Be Accessed</param>
        /// <returns>The Property Value If Found and Can Be Accessed, Else returns 'defaultValue'</returns>
        public static object GetStaticField(object obj, string field, object defaultValue)
        {
            return GetStaticField<object>(obj, field, defaultValue);
        }

        #endregion

        #region Other

        /// <summary>
        /// Gets the property value based on the Property path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="property">The property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetPropertyPath<T>(object obj, string property, T defaultValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(property))
                {
                    var returnValue = obj;
                    if (property.Contains('.'))
                    {
                        var path = property.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                        returnValue = path.Aggregate(returnValue, (current, prop) => GetPropertyValue<object>(current, prop, null) ?? GetFieldValue<object>(current, prop, null));
                        return (T)returnValue;
                    }
                    returnValue = GetPropertyValue<object>(obj, property, null)
                               ?? GetFieldValue<object>(obj, property, defaultValue);
                    if (returnValue != null)
                    {
                        return (T)returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[GetPropertyPath] - An exception occured getting property value, PropertyPath: {1} - {0}", ex, property);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the PropertyInfo from an object based on the property path.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyPath(object obj, string property)
        {
            try
            {
                if (!string.IsNullOrEmpty(property))
                {
                    var returnValue = obj;
                    if (!property.Contains('.')) return obj.GetType().GetProperty(property);
                    var path = property.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                    returnValue = path.Where(prop => prop != path.Last()).Aggregate(returnValue, (current, prop) => GetPropertyValue<object>(current, prop, null));

                    return returnValue != null ? returnValue.GetType().GetProperty(path.Last()) : obj.GetType().GetProperty(property);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogLevel.Debug, "[GetPropertyPath] - An exception occured getting PropertyInfo, PropertyPath: {1} - {0}", ex, property);
            }
            return null;
        }

        #endregion




        public static IEnumerable<string> FindStringValues(object obj)
        {
            return _FindStringValues(obj, new List<object>());
        }

        private static IEnumerable<string> _FindStringValues(object obj, ICollection<object> visitedObjects)
        {
            if (obj == null)
                yield break;

            // Console.WriteLine(string.Join("; ", visitedObjects.Select(i => i.ToString()).ToArray()));
            if (visitedObjects.Any(item => ReferenceEquals(item, obj)))
                yield break;

            if (!(obj is string))
                visitedObjects.Add(obj);

            var type = obj.GetType();

            if (type == typeof(string))
            {
                yield return (obj).ToString();
                yield break;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var array = obj as IEnumerable;
                if (array == null) yield break;
                foreach (var str in array.Cast<object>().SelectMany(item => _FindStringValues(item, visitedObjects)))
                    yield return str;

                yield break;
            }

            if (!type.IsClass) yield break;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var field in fields)
            {
                var item = field.GetValue(obj);

                if (item == null)
                    continue;

                foreach (var str in _FindStringValues(item, visitedObjects))
                    yield return str;
            }
        }
    }
}
