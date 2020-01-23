// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-23-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="ILocalizationRefreshHandlers.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Interface ILocalizationRefreshHandler
    /// </summary>
    public interface ILocalizationRefreshHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can refresh the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can refresh the specified arguments; otherwise, <c>false</c>.</returns>
        bool CanRefresh(LocalizationRefreshArgs args);

        /// <summary>
        /// Refreshes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        void Refresh(LocalizationRefreshArgs args);

        #endregion Methods
    }
}
