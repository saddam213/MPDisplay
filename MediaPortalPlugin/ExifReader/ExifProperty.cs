﻿// <copyright file="ExifProperty.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Drawing.Imaging;
using Common.Log;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Represents an Exif property.
    /// </summary>
    public class ExifProperty
    {
        /// <summary>
        /// The PropertyItem associated with this object.
        /// </summary>
        private readonly PropertyItem _propertyItem;

        /// <summary>
        /// The IExifValue associated with this object.
        /// </summary>
        private IExifValue _exifValue;

        /// <summary>
        /// The IExifPropertyFormatter for this property.
        /// </summary>
        private readonly IExifPropertyFormatter _propertyFormatter;

        /// <summary>
        /// Set to true if this object represents an unknown property tag
        /// </summary>
        private readonly bool _isUnknown;

        /// <summary>
        /// Set to true if this object has a custom property formatter
        /// </summary>
        private readonly bool _hasCustomFormatter;

        /// <summary>
        /// The parent ExifReader that owns this ExifProperty object
        /// </summary>
        private readonly ExifReader _parentReader;

        /// <summary>
        /// Initializes a new instance of the ExifProperty class.
        /// It's marked internal  as it's not intended to be instantiated independently outside of the library.
        /// </summary>
        /// <param name="propertyItem">The PropertyItem to base the object on</param>
        /// <param name="parentReader">The parent ExifReader</param>
        internal ExifProperty(PropertyItem propertyItem, ExifReader parentReader)
        {
            _parentReader = parentReader;
            _propertyItem = propertyItem;
            _isUnknown = !Enum.IsDefined(typeof(PropertyTagId), RawExifTagId);

            var customFormatter = _parentReader.QueryForCustomPropertyFormatter(RawExifTagId);

            if (customFormatter == null)
            {
                _propertyFormatter = ExifPropertyFormatterProvider.GetExifPropertyFormatter(ExifTag);
            }
            else
            {
                _propertyFormatter = customFormatter;
                _hasCustomFormatter = true;
            }
        }

        /// <summary>
        /// Gets the IExifValue for this property
        /// </summary>
        public IExifValue ExifValue => _exifValue ?? InitializeExifValue();

        /// <summary>
        /// Gets the descriptive name of the Exif property
        /// </summary>
        public string ExifPropertyName
        {
            get
            {
                try
                {
                    return _hasCustomFormatter || !_isUnknown ?
                        _propertyFormatter.DisplayName :
                        $"{_propertyFormatter.DisplayName} #{_propertyItem.Id}";
                }
                catch (Exception ex)
                {
                    LogException( "Error getting ExifPropertyName", ex);
                }
                return string.Empty;
            }
        }        

        /// <summary>
        /// Gets a category name for the property.
        /// Note: This is not part of the Exif standard and is merely for convenience.
        /// </summary>
        public string ExifPropertyCategory => _isUnknown ? "Unknown" : "General";

        /// <summary>
        /// Gets the Exif property tag Id for this property
        /// </summary>
        public PropertyTagId ExifTag => _isUnknown ? PropertyTagId.UnknownExifTag : (PropertyTagId)_propertyItem.Id;

        /// <summary>
        /// Gets the Exif data type for this property
        /// </summary>
        public PropertyTagType ExifDatatype => (PropertyTagType)_propertyItem.Type;

        /// <summary>
        /// Gets the raw Exif tag. For unknown tags this will not
        /// match the value of the ExifTag property.
        /// </summary>
        public int RawExifTagId => _propertyItem.Id;

        /// <summary>
        /// Override for ToString
        /// </summary>
        /// <returns>Returns a readable string representing the Exif property's value</returns>
        public override string ToString()
        {
            return GetFormattedString();
        }

        /// <summary>
        /// Gets the formatted string using the property formatter
        /// </summary>
        /// <returns>The formatted string</returns>
        private string GetFormattedString()
        {
            try
            {
                return _propertyFormatter.GetFormattedString(ExifValue);
            }
            catch (Exception ex)
            {
                LogException("Error getting formatted string", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// Initializes the exifValue field.
        /// </summary>
        /// <returns>The initialized exifValue</returns>
        private IExifValue InitializeExifValue()
        {
            try
            {
                var customExtractor = _parentReader.QueryForCustomUndefinedExtractor(RawExifTagId);
                if (customExtractor != null)
                {
                    return _exifValue = customExtractor.GetExifValue(_propertyItem.Value, _propertyItem.Len);
                } 

                return _exifValue = ExifDatatype == PropertyTagType.Undefined ?
                        ExifValueCreator.CreateUndefined(ExifTag, _propertyItem.Value, _propertyItem.Len) :
                        ExifValueCreator.Create(ExifDatatype, _propertyItem.Value, _propertyItem.Len);
            }
            catch (Exception ex)
            {
                LogException("Error in InitializeExifValue", ex);
            }
            return null;
        }

        /// <summary>
        /// Returns an ExifReaderException set with the current property formatter
        /// </summary>
        /// <param name="text">Text to be inserted for the exception</param>
        /// <param name="ex">Inner exception object</param>
        /// <returns>The ExifReaderException object</returns>
        private static void LogException(string text, Exception ex)
        {
            LoggingManager.GetLog(typeof(ExifProperty)).Message(LogLevel.Error, "{0}: {1}.", text, ex);
        }
    }
}
