// <copyright file="ExifColorSpacePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Color Space property
    /// </summary>
    internal class ExifColorSpacePropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName => "Color Space";

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<ushort>();
            string formattedString;

            switch (values.FirstOrDefault())
            {
                case 1:
                    formattedString = "sRGB";
                    break;

                case 0xffff:
                    formattedString = "Uncalibrated";
                    break;

                default:
                    formattedString = "Reserved";
                    break;
            }

            return formattedString;
        }   
    }
}
