// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
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
        /// Determines whether Paradox Launcher is running.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> IsParadoxLauncherRunningAsync();

        /// <summary>
        /// Launches the steam process either via Irony or game launcher process and tries to execute a handshake.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="forceLegacyMode">if set to <c>true</c> forces legacy mode typically for flatpak binaries.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> LaunchSteamAsync(IGame game, bool forceLegacyMode = false);

        #endregion Methods
    }
}
