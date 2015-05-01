// <copyright file="ExifValueUndefinedExtractorAttribute.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// An attribute used to specify an IExifValueUndefinedExtractor for Exif Tags with undefined data value types
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class ExifValueUndefinedExtractorAttribute : Attribute
    {
        /// <summary>
        /// The IExifValueUndefinedExtractor object
        /// </summary>
        private IExifValueUndefinedExtractor _undefinedExtractor;

        /// <summary>
        /// The type of the IExifValueUndefinedExtractor
        /// </summary>
        private Type _undefinedExtractorType;

        /// <summary>
        /// Initializes a new instance of the ExifValueUndefinedExtractorAttribute class
        /// </summary>
        /// <param name="undefinedExtractorType">The type of the IExifValueUndefinedExtractor</param>
        public ExifValueUndefinedExtractorAttribute(Type undefinedExtractorType)
        {
            _undefinedExtractorType = undefinedExtractorType;
        }

        /// <summary>
        /// Gets the IExifValueUndefinedExtractor
        /// </summary>
        /// <returns>The IExifValueUndefinedExtractor</returns>
        public IExifValueUndefinedExtractor GetUndefinedExtractor()
        {
            return _undefinedExtractor ??
                (_undefinedExtractor = Activator.CreateInstance(_undefinedExtractorType) as IExifValueUndefinedExtractor);
        }
    }
}
