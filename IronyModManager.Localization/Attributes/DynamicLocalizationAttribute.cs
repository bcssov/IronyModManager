// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="DynamicLocalizationAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes
{
    /// <summary>
    /// Class DynamicLocalizationAttribute.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicLocalizationAttribute : LocalizationAttributeBase
    {
        #region Fields

        /// <summary>
        /// The dependent property
        /// </summary>
        private readonly string dependentProperty;

        /// <summary>
        /// The resource prefix
        /// </summary>
        private readonly string resourcePrefix;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicLocalizationAttribute" /> class.
        /// </summary>
        /// <param name="dependentProperty">The dependent property.</param>
        public DynamicLocalizationAttribute(string dependentProperty) : this(string.Empty, dependentProperty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicLocalizationAttribute" /> class.
        /// </summary>
        /// <param name="resourcePrefix">The resource prefix.</param>
        /// <param name="dependentProperty">The dependent property.</param>
        public DynamicLocalizationAttribute(string resourcePrefix, string dependentProperty)
        {
            this.resourcePrefix = resourcePrefix;
            this.dependentProperty = dependentProperty;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the dependent property.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetDependentProperty()
        {
            return dependentProperty;
        }

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
