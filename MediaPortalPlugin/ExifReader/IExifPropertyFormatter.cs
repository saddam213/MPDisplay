// <copyright file="IExifPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This interface defines how a property value is formatted for display
    /// </summary>
    public interface IExifPropertyFormatter
    {
        /// <summary>
        /// Gets a display name for this property
        /// </summary>
        string DisplayName { get; }    
        
        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        string GetFormattedString(IExifValue exifValue);
    }
}
