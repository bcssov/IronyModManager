// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 04-16-2021
//
// Last Modified By : Mario
// Last Modified On : 04-16-2021
// ***********************************************************************
// <copyright file="IPlatformConfiguration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Platform.Configuration
{
    /// <summary>
    /// Interface IPlatformConfiguration
    /// </summary>
    public interface IPlatformConfiguration
    {
        #region Methods

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns>PlatformConfigurationOptions.</returns>
        PlatformConfigurationOptions GetOptions();

        #endregion Methods
    }
}
