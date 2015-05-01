// <copyright file="ExifSceneCaptureTypePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the SceneCaptureType property
    /// </summary>
    internal class ExifSceneCaptureTypePropertyFormatter : GenericDictionaryPropertyFormatter<ushort>
    {
        /// <summary>
        /// Map of SceneCaptureType names to their unsigned short representations
        /// </summary>
        private Dictionary<ushort, string> _sceneCaptureTypeMap = new Dictionary<ushort, string>()
        {
            { 0, "Standard" },
            { 1, "Landscape" },
            { 2, "Portrait" },
            { 3, "Night scene" }
        };

        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Scene Capture Type";
            }
        }

        /// <summary>
        /// Gets a dictionary that maps values to named strings
        /// </summary>
        /// <returns>The mapping dictionary</returns>
        protected override Dictionary<ushort, string> GetNameMap()
        {
            return _sceneCaptureTypeMap;
        }
    }
}
