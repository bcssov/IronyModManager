// ***********************************************************************
// Assembly         : IronyModManager.Storage.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2020
// ***********************************************************************
// <copyright file="IStorageProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Interface IStorageProvider
    /// </summary>
    public interface IStorageProvider
    {
        #region Methods

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <returns>IEnumerable&lt;IGameType&gt;.</returns>
        IEnumerable<IGameType> GetGames();

        /// <summary>
        /// Gets the preferences.
        /// </summary>
        /// <returns>IPreferences.</returns>
        IPreferences GetPreferences();

        /// <summary>
        /// Gets the themes.
        /// </summary>
        /// <returns>IEnumerable&lt;IThemeType&gt;.</returns>
        IEnumerable<IThemeType> GetThemes();

        /// <summary>
        /// Gets the state of the window.
        /// </summary>
        /// <returns>IWindowState.</returns>
        IWindowState GetWindowState();

        /// <summary>
        /// Registers the game.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="userDirectory">The user directory.</param>
        /// <param name="workshopDirectory">The workshop directory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool RegisterGame(string name, int appId, string userDirectory, string workshopDirectory);

        /// <summary>
        /// Registers the theme.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="styles">The styles.</param>
        /// <param name="brushes">The brushes.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool RegisterTheme(string name, IEnumerable<string> styles, IDictionary<string, string> brushes, bool isDefault = false);

        /// <summary>
        /// Sets the preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetPreferences(IPreferences preferences);

        /// <summary>
        /// Sets the state of the window.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetWindowState(IWindowState state);

        #endregion Methods
    }
}
