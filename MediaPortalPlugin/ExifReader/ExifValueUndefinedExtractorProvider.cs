// <copyright file="ExifValueUndefinedExtractorProvider.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using MediaPortalPlugin.ExifReader.UndefinedExtractor;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This class provides appropriate IExifValueUndefinedExtractor objects for Exif property values with undefined data
    /// </summary>
    public static class ExifValueUndefinedExtractorProvider
    {
        /// <summary>
        /// Gets an IExifValueUndefinedExtractor for the specific tagId
        /// </summary>
        /// <param name="tagId">The Exif Tag Id</param>
        /// <returns>An IExifValueUndefinedExtractor</returns>
        internal static IExifValueUndefinedExtractor GetExifValueUndefinedExtractor(PropertyTagId tagId)
        {
            var attribute = CachedAttributeExtractor<PropertyTagId, ExifValueUndefinedExtractorAttribute>.Instance.GetAttributeForField(tagId.ToString());

            return attribute != null ? attribute.GetUndefinedExtractor() : new SimpleUndefinedExtractor();
        }
    }
}
