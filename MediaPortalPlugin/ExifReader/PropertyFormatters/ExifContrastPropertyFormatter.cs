﻿// <copyright file="ExifContrastPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Contrast property
    /// </summary>
    internal class ExifContrastPropertyFormatter : GenericDictionaryPropertyFormatter<ushort>
    {
        /// <summary>
        /// Map of SceneCaptureType names to their unsigned short representations
        /// </summary>
        private readonly Dictionary<ushort, string> _contrastMap = new Dictionary<ushort, string>
        {
            { 0, "Normal" },
            { 1, "Soft" },
            { 2, "Hard" }
        };

        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName => "Contrast";

        /// <summary>
        /// Gets a dictionary that maps values to named strings
        /// </summary>
        /// <returns>The mapping dictionary</returns>
        protected override Dictionary<ushort, string> GetNameMap()
        {
            return _contrastMap;
        }
    }
}
