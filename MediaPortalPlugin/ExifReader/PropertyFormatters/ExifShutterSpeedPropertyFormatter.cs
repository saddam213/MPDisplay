// <copyright file="ExifShutterSpeedPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Exif shutter-speed property
    /// </summary>
    internal class ExifShutterSpeedPropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName => "Shutter Speed";

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<Rational32>();

            var rational32S = values as IList<Rational32> ?? values.ToList();
            if (!rational32S.Any())
            {
                return string.Empty;
            }

            var apexValue = (double)rational32S.First();
            var shutterSpeed = 1 / Math.Pow(2, apexValue);

            return shutterSpeed > 1 ?
                $"{(int) Math.Round(shutterSpeed)} sec."
                : $"{1}/{(int) Math.Round(1/shutterSpeed)} sec.";            
        }
    }
}
