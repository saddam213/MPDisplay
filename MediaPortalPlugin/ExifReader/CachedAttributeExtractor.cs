// <copyright file="CachedAttributeExtractor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// A generic class used to retrieve an attribute from a type, 
    /// and cache the extracted values for future access.
    /// </summary>
    /// <typeparam name="T">The type to search on</typeparam>
    /// <typeparam name="TA">The attribute type to extract</typeparam>
    internal class CachedAttributeExtractor<T, TA> where TA : Attribute
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        private static CachedAttributeExtractor<T, TA> _instance = new CachedAttributeExtractor<T, TA>();

        /// <summary>
        /// The map of fields to attributes
        /// </summary>
        private Dictionary<string, TA> _fieldAttributeMap = new Dictionary<string, TA>();

        /// <summary>
        /// Prevents a default instance of the CachedAttributeExtractor class from being created.
        /// </summary>
        private CachedAttributeExtractor()
        {
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        internal static CachedAttributeExtractor<T, TA> Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the attribute for the field
        /// </summary>
        /// <param name="field">Name of the field</param>
        /// <returns>The attribute on the field or null</returns>
        public TA GetAttributeForField(string field)
        {
            TA attribute;

            if (!_fieldAttributeMap.TryGetValue(field, out attribute))
            {
                if (TryExtractAttributeFromField(field, out attribute))
                {
                    _fieldAttributeMap[field] = attribute;
                }
                else
                {
                    attribute = null;
                }
            }

            return attribute;
        }

        /// <summary>
        /// Get the attribute for the field 
        /// </summary>
        /// <param name="field">Name of the field</param>
        /// <param name="attribute">The attribute</param>
        /// <returns>Returns true of the attribute was found</returns>
        private bool TryExtractAttributeFromField(string field, out TA attribute)
        {
            var fieldInfo = typeof(T).GetField(field);
            attribute = null;

            if (fieldInfo != null)
            {
                TA[] attributes = fieldInfo.GetCustomAttributes(typeof(TA), false) as TA[];
                if (attributes.Length > 0)
                {
                    attribute = attributes[0];
                }
            }

            return attribute != null;
        }
    }
}
