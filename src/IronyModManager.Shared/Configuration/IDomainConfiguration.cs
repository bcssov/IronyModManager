// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-24-2022
//
// Last Modified By : Mario
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="IDomainConfiguration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Configuration
{
    /// <summary>
    /// Interface IDomainConfiguration
    /// </summary>
    public interface IDomainConfiguration
    {
        #region Methods

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns>DomainConfigurationOptions.</returns>
        DomainConfigurationOptions GetOptions();

        #endregion Methods
    }
}
