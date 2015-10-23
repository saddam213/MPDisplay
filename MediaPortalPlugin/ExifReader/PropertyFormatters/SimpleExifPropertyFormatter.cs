// <copyright file="SimpleExifPropertyFormatter.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Text.RegularExpressions;

namespace MediaPortalPlugin.ExifReader.PropertyFormatters
{
    /// <summary>
    /// A very simple implementation of IExifPropertyFormatter that's used by default
    /// if a more specialized implementation is not available.
    /// </summary>
    internal class SimpleExifPropertyFormatter : IExifPropertyFormatter
    {
        /// <summary>
        /// The associated PropertyTagId
        /// </summary>
        private PropertyTagId _tagId;

        /// <summary>
        /// The display name attribute if one's available
        /// </summary>
        private EnumDisplayNameAttribute _displayNameAttribute;

        /// <summary>
        /// Initializes a new instance of the SimpleExifPropertyFormatter class.
        /// </summary>
        /// <param name="tagId">The associated PropertyTagId</param>
        public SimpleExifPropertyFormatter(PropertyTagId tagId)
        {
            _tagId = tagId;
            _displayNameAttribute = CachedAttributeExtractor<PropertyTagId, EnumDisplayNameAttribute>.Instance.GetAttributeForField(_tagId.ToString());
        }

        /// <summary>
        /// Gets a display name for this property. This default implementation checks to
        /// see if a display name is provided, and if one is not, then it attempts a rather 
        /// crude enhancement and separates out words heuristically by spliting them 
        /// up based on an uppercase letter following a lowercase one.
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                return _displayNameAttribute != null ?
                    _displayNameAttribute.DisplayName :
                    Regex.Replace(_tagId.ToString(), @"([a-z])([A-Z])", @"$1 $2", RegexOptions.None);
            }
        }

        /// <summary>
        /// Gets a formatted string for a given Exif value
        /// </summary>
        /// <param name="exifValue">The source Exif value</param>
        /// <returns>The formatted string</returns>
        public virtual string GetFormattedString(IExifValue exifValue)
        {
            var firstValue = String.Empty;

            foreach (var item in exifValue.Values)
            {
                firstValue = item.ToString();
                break;
            }

            return firstValue;
        }
    }        
}
