// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="IDefaultLocalizationResourceProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.ResourceProviders
{
    /// <summary>
    /// Interface IDefaultLocalizationResourceProvider
    /// Implements the <see cref="IronyModManager.Localization.ILocalizationResourceProvider" />
    /// Implements the <see cref="IronyModManager.Localization.ResourceProviders.ILocalizationResourceProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ResourceProviders.ILocalizationResourceProvider" />
    /// <seealso cref="IronyModManager.Localization.ILocalizationResourceProvider" />
    public interface IDefaultLocalizationResourceProvider : ILocalizationResourceProvider
    {
        #region Methods

        /// <summary>
        /// Gets the available locales.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAvailableLocales();

        #endregion Methods
    }
}
