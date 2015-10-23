// <copyright file="QueryPropertyFormatterEventArgs.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Provides data for the QueryPropertyFormatter event
    /// </summary>
    public class QueryPropertyFormatterEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the QueryPropertyFormatterEventArgs class.
        /// </summary>
        /// <param name="tagId">The tag Id to query a property formatter for</param>
        public QueryPropertyFormatterEventArgs(int tagId)
        {
            TagId = tagId;
        }

        /// <summary>
        /// Gets or sets the associated property formatter
        /// </summary>
        public IExifPropertyFormatter PropertyFormatter { get; set; }

        /// <summary>
        /// Gets the associated tag Id
        /// </summary>
        public int TagId { get; private set; }
    }
}
