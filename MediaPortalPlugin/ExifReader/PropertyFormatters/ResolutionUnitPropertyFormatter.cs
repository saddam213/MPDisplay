// <copyright file="ResolutionUnitPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Linq;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Resolution Unit property
    /// </summary>
    internal class ResolutionUnitPropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public virtual string DisplayName => "Resolution Unit";

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public virtual string GetFormattedString(IExifValue exifValue)
        {
            var values = exifValue.Values.Cast<ushort>();
            string formattedString;

            switch (values.FirstOrDefault())
            {
                case 2:
                    formattedString = "Inches";
                    break;

                case 3:
                    formattedString = "Centimeters";
                    break;

                default:
                    formattedString = "Reserved";
                    break;
            }

            return formattedString;
        }
    }
}
