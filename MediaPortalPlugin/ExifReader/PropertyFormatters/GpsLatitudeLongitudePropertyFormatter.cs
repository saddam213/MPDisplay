// <copyright file="GpsLatitudeLongitudePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Gps Latitude and Longitude properties
    /// </summary>
    internal class GpsLatitudeLongitudePropertyFormatter : SimpleExifPropertyFormatter
    {
        /// <summary>
        /// Initializes a new instance of the GpsLatitudeLongitudePropertyFormatter class.
        /// </summary>
        /// <param name="tagId">The associated PropertyTagId</param>
        public GpsLatitudeLongitudePropertyFormatter(PropertyTagId tagId)
            : base(tagId)
        {            
        }

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public override string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<Rational32>();
            var rational32S = values as IList<Rational32> ?? values.ToList();
            if (rational32S.Count() != 3)
            {
                return String.Empty;
            }

            double val = (double)rational32S.ElementAt(0) + (double)rational32S.ElementAt(1)/60 + (double)rational32S.ElementAt(2)/3600;
            return val.ToString("F6", CultureInfo.InvariantCulture);
        }
    }
}
