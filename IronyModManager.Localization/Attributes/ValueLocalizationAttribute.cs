// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="ValueLocalizationAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes
{
    /// <summary>
    /// Class ValueLocalizationAttribute.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValueLocalizationAttribute : LocalizationAttributeBase
    {
        #region Fields

        /// <summary>
        /// The resource prefix
        /// </summary>
        private readonly string resourcePrefix;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueLocalizationAttribute" /> class.
        /// </summary>
        public ValueLocalizationAttribute() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueLocalizationAttribute" /> class.
        /// </summary>
        /// <param name="resourcePrefix">The resource prefix.</param>
        public ValueLocalizationAttribute(string resourcePrefix)
        {
            this.resourcePrefix = resourcePrefix;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Resources the prefix.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string ResourcePrefix()
        {
            return $"{resourcePrefix}";
        }

        #endregion Methods
    }
}
