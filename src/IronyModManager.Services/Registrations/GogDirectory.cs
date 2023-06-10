
// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 07-15-2022
//
// Last Modified By : Mario
// Last Modified On : 05-26-2023
// ***********************************************************************
// <copyright file="GogDirectory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using IronyModManager.Shared;
using Microsoft.Win32;

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
        /// The game key
        /// </summary>
        // Sure let's break the f*** convention even with string compatibility on
        private const string GameKey = "Software\\\\GOG.com\\\\Games\\\\{Id}";

        /// <summary>
        /// The root key
        /// </summary>
        private const string RootKey = "Software\\GOG.com\\Games";

        /// <summary>
        /// The games
        /// </summary>
        private static IReadOnlyCollection<GogGame> games;

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
                var game = games.FirstOrDefault(p => p.Id == appId.GetValueOrDefault());
                if (game != null)
                {
                    return game.Path;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Fetches the games.
        /// </summary>
        /// <param name="registryHive">The registry hive.</param>
        /// <returns>IReadOnlyCollection&lt;GogGame&gt;.</returns>
        private static IReadOnlyCollection<GogGame> FetchGames(RegistryHive registryHive)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            try
            {
                using var rootKey = registryHive.GetRegistryKey(RootKey);
                if (rootKey != null)
                {
                    var keys = rootKey.GetSubKeyNames();

                    // Yes running on windows only
                    if (keys != null && keys.Any())
                    {
                        var games = keys.AsParallel().Select(p =>
                        {
                            long.TryParse(p, CultureInfo.InvariantCulture, out var id);
                            using var gameKey = registryHive.GetRegistryKey(GameKey.FormatIronySmart(new { Id = id }));
                            return new GogGame()
                            {
                                Id = id,
                                Name = GetKeyValue<string>(gameKey, "gameName"),
                                Path = GetKeyValue<string>(gameKey, "path")
                            };
                        }).ToList();
                        return games;
                    }
                }
            }
            catch
            {
            }
#pragma warning restore CA1416 // Validate platform compatibility
            return null;
        }

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registryKey">The registry key.</param>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        private static T GetKeyValue<T>(RegistryKey registryKey, string key)
        {
            try
            {
#pragma warning disable CA1416 // Validate platform compatibility
                var value = registryKey.GetValue(key);
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
#pragma warning restore CA1416 // Validate platform compatibility
            }
            catch
            {
            }
            return default(T);
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
                var allGames = new List<GogGame>();
                var userGames = FetchGames(RegistryHive.CurrentUser);
                if (userGames != null && userGames.Any())
                {
                    allGames.AddRange(userGames);
                }
                var localGames = FetchGames(RegistryHive.LocalMachine);
                if (localGames != null && localGames.Any())
                {
                    allGames.AddRange(localGames);
                }
                games = allGames;
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class GogGame.
        /// </summary>
        private class GogGame
        {
            #region Properties

            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public long Id { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
