// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 07-15-2022
//
// Last Modified By : Mario
// Last Modified On : 07-15-2022
// ***********************************************************************
// <copyright file="GogDirectory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GameFinder.StoreHandlers.GOG;
using IronyModManager.Shared;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class GogDirectory.
    /// </summary>
    [ExcludeFromCoverage("Helper setup static class.")]
    public static class GogDirectory
    {
        #region Fields

        /// <summary>
        /// The games
        /// </summary>
        private static List<GOGGame> games;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the game directory.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetGameDirectory(int? appId)
        {
            InitGames();
            if (games != null)
            {
                var game = games.FirstOrDefault(p => p.GameID == appId.GetValueOrDefault());
                if (game != null)
                {
                    return game.Path;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Initializes the games.
        /// </summary>
        private static void InitGames()
        {
            if (games != null)
            {
                return;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var gogHandler = new GOGHandler();
                gogHandler.FindAllGames();
                games = gogHandler.Games;
            }
        }

        #endregion Methods
    }
}
