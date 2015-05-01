// <copyright file="ExifSubjectDistanceRangePropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the SubjectDistanceRange property
    /// </summary>
    internal class ExifSubjectDistanceRangePropertyFormatter : GenericDictionaryPropertyFormatter<ushort>
    {
        /// <summary>
        /// Map of SceneCaptureType names to their unsigned short representations
        /// </summary>
        private Dictionary<ushort, string> _subjectDistanceRangeMap = new Dictionary<ushort, string>()
        {
            { 0, "unknown" },
            { 1, "Macro" },
            { 2, "Close view" },
            { 3, "Distant view" }
        };

        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Subject Distance Range";
            }
        }

        /// <summary>
        /// Gets a dictionary that maps values to named strings
        /// </summary>
        /// <returns>The mapping dictionary</returns>
        protected override Dictionary<ushort, string> GetNameMap()
        {
            return _subjectDistanceRangeMap;
        }
    }
}
