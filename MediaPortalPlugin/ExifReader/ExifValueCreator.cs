// <copyright file="ExifValueCreator.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Common.Log;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// A factory class for creating different types of ExifValue objects
    /// </summary>
    internal class ExifValueCreator
    {
        /// <summary>
        /// Delegate map between Exif tag types and associated creation methods
        /// </summary>
        private static readonly Dictionary<PropertyTagType, CreateExifValueDelegate> CreateExifValueDelegateMap = new Dictionary<PropertyTagType, CreateExifValueDelegate>
        {
            { PropertyTagType.ASCII, CreateExifValueForAsciiData },
            { PropertyTagType.Byte, CreateExifValueForByteData },
            { PropertyTagType.Short, CreateExifValueForShortData },
            { PropertyTagType.Long, CreateExifValueForLongData },            
            { PropertyTagType.SLong, CreateExifValueForSLongData },            
            { PropertyTagType.Rational, CreateExifValueForRationalData },            
            { PropertyTagType.SRational, CreateExifValueForSRationalData }
        };

        /// <summary>
        /// Delegate that creates the appropriate Exif value of a specific type
        /// </summary>
        /// <param name="value">Array of bytes representing the value or values</param>
        /// <param name="length">Number of values or length of an ASCII string value</param>
        /// <returns>The Exif value or values</returns>
        private delegate IExifValue CreateExifValueDelegate(byte[] value, int length);

        /// <summary>
        /// Creates an ExifValue for a specific type
        /// </summary>
        /// <param name="type">The property data type</param>
        /// <param name="value">An array of bytes representing the value or values</param>
        /// <param name="length">A length parameter specifying the number of values or the length of a string for ASCII string data</param>
        /// <returns>An appropriate IExifValue object</returns>
        internal static IExifValue Create(PropertyTagType type, byte[] value, int length)
        {
            try
            {
                CreateExifValueDelegate createExifValueDelegate;
                return CreateExifValueDelegateMap.TryGetValue(type, out createExifValueDelegate) ? createExifValueDelegate(value, length) : new ExifValue<string>(new[] { type.ToString() });
            }
            catch (Exception ex)
            {
                LogException("Error in Create", ex);
            }
            return null;
        }

        /// <summary>
        /// Creates an ExifValue for an undefined value type
        /// </summary>
        /// <param name="tagId">The tag Id whose value needs to be extracted</param>
        /// <param name="value">An array of bytes representing the value or values</param>
        /// <param name="length">The number of bytes</param>
        /// <returns>An appropriate IExifValue object</returns>
        internal static IExifValue CreateUndefined(PropertyTagId tagId, byte[] value, int length)
        {
            var extractor = ExifValueUndefinedExtractorProvider.GetExifValueUndefinedExtractor(tagId);

            try
            {
                return extractor.GetExifValue(value, length);
            }
            catch (Exception ex)
            {
                LogException("Error in CreateUndefined", ex);
            }
            return null;
        }

        /// <summary>
        /// Creation method for ASCII string values.
        /// </summary>
        /// <param name="value">Bytes representing the string value</param>
        /// <param name="length">Length of the string</param>
        /// <returns>Exif value representing the string</returns>
        private static IExifValue CreateExifValueForAsciiData(byte[] value, int length)
        {
            var strings = new string[1]; // There's always 1 string
            var encoding = new ASCIIEncoding();

            strings[0] = encoding.GetString(value).TrimEnd('\0');

            return new ExifValue<string>(strings);
        }

        /// <summary>
        /// Creation method for byte values
        /// </summary>
        /// <param name="value">Bytes representing the byte data</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>Exif value representing the byte data</returns>
        private static IExifValue CreateExifValueForByteData(byte[] value, int length)
        {
            return new ExifValue<byte>(value);
        }

        /// <summary>
        /// Creation method for short values
        /// </summary>
        /// <param name="value">Bytes representing the short data</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>Exif value representing the short data</returns>
        private static IExifValue CreateExifValueForShortData(byte[] value, int length)
        {
            return CreateExifValueForGenericData(value, length, BitConverter.ToUInt16);
        }

        /// <summary>
        /// Creation method for long values
        /// </summary>
        /// <param name="value">Bytes representing the long data</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>Exif value representing the long data</returns>
        private static IExifValue CreateExifValueForLongData(byte[] value, int length)
        {
            return CreateExifValueForGenericData(value, length, BitConverter.ToUInt32);
        }

        /// <summary>
        /// Creation method for signed long values
        /// </summary>
        /// <param name="value">Bytes representing the slong data</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>Exif value representing the slong data</returns>
        private static IExifValue CreateExifValueForSLongData(byte[] value, int length)
        {
            return CreateExifValueForGenericData(value, length, BitConverter.ToInt32);
        }

        /// <summary>
        /// Creation method for Rational values
        /// </summary>
        /// <param name="value">Bytes representing the Rational data</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>Exif value representing the Rational data</returns>
        private static IExifValue CreateExifValueForRationalData(byte[] value, int length)
        {
            return CreateExifValueForGenericData(
                value, 
                length,
                sizeof(uint) * 2,
                (bytes, pos) => new Rational32(BitConverter.ToUInt32(bytes, pos), BitConverter.ToUInt32(bytes, pos + sizeof(uint))));
        }

        /// <summary>
        /// Creation method for SRational values
        /// </summary>
        /// <param name="value">Bytes representing the SRational data</param>
        /// <param name="length">Number of bytes</param>
        /// <returns>Exif value representing the Rational data</returns>
        private static IExifValue CreateExifValueForSRationalData(byte[] value, int length)
        {
            return CreateExifValueForGenericData(
                value, 
                length,
                sizeof(int) * 2,
                (bytes, pos) => new Rational32(BitConverter.ToInt32(bytes, pos), BitConverter.ToInt32(bytes, pos + sizeof(int))));
        }

        /// <summary>
        /// Generic creation method
        /// </summary>
        /// <typeparam name="T">The data type of the value data</typeparam>
        /// <param name="value">Bytes representing the data</param>
        /// <param name="length">Number of bytes</param>
        /// <param name="converterFunction">Function that converts from bytes to a specific data type</param>
        /// <returns>Exif value representing the generic data type</returns>
        private static IExifValue CreateExifValueForGenericData<T>(byte[] value, int length, Func<byte[], int, T> converterFunction) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            return CreateExifValueForGenericData(value, length, size, converterFunction);
        }

        /// <summary>
        /// Generic creation method
        /// </summary>
        /// <typeparam name="T">The data type of the value data</typeparam>
        /// <param name="value">Bytes representing the data</param>
        /// <param name="length">Number of bytes</param>
        /// <param name="dataValueSize">Size of each data value</param>
        /// <param name="converterFunction">Function that converts from bytes to a specific data type</param>
        /// <returns>Exif value representing the generic data type</returns>
        private static IExifValue CreateExifValueForGenericData<T>(byte[] value, int length, int dataValueSize, Func<byte[], int, T> converterFunction) where T : struct
        {
            var data = new T[length / dataValueSize];

            for (int i = 0, pos = 0; i < length / dataValueSize; i++, pos += dataValueSize)
            {
                data[i] = converterFunction(value, pos);
            }

            return new ExifValue<T>(data);
        }
        /// <summary>
        /// Returns an ExifReaderException set with the current property formatter
        /// </summary>
        /// <param name="text">Text to be inserted for the exception</param>
        /// <param name="ex">Inner exception object</param>
        /// <returns>The ExifReaderException object</returns>
        private static void LogException(string text, Exception ex)
        {
            LoggingManager.GetLog(typeof(ExifValueCreator)).Message(LogLevel.Error, "{0}: {1}.", text, ex);
        }

    }
}
