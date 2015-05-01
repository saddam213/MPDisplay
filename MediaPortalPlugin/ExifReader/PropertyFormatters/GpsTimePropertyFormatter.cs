// <copyright file="GpsTimePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
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
        public string DisplayName
        {
            get
            {
                return "GPS Time";
            }
        }

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<Rational32>();
            var rational32S = values as IList<Rational32> ?? values.ToList();
            if (rational32S.Count() != 3)
            {
                return String.Empty;
            }

            return String.Format("{0}:{1}:{2}", (int)(double)rational32S.ElementAt(0), (int)(double)rational32S.ElementAt(1), (int)(double)rational32S.ElementAt(2));
        }
    }
}
