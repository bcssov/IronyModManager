// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-23-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="LocalizationRefreshArgs.cs" company="Mario">
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
    /// Class LocalizationRefreshArgs.
    /// </summary>
    public class LocalizationRefreshArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationRefreshArgs" /> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="prop">The property.</param>
        public LocalizationRefreshArgs(IInvocation invocation, PropertyInfo prop)
        {
            Property = prop;
            Invocation = invocation;
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties
    }
}
