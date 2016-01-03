// <copyright file="ExifExposureModePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Exposure Mode property
    /// </summary>
    internal class ExifExposureModePropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName => "Exposure Mode";

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
                case 0:
                    formattedString = "Auto exposure";
                    break;

                case 1:
                    formattedString = "Manual exposure";
                    break;

                case 2:
                    formattedString = "Auto bracket";
                    break;

                default:
                    formattedString = "Reserved";
                    break;
            }

            return formattedString;
        }
    }
}
