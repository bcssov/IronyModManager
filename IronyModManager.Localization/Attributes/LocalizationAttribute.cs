// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="LocalizationAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;

namespace IronyModManager.Localization.Attributes
{
    /// <summary>
    /// Class LocalizationAttribute.
    /// Implements the <see cref="IronyModManager.Localization.LocalizationAttributeBase" />
    /// Implements the <see cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    /// <seealso cref="IronyModManager.Localization.LocalizationAttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LocalizationAttribute : LocalizationAttributeBase
    {
        #region Fields

        /// <summary>
        /// The key
        /// </summary>
        private readonly string key;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationAttribute" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public LocalizationAttribute(string key)
        {
            this.key = key;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the resource key.
        /// </summary>
        /// <value>The resource key.</value>
        public virtual string ResourceKey
        {
            get
            {
                return key;
            }
        }

        #endregion Properties
    }
}
