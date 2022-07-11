// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="IExternalProcessHandlerService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IExternalProcessHandlerService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IExternalProcessHandlerService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Launches the steam asynchronous.
        /// </summary>
        /// <param name="useLegacyMethod">if set to <c>true</c> [use legacy method].</param>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> LaunchSteamAsync(bool useLegacyMethod, IGame game);

        #endregion Methods
    }
}
