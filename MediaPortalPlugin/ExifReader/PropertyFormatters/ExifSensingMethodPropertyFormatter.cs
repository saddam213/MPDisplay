// <copyright file="ExifSensingMethodPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the SensingMethod property
    /// </summary>
    internal class ExifSensingMethodPropertyFormatter : GenericDictionaryPropertyFormatter<ushort>
    {
        /// <summary>
        /// Map of SceneCaptureType names to their unsigned short representations
        /// </summary>
        private Dictionary<ushort, string> _sensingMethodMap = new Dictionary<ushort, string>
        {
            { 1, "Not defined" },
            { 2, "One-chip color area sensor" },
            { 3, "Two-chip color area sensor" },
            { 4, "Three-chip color area sensor" },
            { 5, "Color sequential area sensor" },
            { 7, "Trilinear sensor" },
            { 8, "Color sequential linear sensor" }
        };

        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Sensing Method";
            }
        }

        /// <summary>
        /// Gets a dictionary that maps values to named strings
        /// </summary>
        /// <returns>The mapping dictionary</returns>
        protected override Dictionary<ushort, string> GetNameMap()
        {
            return _sensingMethodMap;
        }
    }
}
