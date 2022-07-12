// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 07-13-2022
// ***********************************************************************
// <copyright file="DescriptorPropertyAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Mod.Search
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
        /// <param name="isList">if set to <c>true</c> [is list].</param>
        public DescriptorPropertyAttribute(string propertyName, bool isList) : this(propertyName)
        {
            IsList = isList;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is list.
        /// </summary>
        /// <value><c>true</c> if this instance is list; otherwise, <c>false</c>.</value>
        public bool IsList { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; }

        #endregion Properties
    }
}
