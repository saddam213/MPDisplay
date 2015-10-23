// <copyright file="ExifReaderTypeDescriptionProvider.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Implements a TypeDescriptionProvider for ExifReader
    /// </summary>
    internal class ExifReaderTypeDescriptionProvider : TypeDescriptionProvider
    {
        /// <summary>
        /// The default TypeDescriptionProvider to use
        /// </summary>
        private static TypeDescriptionProvider _defaultTypeProvider = TypeDescriptor.GetProvider(typeof(ExifReader));

        /// <summary>
        /// Initializes a new instance of the ExifReaderTypeDescriptionProvider class.
        /// </summary>
        public ExifReaderTypeDescriptionProvider()
            : base(_defaultTypeProvider)
        {
        }

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor</param>
        /// <param name="instance">An instance of the type.</param>
        /// <returns>Returns a custom type descriptor</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var defaultDescriptor = base.GetTypeDescriptor(objectType, instance);

            return instance == null ? defaultDescriptor : new ExifReaderCustomTypeDescriptor(defaultDescriptor, instance);
        }
    }
}
