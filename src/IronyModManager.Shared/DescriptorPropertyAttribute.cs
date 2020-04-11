// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-11-2020
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
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public DescriptorPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="alternatePropertyName">Name of the alternate property.</param>
        /// <param name="alternateNameEndsWithCondition">The alternate name ends with condition.</param>
        public DescriptorPropertyAttribute(string propertyName, string alternatePropertyName, string alternateNameEndsWithCondition) : this(propertyName)
        {
            AlternatePropertyName = alternatePropertyName;
            AlternateNameEndsWithCondition = alternateNameEndsWithCondition;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the alternate name ends with condition.
        /// </summary>
        /// <value>The alternate name ends with condition.</value>
        public string AlternateNameEndsWithCondition { get; }

        /// <summary>
        /// Gets the name of the alternate property.
        /// </summary>
        /// <value>The name of the alternate property.</value>
        public string AlternatePropertyName { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; }

        #endregion Properties
    }
}
