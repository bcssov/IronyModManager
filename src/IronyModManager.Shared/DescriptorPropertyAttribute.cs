// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 03-31-2020
// ***********************************************************************
// <copyright file="DescriptorPropertyAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class DescriptorPropertyAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DescriptorPropertyAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public DescriptorPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }

        #endregion Properties
    }
}
