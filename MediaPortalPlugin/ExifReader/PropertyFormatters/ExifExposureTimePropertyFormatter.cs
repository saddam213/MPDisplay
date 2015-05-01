// <copyright file="ExifExposureTimePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Exposure Time property
    /// </summary>
    internal class ExifExposureTimePropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName
        {
            get
            {
                return "Exposure time";
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
            if (!rational32S.Any())
            {
                return String.Empty;
            }

            Rational32 exposure = rational32S.First();
            uint numerator = (uint)exposure.Numerator;
            uint denominator = (uint)exposure.Denominator;

            return denominator == 1 ? String.Format("{0} sec.", numerator) : String.Format("{0}/{1} sec.", numerator, denominator);
        }
    }
}
