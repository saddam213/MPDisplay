// <copyright file="ExifFocalResUnitPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// An IExifPropertyFormatter specific to the ExifFocalResUnit property.
    /// This property tag behaves exactly the same as the ResolutionUnit property,
    /// and thus we derive this class from ResolutionUnitPropertyFormatter and override
    /// just the DisplayName property.
    /// </summary>
    internal class ExifFocalResUnitPropertyFormatter : ResolutionUnitPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        public override string DisplayName
        {
            get { return "Focal Plane Resolution Unit"; }
        }
    }
}
