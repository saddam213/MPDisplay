// <copyright file="ExifSceneTypeUndefinedExtractor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Linq;

namespace MediaPortalPlugin.ExifReader.UndefinedExtractor
{
    /// <summary>
    /// A class that extracts a value for the Scene Type property
    /// </summary>
    internal class ExifSceneTypeUndefinedExtractor : IExifValueUndefinedExtractor
    {
        /// <summary>
        /// Gets the Exif Value
        /// </summary>
        /// <param name="value">Array of bytes representing the value or values</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>The Exif Value</returns>
        public IExifValue GetExifValue(byte[] value, int length)
        {
            var sceneType = value.FirstOrDefault() == 1 ? "A directly photographed image" : "Reserved";
            return new ExifValue<string>(new[] { sceneType });
        }
    }
}
