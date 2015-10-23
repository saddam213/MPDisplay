// <copyright file="IExifValueUndefinedExtractor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This interface defines how an undefined property value is extracted
    /// </summary>
    public interface IExifValueUndefinedExtractor
    {
        /// <summary>
        /// Gets the Exif Value
        /// </summary>
        /// <param name="value">Array of bytes representing the value or values</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>The Exif Value</returns>
        IExifValue GetExifValue(byte[] value, int length);
    }
}
