// <copyright file="SimpleUndefinedExtractor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.UndefinedExtractor
{
    /// <summary>
    /// Does not attempt to translate the bytes and merely returns a string representation
    /// </summary>
    internal class SimpleUndefinedExtractor : IExifValueUndefinedExtractor
    {
        /// <summary>
        /// Gets the Exif Value
        /// </summary>
        /// <param name="value">Array of bytes representing the value or values</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>The Exif Value</returns>
        public IExifValue GetExifValue(byte[] value, int length)
        {
            string bytesString = String.Join(" ", value.Select(b => b.ToString("X2")));
            return new ExifValue<string>(new[] { bytesString });
        }
    }
}
