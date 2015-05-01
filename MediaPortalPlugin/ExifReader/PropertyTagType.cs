// <copyright file="PropertyTagType.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Defines the various Exif property tag type values
    /// </summary>
    public enum PropertyTagType
    {
        /// <summary>
        /// An 8-bit unsigned integer
        /// </summary>
        Byte = 1,

        /// <summary>
        /// A NULL terminated ASCII string
        /// </summary>
        ASCII = 2,

        /// <summary>
        /// A 16-bit unsigned integer
        /// </summary>
        Short = 3,

        /// <summary>
        /// A 32-bit unsigned integer
        /// </summary>
        Long = 4,

        /// <summary>
        /// Two LONGs. The first is the numerator and the second the denominator
        /// </summary>
        Rational = 5,

        /// <summary>
        /// An 8-bit byte that can take any value depending on the field definition
        /// </summary>
        Undefined = 7,

        /// <summary>
        /// A 32-bit signed integer 
        /// </summary>
        SLong = 9,

        /// <summary>
        /// Two SLONGs. The first SLONG is the numerator and the second the denominator
        /// </summary>
        SRational = 10
    }
}
