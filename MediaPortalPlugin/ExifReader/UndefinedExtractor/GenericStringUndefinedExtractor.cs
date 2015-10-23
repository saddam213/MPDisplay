// <copyright file="GenericStringUndefinedExtractor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Text;

namespace MediaPortalPlugin.ExifReader.UndefinedExtractor
{
    /// <summary>
    /// A class that extracts a string from undefined data that is not null-terminated
    /// </summary>
    internal class GenericStringUndefinedExtractor : IExifValueUndefinedExtractor
    {
        /// <summary>
        /// Gets the Exif Value
        /// </summary>
        /// <param name="value">Array of bytes representing the value or values</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>The Exif Value</returns>
        public IExifValue GetExifValue(byte[] value, int length)
        {
            var encoding = new ASCIIEncoding();
            return new ExifValue<string>(new[] { encoding.GetString(value) });
        }
    }
}
