// <copyright file="ExifFileSourceUndefinedExtractor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Linq;

namespace MediaPortalPlugin.ExifReader.UndefinedExtractor
{
    /// <summary>
    /// A class that extracts a value for the Exif File Source property
    /// </summary>
    internal class ExifFileSourceUndefinedExtractor : IExifValueUndefinedExtractor
    {
        /// <summary>
        /// Gets the Exif Value
        /// </summary>
        /// <param name="value">Array of bytes representing the value or values</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>The Exif Value</returns>
        public IExifValue GetExifValue(byte[] value, int length)
        {
            string fileSource = value.FirstOrDefault() == 3 ? "DSC" : "Reserved";
            return new ExifValue<string>(new[] { fileSource });
        }
    }
}
