// <copyright file="ExifPropertyFormatterAttribute.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// An attribute used to specify an IExifPropertyFormatter for Exif Tag Ids 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class ExifPropertyFormatterAttribute : Attribute
    {
        /// <summary>
        /// The IExifPropertyFormatter object
        /// </summary>
        private IExifPropertyFormatter _exifPropertyFormatter;

        /// <summary>
        /// The type of the IExifPropertyFormatter
        /// </summary>
        private Type _exifPropertyFormatterType;

        /// <summary>
        /// Initializes a new instance of the ExifPropertyFormatterAttribute class
        /// </summary>
        /// <param name="exifPropertyFormatterType">The type of the IExifPropertyFormatter</param>
        public ExifPropertyFormatterAttribute(Type exifPropertyFormatterType)
        {
            _exifPropertyFormatterType = exifPropertyFormatterType;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the constructor for the property formatter
        /// needs to be passed the property tag as an argument. 
        /// </summary>
        public bool ConstructorNeedsPropertyTag { get; set; }

        /// <summary>
        /// Gets the IExifPropertyFormatter
        /// </summary>
        /// <param name="args">Optional arguments</param>
        /// <returns>The IExifPropertyFormatter</returns>
        public IExifPropertyFormatter GetExifPropertyFormatter(params object[] args)
        {
                return _exifPropertyFormatter ??
                    (_exifPropertyFormatter = Activator.CreateInstance(_exifPropertyFormatterType, args) as IExifPropertyFormatter);
        }
    }
}
