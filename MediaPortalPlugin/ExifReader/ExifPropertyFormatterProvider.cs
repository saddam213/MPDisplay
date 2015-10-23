// <copyright file="ExifPropertyFormatterProvider.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using MediaPortalPlugin.ExifReader.PropertyFormatters;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This class provides appropriate IExifPropertyFormatter objects for Exif property values
    /// </summary>
    public static class ExifPropertyFormatterProvider
    {
        /// <summary>
        /// Gets an IExifPropertyFormatter for the specific tagId
        /// </summary>
        /// <param name="tagId">The Exif Tag Id</param>
        /// <returns>An IExifPropertyFormatter</returns>
        internal static IExifPropertyFormatter GetExifPropertyFormatter(PropertyTagId tagId)
        {
            var attribute = CachedAttributeExtractor<PropertyTagId, ExifPropertyFormatterAttribute>.Instance.GetAttributeForField(tagId.ToString());

            if (attribute != null)
            {
                return attribute.ConstructorNeedsPropertyTag ? attribute.GetExifPropertyFormatter(tagId) : attribute.GetExifPropertyFormatter();
            }

            return new SimpleExifPropertyFormatter(tagId);
        }
    }
}
