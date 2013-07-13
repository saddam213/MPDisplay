using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}
