// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="IGameService.cs" company="Mario">
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
    /// Interface IGameService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IGameService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Exports the hash report asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportHashReportAsync(IGame game, string path);

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;IGame&gt;.</returns>
        IEnumerable<IGame> Get();

        /// <summary>
        /// Gets the default game settings.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IGameSettings.</returns>
        IGameSettings GetDefaultGameSettings(IGame game);

        /// <summary>
        /// Gets the game settings from json.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>IGameSettings.</returns>
        IGameSettings GetGameSettingsFromJson(IGame game, string path);

        /// <summary>
        /// Gets the launch settings.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="appendContinueGame">if set to <c>true</c> [append continue game].</param>
        /// <returns>IGameSettings.</returns>
        IGameSettings GetLaunchSettings(IGame game, bool appendContinueGame = false);

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>IGame.</returns>
        IGame GetSelected();

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        string GetVersion(IGame game);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        IEnumerable<string> GetVersions(IGame game);

        /// <summary>
        /// Imports the hash report asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>Task&lt;IEnumerable&lt;IHashReport&gt;&gt;.</returns>
        Task<IEnumerable<IHashReport>> ImportHashReportAsync(IGame game, IReadOnlyCollection<IHashReport> hashReports);

        /// <summary>
        /// Determines whether game has an active save and game continue logic can be executed.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if game has an active save; otherwise, <c>false</c>.</returns>
        bool IsContinueGameAllowed(IGame game);

        /// <summary>
        /// Determines whether game is a flatpak steam installation.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if flatpak steam installation; otherwise, <c>false</c>.</returns>
        bool IsFlatpakSteamGame(IGameSettings settings);

        /// <summary>
        /// Determines whether [is steam game] [the specified settings].
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if [is steam game] [the specified settings]; otherwise, <c>false</c>.</returns>
        bool IsSteamGame(IGameSettings settings);

        /// <summary>
        /// Determines whether [is steam launch path] [the specified settings].
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if [is steam launch path] [the specified settings]; otherwise, <c>false</c>.</returns>
        bool IsSteamLaunchPath(IGameSettings settings);

        /// <summary>
        /// Saves the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(IGame game);

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="games">The games.</param>
        /// <param name="selectedGame">The selected game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetSelected(IEnumerable<IGame> games, IGame selectedGame);

        #endregion Methods
    }
}
