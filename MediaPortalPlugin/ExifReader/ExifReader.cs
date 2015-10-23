// <copyright file="ExifReader.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Common.Log;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// This is the implementation of the ExifReader class that reads EXIF data from image files.
    /// It partially supports the Exif Version 2.2 standard.
    /// </summary>
    [TypeDescriptionProvider(typeof(ExifReaderTypeDescriptionProvider))]
    public class ExifReader
    {
        /// <summary>
        /// List of Exif properties for the image.
        /// </summary>
        private List<ExifProperty> _exifproperties;

        /// <summary>
        /// Initializes a new instance of the ExifReader class based on a file path.
        /// </summary>
        /// <param name="imageFileName">Full path to the image file</param>
        public ExifReader(string imageFileName)
        {
            try
            {
                if (File.Exists(imageFileName))
                {
                    using (var stream = File.Open(imageFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                     var imageFile = Image.FromStream(stream);
                     InitializeExifProperties(imageFile);
                     imageFile.Dispose();
                    }
                }
                else
                {
                    LoggingManager.GetLog(typeof(ExifReader)).Message(LogLevel.Info, "Image file <{0}> does not exist.", imageFileName);
                }
            }
            catch (Exception ex)
            {
                _exifproperties = null;
                LoggingManager.GetLog(typeof(ExifReader)).Message(LogLevel.Error, "Error initializing for file <{0}>: {1}.", imageFileName, ex);
            }
        }

        /// <summary>
        /// Occurs when the class needs to query for a property formatter
        /// </summary>
        public event EventHandler<QueryPropertyFormatterEventArgs> QueryPropertyFormatter;

        /// <summary>
        /// Occurs when the class needs to query for an undefined extractor
        /// </summary>
        public event EventHandler<QueryUndefinedExtractorEventArgs> QueryUndefinedExtractor;

        /// <summary>
        /// Returns a read-only collection of all the Exif properties
        /// </summary>
        /// <returns>The Exif properties</returns>
        public ReadOnlyCollection<ExifProperty> GetExifProperties()
        {
            return _exifproperties != null ? _exifproperties.AsReadOnly() : null;
        }

        /// <summary>
        /// Checks to see if a custom property formatter is available
        /// </summary>
        /// <param name="tagId">The tag Id to check for a formatter</param>
        /// <returns>An IExifPropertyFormatter or null if there's no formatter available</returns>
        internal IExifPropertyFormatter QueryForCustomPropertyFormatter(int tagId)
        {
            var eventArgs = new QueryPropertyFormatterEventArgs(tagId);
            FireQueryPropertyFormatter(eventArgs);
            return eventArgs.PropertyFormatter;
        }

        /// <summary>
        /// Checks to see if a custom undefined extractor is available
        /// </summary>
        /// <param name="tagId">The tag Id to check for an extractor</param>
        /// <returns>An IExifValueUndefinedExtractor or null if there's no formatter available</returns>
        internal IExifValueUndefinedExtractor QueryForCustomUndefinedExtractor(int tagId)
        {
            var eventArgs = new QueryUndefinedExtractorEventArgs(tagId);
            FireQueryUndefinedExtractor(eventArgs);
            return eventArgs.UndefinedExtractor;
        }

        /// <summary>
        /// Fires the QueryPropertyFormatter event
        /// </summary>
        /// <param name="eventArgs">Args data for the QueryPropertyFormatter event</param>
        private void FireQueryPropertyFormatter(QueryPropertyFormatterEventArgs eventArgs)
        {
            var queryPropertyFormatter = QueryPropertyFormatter;

            if (queryPropertyFormatter != null)
            {
                queryPropertyFormatter(this, eventArgs);
            }
        }

        /// <summary>
        /// Fires the QueryUndefinedExtractor event
        /// </summary>
        /// <param name="eventArgs">Args data for the QueryUndefinedExtractor event</param>
        private void FireQueryUndefinedExtractor(QueryUndefinedExtractorEventArgs eventArgs)
        {
            var queryUndefinedExtractor = QueryUndefinedExtractor;

            if (queryUndefinedExtractor != null)
            {
                queryUndefinedExtractor(this, eventArgs);
            }
        }

        /// <summary>
        /// Initializes the Exif properties for the associated image file
        /// </summary>
        private void InitializeExifProperties(Image imageFile)
        {
            _exifproperties = new List<ExifProperty>();

            if (imageFile == null) return;

            foreach (var propertyItem in imageFile.PropertyItems)
            {
                _exifproperties.Add(new ExifProperty(propertyItem, this));
            }
        }
    }
}
