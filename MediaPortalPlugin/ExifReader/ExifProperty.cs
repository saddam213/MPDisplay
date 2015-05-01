// <copyright file="ExifProperty.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Drawing.Imaging;

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
        private PropertyItem _propertyItem;

        /// <summary>
        /// The IExifValue associated with this object.
        /// </summary>
        private IExifValue _exifValue;

        /// <summary>
        /// The IExifPropertyFormatter for this property.
        /// </summary>
        private IExifPropertyFormatter _propertyFormatter;

        /// <summary>
        /// Set to true if this object represents an unknown property tag
        /// </summary>
        private bool _isUnknown;

        /// <summary>
        /// Set to true if this object has a custom property formatter
        /// </summary>
        private bool _hasCustomFormatter;

        /// <summary>
        /// The parent ExifReader that owns this ExifProperty object
        /// </summary>
        private ExifReader _parentReader;

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
        public IExifValue ExifValue
        {
            get
            {
                return _exifValue ?? InitializeExifValue();
            }
        }

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
                        String.Format("{0} #{1}", _propertyFormatter.DisplayName, _propertyItem.Id);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "An ExifReaderException was caught. See InnerException for more details",
                        GetExifReaderException(ex));
                }
            }
        }        

        /// <summary>
        /// Gets a category name for the property.
        /// Note: This is not part of the Exif standard and is merely for convenience.
        /// </summary>
        public string ExifPropertyCategory
        {
            get
            {
                return _isUnknown ? "Unknown" : "General";
            }
        }

        /// <summary>
        /// Gets the Exif property tag Id for this property
        /// </summary>
        public PropertyTagId ExifTag
        {
            get
            {
                return _isUnknown ? PropertyTagId.UnknownExifTag : (PropertyTagId)_propertyItem.Id;
            }
        }

        /// <summary>
        /// Gets the Exif data type for this property
        /// </summary>
        public PropertyTagType ExifDatatype
        {
            get
            {
                return (PropertyTagType)_propertyItem.Type;
            }
        }

        /// <summary>
        /// Gets the raw Exif tag. For unknown tags this will not
        /// match the value of the ExifTag property.
        /// </summary>
        public int RawExifTagId
        {
            get
            {
                return _propertyItem.Id;
            }
        }

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
                throw new InvalidOperationException(
                    "An ExifReaderException was caught. See InnerException for more details",
                    GetExifReaderException(ex));
            }
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
            catch (ExifReaderException ex)
            {
                throw new InvalidOperationException(
                    "An ExifReaderException was caught. See InnerException for more details",
                    ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An ExifReaderException was caught. See InnerException for more details",
                    new ExifReaderException(ex, _propertyFormatter, null));
            }
        }

        /// <summary>
        /// Returns an ExifReaderException set with the current property formatter
        /// </summary>
        /// <param name="ex">Inner exception object</param>
        /// <returns>The ExifReaderException object</returns>
        private ExifReaderException GetExifReaderException(Exception ex)
        {
            return new ExifReaderException(ex, _propertyFormatter, null);
        }
    }
}
