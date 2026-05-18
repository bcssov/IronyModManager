// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="DescriptorPropertyAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class DescriptorPropertyAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class DescriptorPropertyAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="keyedArray">if set to <c>true</c> [keyed array].</param>
        public DescriptorPropertyAttribute(string propertyName, bool keyedArray) : this(propertyName)
        {
            KeyedArray = keyedArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public DescriptorPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
            AlternateNameEndsWithCondition = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="alternatePropertyName">Name of the alternate property.</param>
        /// <param name="alternateNameEndsWithCondition">The alternate name ends with condition.</param>
        public DescriptorPropertyAttribute(string propertyName, string alternatePropertyName, params string[] alternateNameEndsWithCondition) : this(propertyName, false)
        {
            AlternatePropertyName = alternatePropertyName;
            AlternateNameEndsWithCondition = alternateNameEndsWithCondition != null ? alternateNameEndsWithCondition.ToList() : [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="alternatePropertyName">Name of the alternate property.</param>
        /// <param name="translateProtonPath">if set to <c>true</c> [translate proton path].</param>
        /// <param name="alternateNameEndsWithCondition">The alternate name ends with condition.</param>
        public DescriptorPropertyAttribute(string propertyName, string alternatePropertyName, bool translateProtonPath, params string[] alternateNameEndsWithCondition) : this(propertyName, alternatePropertyName,
            alternateNameEndsWithCondition)
        {
            TranslateProtonPath = translateProtonPath;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the alternate name ends with condition.
        /// </summary>
        /// <value>The alternate name ends with condition.</value>
        public IEnumerable<string> AlternateNameEndsWithCondition { get; }

        /// <summary>
        /// Gets the name of the alternate property.
        /// </summary>
        /// <value>The name of the alternate property.</value>
        public string AlternatePropertyName { get; }

        /// <summary>
        /// Gets a value indicating whether [keyed array].
        /// </summary>
        /// <value><c>true</c> if [keyed array]; otherwise, <c>false</c>.</value>
        public bool KeyedArray { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [translate proton path].
        /// </summary>
        /// <value><c>true</c> if [translate proton path]; otherwise, <c>false</c>.</value>
        public bool TranslateProtonPath { get; set; }

        #endregion Properties
    }
}
