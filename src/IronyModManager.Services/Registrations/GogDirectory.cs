// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 07-15-2022
//
// Last Modified By : erri120
// Last Modified On : 05-10-2023
// ***********************************************************************
// <copyright file="GogDirectory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

#nullable enable
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.GOG;
using IronyModManager.Shared;
using NexusMods.Paths;

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
        private static IReadOnlyDictionary<GOGGameId, GOGGame>? _games;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the game directory.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetGameDirectory(int? appId)
        {
            if (!appId.HasValue) return string.Empty;

            // TODO: GOG is supported in Wine using GameFinder.Wine (https://github.com/erri120/GameFinder/tree/master#gog-galaxy)
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return string.Empty;

            if (_games is null)
            {
                var handler = new GOGHandler(new WindowsRegistry(), FileSystem.Shared);
                _games = handler.FindAllGamesById(out var errors);
                // TODO: log error messages
            }

            return _games.TryGetValue(GOGGameId.From(appId.Value), out var game)
                ? game.Path.GetFullPath()
                : string.Empty;
        }

        #endregion Methods
    }
}
