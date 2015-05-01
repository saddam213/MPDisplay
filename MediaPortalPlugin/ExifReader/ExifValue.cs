// <copyright file="ExifValue.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Linq;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This class represents an Exif property value (or values)
    /// </summary>
    /// <typeparam name="T">The type of the Exif property value</typeparam>
    public class ExifValue<T> : IExifValue
    {
        /// <summary>
        /// Array of values
        /// </summary>
        private T[] _values;

        /// <summary>
        /// Initializes a new instance of the ExifValue class.
        /// </summary>
        /// <param name="values">Array of Exif values</param>
        public ExifValue(T[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Gets the type of the Exif property value or values
        /// </summary>
        public Type ValueType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets the number of values
        /// </summary>
        public int Count
        {
            get { return _values.Length; }
        }

        /// <summary>
        /// Gets a type-unsafe collection of values of a specific Exif tag data type
        /// </summary>
        public IEnumerable Values
        {
            get { return _values.AsEnumerable(); }
        }
    }
}
