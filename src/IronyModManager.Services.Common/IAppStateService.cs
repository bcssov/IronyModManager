// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="IAppStateService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IAppStateService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IAppStateService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IAppState.</returns>
        IAppState Get();

        /// <summary>
        /// Saves the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(IAppState state);

        #endregion Methods
    }
}
