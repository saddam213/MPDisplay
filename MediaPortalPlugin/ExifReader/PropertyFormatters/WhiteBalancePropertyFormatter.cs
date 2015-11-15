// <copyright file="WhiteBalancePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Exposure Mode property
    /// </summary>
    internal class WhiteBalancePropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public string DisplayName => "White Balance";

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
                    formattedString = "Auto white balance";
                    break;

                case 1:
                    formattedString = "Manual white balance";
                    break;

                default:
                    formattedString = "Reserved";
                    break;
            }

            return formattedString;
        }
    }
}
