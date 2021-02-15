// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-15-2021
// ***********************************************************************
// <copyright file="GameRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Services.Models;
using IronyModManager.Services.Resolver;
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
        #region Fields

        /// <summary>
        /// The path resolver
        /// </summary>
        private PathResolver pathResolver;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            pathResolver = new PathResolver();
            var storage = DIResolver.Get<IStorageProvider>();
            var userDir = UserDirectory.GetDirectory();
            storage.RegisterGame(GetStellaris(userDir));
            storage.RegisterGame(GetEUIV(userDir));
            storage.RegisterGame(GetHOI4(userDir));
            storage.RegisterGame(GetImperator(userDir));
            storage.RegisterGame(GetCK3(userDir));
        }

        /// <summary>
        /// Gets the c k3.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetCK3(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.ChecksumFolders = Shared.Constants.GamesTypes.CrusaderKings3.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.CrusaderKings3.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.CrusaderKings3.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.CrusaderKings3.Id;
            game.SteamAppId = Shared.Constants.GamesTypes.CrusaderKings3.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.CrusaderKings3.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.CrusaderKings3.SteamAppId).StandardizeDirectorySeparator();
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.CrusaderKings3.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.CrusaderKings3.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = Shared.Constants.GamesTypes.CrusaderKings3.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator()).ToList();
            game.AdvancedFeaturesSupported = false;
            MapGameSettings(game, GetExecutableSettings(game.BaseSteamGameDirectory, game.LauncherSettingsFileName));
            return game;
        }

        /// <summary>
        /// Gets the euiv.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetEUIV(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.ChecksumFolders = Shared.Constants.GamesTypes.EuropaUniversalis4.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.EuropaUniversalis4.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.EuropaUniversalis4.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.EuropaUniversalis4.Id;
            game.SteamAppId = Shared.Constants.GamesTypes.EuropaUniversalis4.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.EuropaUniversalis4.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.EuropaUniversalis4.SteamAppId).StandardizeDirectorySeparator();
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.EuropaUniversalis4.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator()).ToList();
            game.AdvancedFeaturesSupported = false;
            MapGameSettings(game, GetExecutableSettings(game.BaseSteamGameDirectory, game.LauncherSettingsFileName));
            return game;
        }

        /// <summary>
        /// Gets the executable launcher path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="launcherFilename">The launcher filename.</param>
        /// <returns>System.String.</returns>
        private GameSettings GetExecutableSettings(string path, string launcherFilename)
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
                            return new GameSettings()
                            {
                                ExecutableArgs = string.Join(" ", settings.ExeArgs),
                                ExecutablePath = Path.Combine(path, settings.ExePath).StandardizeDirectorySeparator(),
                                UserDir = pathResolver.Parse(settings.GameDataPath)
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
        /// Gets the ho i4.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetHOI4(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.ChecksumFolders = Shared.Constants.GamesTypes.HeartsOfIron4.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.HeartsOfIron4.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.HeartsOfIron4.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.HeartsOfIron4.Id;
            game.SteamAppId = Shared.Constants.GamesTypes.HeartsOfIron4.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.HeartsOfIron4.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.HeartsOfIron4.SteamAppId).StandardizeDirectorySeparator();
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.HeartsOfIron4.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator()).ToList();
            game.AdvancedFeaturesSupported = false;
            MapGameSettings(game, GetExecutableSettings(game.BaseSteamGameDirectory, game.LauncherSettingsFileName));
            return game;
        }

        /// <summary>
        /// Gets the imperator.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetImperator(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.ChecksumFolders = Shared.Constants.GamesTypes.ImperatorRome.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.ImperatorRome.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.ImperatorRome.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.ImperatorRome.Id;
            game.SteamAppId = Shared.Constants.GamesTypes.ImperatorRome.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.ImperatorRome.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.ImperatorRome.SteamAppId).StandardizeDirectorySeparator();
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.ImperatorRome.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.ImperatorRome.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = Shared.Constants.GamesTypes.ImperatorRome.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator()).ToList();
            game.AdvancedFeaturesSupported = false;
            MapGameSettings(game, GetExecutableSettings(game.BaseSteamGameDirectory, game.LauncherSettingsFileName));
            return game;
        }

        /// <summary>
        /// Gets the stellaris.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetStellaris(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.ChecksumFolders = Shared.Constants.GamesTypes.Stellaris.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.Stellaris.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Stellaris.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.Stellaris.Id;
            game.SteamAppId = Shared.Constants.GamesTypes.Stellaris.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Stellaris.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId).StandardizeDirectorySeparator();
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator()).ToList();
            game.AdvancedFeaturesSupported = true;
            MapGameSettings(game, GetExecutableSettings(game.BaseSteamGameDirectory, game.LauncherSettingsFileName));
            return game;
        }

        /// <summary>
        /// Maps the executable settings.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="settings">The settings.</param>
        private void MapGameSettings(IGameType gameType, GameSettings settings)
        {
            if (settings != null && gameType != null)
            {
                gameType.ExecutablePath = settings.ExecutablePath;
                gameType.ExecutableArgs = settings.ExecutableArgs;
                if (Directory.Exists(settings.UserDir))
                {
                    gameType.UserDirectory = settings.UserDir;
                }
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ExecutableSettings.
        /// </summary>
        private class GameSettings
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

            /// <summary>
            /// Gets or sets the user dir.
            /// </summary>
            /// <value>The user dir.</value>
            public string UserDir { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
