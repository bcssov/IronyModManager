// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-18-2020
// ***********************************************************************
// <copyright file="ILocalizationResourceProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Interface ILocalizationResourceProvider
    /// </summary>
    public interface ILocalizationResourceProvider
    {
        #region Properties

        /// <summary>
        /// Gets the resource path.
        /// </summary>
        /// <value>The resource path.</value>
        string ResourcePath { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Reads the resource.
        /// </summary>
        /// <returns>System.String.</returns>
        string ReadResource();

        #endregion Methods

    }
}
