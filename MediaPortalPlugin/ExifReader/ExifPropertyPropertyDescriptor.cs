// <copyright file="ExifPropertyPropertyDescriptor.cs" company="Nish Sivakumar">
// Copyright (c) Nish Sivakumar. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;

namespace MediaPortalPlugin.ExifReader
{
    /// <summary>
    /// Implements a PropertyDescriptor for ExifProperty that returns the descriptive
    /// string representation of the property's current value.
    /// </summary>
    internal class ExifPropertyPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the ExifPropertyPropertyDescriptor class.
        /// </summary>
        /// <param name="exifProperty">The ExifProperty to use with this instance</param>
         public ExifPropertyPropertyDescriptor(ExifProperty exifProperty)
            : base(exifProperty.ExifPropertyName, new Attribute[] { new CategoryAttribute(exifProperty.ExifPropertyCategory) })
        {
            ExifProperty = exifProperty;
        }       
        
        /// <summary>
        /// Gets the ExifProperty associated with this instance
        /// </summary>
        public ExifProperty ExifProperty { get; private set; }

        /// <summary>
        /// Gets the type of the component this property is bound to
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(ExifReader); }
        }

        /// <summary>
        /// Gets a value indicating whether this property is read-only
        /// </summary>
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get { return typeof(string); }
        }

        /// <summary>
        /// Indicates if the component can be reset
        /// </summary>
        /// <param name="component">The component to test</param>
        /// <returns>A value indicating whether this component can be reset</returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Gets the current value of the property
        /// </summary>
        /// <param name="component">The associated component</param>
        /// <returns>The property value</returns>
        public override object GetValue(object component)
        {
            return ExifProperty.ToString();
        }

        /// <summary>
        /// Resets the value of this property
        /// </summary>
        /// <param name="component">The associated component</param>
        public override void ResetValue(object component)
        {
            // We don't need to support this
        }

        /// <summary>
        /// Sets the value of the property
        /// </summary>
        /// <param name="component">The associated component</param>
        /// <param name="value">The new value to set</param>
        public override void SetValue(object component, object value)
        {
            // We don't need to support this
        }

        /// <summary>
        /// Gets a value indicating whether to serialize this property
        /// </summary>
        /// <param name="component">The associated component</param>
        /// <returns>A bool indicating whether to serialize this property or not</returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
