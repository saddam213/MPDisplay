// <copyright file="ExifFocalLengthPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// A generic IExifPropertyFormatter that handles focal length property values.
    /// </summary>
    internal class ExifFocalLengthPropertyFormatter : SimpleExifPropertyFormatter
    {
        /// <summary>
        /// Initializes a new instance of the ExifFocalLengthPropertyFormatter class.
        /// </summary>
        /// <param name="tagId">The associated PropertyTagId</param>
        public ExifFocalLengthPropertyFormatter(PropertyTagId tagId)
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
            return !rational32S.Any() ? String.Empty : String.Concat(((double)rational32S.First()).ToString("0"), " mm");
        }
    }
}
