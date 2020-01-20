// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="LocalizationAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class LocalizationAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LocalizationAttribute : Attribute
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
                var localizationManager = DI.DIResolver.Get<ILocalizationManager>();
                return localizationManager.GetResource(key);
            }
        }

        #endregion Properties
    }
}
