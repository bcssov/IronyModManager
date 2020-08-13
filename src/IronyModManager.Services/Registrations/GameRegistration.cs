// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="GameRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using IronyModManager.DI;
using IronyModManager.Services.Registrations.Models;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using Newtonsoft.Json;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class GameRegistration.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    [ExcludeFromCoverage("Setup module.")]
    public class GameRegistration : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            var storage = DIResolver.Get<IStorageProvider>();
            var userDir = UserDirectory.GetDirectory();
            storage.RegisterGame(GetStellaris(userDir));
        }

        /// <summary>
        /// Gets the executable launcher path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="launcherFilename">The launcher filename.</param>
        /// <returns>System.String.</returns>
        private ExecutableSettings GetExecutableSettings(string path, string launcherFilename)
        {
            if (File.Exists(Path.Combine(path, launcherFilename)))
            {
                var text = File.ReadAllText(Path.Combine(path, launcherFilename));
                if (!string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        var settings = JsonConvert.DeserializeObject<LauncherSettings>(text);
                        var exePath = Path.Combine(path, settings.ExePath).StandardizeDirectorySeparator();
                        if (File.Exists(exePath))
                        {
                            return new ExecutableSettings()
                            {
                                ExecutableArgs = string.Join(" ", settings.ExeArgs),
                                ExecutablePath = Path.Combine(path, settings.ExePath).StandardizeDirectorySeparator()
                            };
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the stellaris.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetStellaris(string baseUserDir)
        {
            var stellaris = DIResolver.Get<IGameType>();
            stellaris.ChecksumFolders = Shared.Constants.GamesTypes.Stellaris.ChecksumFolders;
            stellaris.GameFolders = Shared.Constants.GamesTypes.Stellaris.GameFolders;
            stellaris.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Stellaris.Name), Shared.Constants.GamesTypes.Stellaris.LogLocation).StandardizeDirectorySeparator();
            stellaris.Name = Shared.Constants.GamesTypes.Stellaris.Name;
            stellaris.SteamAppId = Shared.Constants.GamesTypes.Stellaris.SteamAppId;
            stellaris.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Stellaris.Name).StandardizeDirectorySeparator();
            stellaris.WorkshopDirectory = SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId).StandardizeDirectorySeparator();
            stellaris.BaseGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId).StandardizeDirectorySeparator();
            stellaris.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            stellaris.AdvancedFeaturesSupported = true;
            MapExecutableSettings(stellaris, GetExecutableSettings(stellaris.BaseGameDirectory, stellaris.LauncherSettingsFileName));
            return stellaris;
        }

        /// <summary>
        /// Maps the executable settings.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="settings">The settings.</param>
        private void MapExecutableSettings(IGameType gameType, ExecutableSettings settings)
        {
            if (settings != null && gameType != null)
            {
                gameType.ExecutablePath = settings.ExecutablePath;
                gameType.ExecutableArgs = settings.ExecutableArgs;
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ExecutableSettings.
        /// </summary>
        private class ExecutableSettings
        {
            #region Properties

            /// <summary>
            /// Gets or sets the executable arguments.
            /// </summary>
            /// <value>The executable arguments.</value>
            public string ExecutableArgs { get; set; }

            /// <summary>
            /// Gets or sets the executable path.
            /// </summary>
            /// <value>The executable path.</value>
            public string ExecutablePath { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
