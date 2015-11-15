// <copyright file="ExifFlashPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the Flash property
    /// </summary>
    internal class ExifFlashPropertyFormatter : GenericDictionaryPropertyFormatter<ushort>
    {
        /// <summary>
        /// Map of flash mode names to their unsigned short representations
        /// </summary>
        private readonly Dictionary<ushort, string> _flashDescriptionMap = new Dictionary<ushort, string>
        {
            { 0x00, "Flash did not fire" },
            { 0x01, "Flash fired" },
            { 0x05, "Strobe return light not detected" },
            { 0x07, "Strobe return light detected" },
            { 0x09, "Flash fired, compulsory flash mode" },
            { 0x0D, "Flash fired, compulsory flash mode, return light not detected" },
            { 0x0F, "Flash fired, compulsory flash mode, return light detected" },
            { 0x10, "Flash did not fire, compulsory flash mode" },
            { 0x18, "Flash did not fire, auto mode" },
            { 0x19, "Flash fired, auto mode" },
            { 0x1D, "Flash fired, auto mode, return light not detected" },
            { 0x1F, "Flash fired, auto mode, return light detected" },
            { 0x20, "No flash function" },
            { 0x41, "Flash fired, red-eye reduction mode" },
            { 0x45, "Flash fired, red-eye reduction mode, return light not detected" },
            { 0x47, "Flash fired, red-eye reduction mode, return light detected" },
            { 0x49, "Flash fired, compulsory flash mode, red-eye reduction mode" },
            { 0x4D, "Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected" },
            { 0x4F, "Flash fired, compulsory flash mode, red-eye reduction mode, return light detected" },
            { 0x59, "Flash fired, auto mode, red-eye reduction mode" },
            { 0x5D, "Flash fired, auto mode, return light not detected, red-eye reduction mode" },
            { 0x5F, "Flash fired, auto mode, return light detected, red-eye reduction mode" }
        };

        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName => "Flash";

        /// <summary>
        /// Gets a dictionary that maps values to named strings
        /// </summary>
        /// <returns>The mapping dictionary</returns>
        protected override Dictionary<ushort, string> GetNameMap()
        {
            return _flashDescriptionMap;
        }
    }
}
