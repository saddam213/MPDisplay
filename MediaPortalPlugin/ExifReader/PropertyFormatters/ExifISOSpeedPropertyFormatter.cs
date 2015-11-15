// <copyright file="ExifISOSpeedPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the ISO property
    /// </summary>
    internal class ExifISOSpeedPropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName => "ISO Speed";

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<ushort>();
            var enumerable = values as IList<ushort> ?? values.ToList();
            return !enumerable.Any() ? string.Empty : $"ISO-{enumerable.First()}";
        }
    }
}
