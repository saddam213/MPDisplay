// <copyright file="ExifLightSourcePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Exif Light Source property
    /// </summary>
    internal class ExifLightSourcePropertyFormatter : GenericDictionaryPropertyFormatter<ushort>
    {
        /// <summary>
        /// Map of light source names to their unsigned short representations
        /// </summary>
        private readonly Dictionary<ushort, string> _lightSourceNameMap = new Dictionary<ushort, string>
        {
            { 0, "Unknown" },
            { 1, "Daylight" },
            { 2, "Fluorescent" },
            { 3, "Tungsten (incandescent light)" },
            { 4, "Flash" },
            { 9, "Fine weather" },
            { 10, "Cloudy weather" },
            { 11, "Shade" },
            { 12, "Daylight fluorescent (D 5700 – 7100K)" },
            { 13, "Day white fluorescent (N 4600 – 5400K)" },
            { 14, "Cool white fluorescent (W 3900 – 4500K)" },
            { 15, "White fluorescent (WW 3200 – 3700K)" },
            { 17, "Standard light A" },
            { 18, "Standard light B" },
            { 19, "Standard light C" },
            { 20, "D55" },
            { 21, "D65" },
            { 22, "D75" },
            { 23, "D50" },
            { 24, "ISO studio tungsten" },
            { 255, "Other light source" }
        };

        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName => "Light Source";

        /// <summary>
        /// Gets a dictionary that maps values to named strings
        /// </summary>
        /// <returns>The mapping dictionary</returns>
        protected override Dictionary<ushort, string> GetNameMap()
        {
            return _lightSourceNameMap;
        }
    }
}
