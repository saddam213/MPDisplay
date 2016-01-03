// <copyright file="ExifReaderCustomTypeDescriptor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Implements a CustomTypeDescriptor for the ExifReader class
    /// </summary>
    internal class ExifReaderCustomTypeDescriptor : CustomTypeDescriptor
    {
        /// <summary>
        /// List of custom fields
        /// </summary>
        private readonly List<PropertyDescriptor> _customFields = new List<PropertyDescriptor>();

        /// <summary>
        ///  Initializes a new instance of the ExifReaderCustomTypeDescriptor class.
        /// </summary>
        /// <param name="parent">The parent custom type descriptor.</param>
        /// <param name="instance">Instance of ExifReader</param>
        public ExifReaderCustomTypeDescriptor(ICustomTypeDescriptor parent, object instance)
            : base(parent)
        {
            var exifReader = (ExifReader)instance;
            _customFields.AddRange(exifReader.GetExifProperties().Select(ep => new ExifPropertyPropertyDescriptor(ep)));
        }

        /// <summary>
        /// Returns a collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <returns>A collection of property descriptors</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(base.GetProperties().Cast<PropertyDescriptor>().Union(_customFields).ToArray());
        }

        /// <summary>
        /// Returns a collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <param name="attributes">Attributes to filter on</param>
        /// <returns>A collection of property descriptors</returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(base.GetProperties(attributes).Cast<PropertyDescriptor>().Union(_customFields).ToArray());
        }
    }
}
