// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="StaticLocalizationAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Shared;

namespace IronyModManager.Localization.Attributes
{
    /// <summary>
    /// Class StaticLocalizationAttribute.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [ExcludeFromCoverage("Attributes don't need testing.")]
    public class StaticLocalizationAttribute : LocalizationAttributeBase
    {
        #region Fields

        /// <summary>
        /// The key
        /// </summary>
        private readonly string key;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticLocalizationAttribute" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public StaticLocalizationAttribute(string key)
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
