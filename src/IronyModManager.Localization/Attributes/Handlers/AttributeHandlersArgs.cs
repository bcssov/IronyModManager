// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-23-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="AttributeHandlersArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Class AttributeHandlersArgs.
    /// </summary>
    public class AttributeHandlersArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeHandlersArgs" /> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="invocation">The invocation.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public AttributeHandlersArgs(LocalizationAttributeBase attribute, IInvocation invocation, PropertyInfo property, object value)
        {
            Attribute = attribute;
            Invocation = invocation;
            Value = value;
            Property = property;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>The attribute.</value>
        public LocalizationAttributeBase Attribute { get; set; }

        /// <summary>
        /// Gets or sets the invocation.
        /// </summary>
        /// <value>The invocation.</value>
        public IInvocation Invocation { get; set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        #endregion Properties
    }
}
