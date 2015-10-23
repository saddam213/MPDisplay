// <copyright file="IExifValue.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This interface represents an Exif property value
    /// </summary>
    public interface IExifValue
    {
        /// <summary>
        /// Gets the type of the Exif property value or values
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Gets the number of values
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a type-unsafe collection of values of a specific Exif tag data type
        /// </summary>
        IEnumerable Values { get; }
    }
}
