// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="LocalizationAttributeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reflection;
using IronyModManager.DI;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Class LocalizationAttributeHandler.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
    public class LocalizationAttributeHandler : ILocalizationAttributeHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can process the specified attribute.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <param name="prop">The property.</param>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if this instance can process the specified attribute; otherwise, <c>false</c>.</returns>
        public bool CanProcess(LocalizationAttributeBase attr, PropertyInfo prop, ILocalizableModel target)
        {
            return attr is LocalizationAttribute;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <param name="prop">The property.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        public string GetData(LocalizationAttributeBase attr, PropertyInfo prop, ILocalizableModel target)
        {
            var locAttr = (LocalizationAttribute)attr;
            return DIResolver.Get<ILocalizationManager>().GetResource(locAttr.ResourceKey);
        }

        /// <summary>
        /// Determines whether the specified attribute has data.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <param name="prop">The property.</param>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the specified attribute has data; otherwise, <c>false</c>.</returns>
        public bool HasData(LocalizationAttributeBase attr, PropertyInfo prop, ILocalizableModel target)
        {
            return true;
        }

        #endregion Methods
    }
}
