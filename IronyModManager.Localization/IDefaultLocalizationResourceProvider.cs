// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="IDefaultLocalizationResourceProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Interface IDefaultLocalizationResourceProvider
    /// Implements the <see cref="IronyModManager.Localization.ILocalizationResourceProvider" />
    /// </summary>
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
