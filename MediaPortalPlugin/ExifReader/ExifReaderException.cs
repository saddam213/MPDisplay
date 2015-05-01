// <copyright file="ExifReaderException.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Represents an exception that is thrown whenever the ExifReader catches
    /// any exception when applying a formatter or an extractor.
    /// </summary>
    [Serializable]
    public class ExifReaderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ExifReaderException class.
        /// </summary>
        public ExifReaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExifReaderException class with the specific arguments
        /// </summary>
        /// <param name="innerException">The source exception</param>
        internal ExifReaderException(Exception innerException)
            : this(innerException, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExifReaderException class with the specific arguments
        /// </summary>
        /// <param name="message">The error message for the exception</param>
        /// <param name="innerException">The source exception</param>
        internal ExifReaderException(string message, Exception innerException)
            : this(message, innerException, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExifReaderException class with the specific arguments
        /// </summary>
        /// <param name="innerException">The source exception</param>
        /// <param name="propertyFormatter">The property formatter if any</param>
        internal ExifReaderException(Exception innerException, IExifPropertyFormatter propertyFormatter)
            : this(innerException, propertyFormatter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExifReaderException class with the specific arguments
        /// </summary>
        /// <param name="innerException">The source exception</param>
        /// <param name="undefinedExtractor">The undefined extractor if any</param>
        internal ExifReaderException(Exception innerException, IExifValueUndefinedExtractor undefinedExtractor)
            : this(innerException, null, undefinedExtractor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExifReaderException class with the specific arguments
        /// </summary>
        /// <param name="innerException">The source exception</param>
        /// <param name="propertyFormatter">The property formatter if any</param>
        /// <param name="undefinedExtractor">The undefined extractor if any</param>
        internal ExifReaderException(Exception innerException, IExifPropertyFormatter propertyFormatter, IExifValueUndefinedExtractor undefinedExtractor)
            : this(
                String.Format("There was a problem handling an Exif tag.\r\n" + 
                    "The PropertyFormatter or UndefinedExtractor properties should indicate the cause of the problem.\r\n" + 
                    "See the InnerException for more info on the source exception that was thrown.\r\n"), 
                innerException, 
                propertyFormatter, 
                undefinedExtractor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExifReaderException class with the specific arguments
        /// </summary>
        /// <param name="message">The error message for the exception</param>
        /// <param name="innerException">The source exception</param>
        /// <param name="propertyFormatter">The property formatter if any</param>
        /// <param name="undefinedExtractor">The undefined extractor if any</param>
        internal ExifReaderException(string message, Exception innerException, IExifPropertyFormatter propertyFormatter, IExifValueUndefinedExtractor undefinedExtractor)
            : base(message, innerException)
        {
            PropertyFormatter = propertyFormatter;
            UndefinedExtractor = undefinedExtractor;
        }

        /// <summary>
        /// Gets the property formatter used at the time of exception
        /// </summary>
        public IExifPropertyFormatter PropertyFormatter { get; private set; }

        /// <summary>
        /// Gets the undefined extractor used at the time of exception
        /// </summary>
        public IExifValueUndefinedExtractor UndefinedExtractor { get; private set; }

        /// <summary>
        /// Sets info into the SerializationInfo object
        /// </summary>
        /// <param name="info">The serialized object data on the exception being thrown</param>
        /// <param name="context">Contaisn context info</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PropertyFormatter", PropertyFormatter, typeof(IExifPropertyFormatter));
            info.AddValue("UndefinedExtractor", UndefinedExtractor, typeof(IExifValueUndefinedExtractor));
        }
    }
}
