// <copyright file="GpsTimePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Gps Time property
    /// </summary>
    internal class GpsTimePropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName => "GPS Time";

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<Rational32>();
            var rational32S = values as IList<Rational32> ?? values.ToList();
            return rational32S.Count != 3 ? string.Empty :
                $"{ (double)rational32S.ElementAt(0)}:{ (double)rational32S.ElementAt(1)}:{ (double)rational32S.ElementAt(2)}";
        }
    }
}
