// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
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
using System.Runtime.InteropServices;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.Models.Common;
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
        /// The hoi4 cache version
        /// </summary>
        private const int HOI4CacheVersion = 20;

        /// <summary>
        /// The stellaris cache version
        /// </summary>
        private const int StellarisCacheVersion = 31;

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
            storage.RegisterGame(GetVicky3(userDir));
            storage.RegisterGame(GetSTInfinite(userDir));
            storage.RegisterGame(GetEUV(userDir));
        }

        /// <summary>
        /// Gets the ck3 registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        // ReSharper disable once InconsistentNaming
        private IGameType GetCK3(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.CrusaderKings3.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.CrusaderKings3.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.CrusaderKings3.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.CrusaderKings3.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.CrusaderKings3.Id;
            game.Abrv = Shared.Constants.GamesTypes.CrusaderKings3.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.CrusaderKings3.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.CrusaderKings3.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.CrusaderKings3.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.CrusaderKings3.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.CrusaderKings3.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = Shared.Constants.GamesTypes.CrusaderKings3.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.None;
            game.ParadoxGameId = Shared.Constants.GamesTypes.CrusaderKings3.ParadoxGameId;
            game.SupportedMergeTypes = SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.DescriptorMod;
            game.GameIndexCacheVersion = 1;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the EU4 registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        // ReSharper disable once InconsistentNaming
        private IGameType GetEUIV(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.EuropaUniversalis4.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.EuropaUniversalis4.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.EuropaUniversalis4.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.EuropaUniversalis4.Id;
            game.Abrv = Shared.Constants.GamesTypes.EuropaUniversalis4.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.EuropaUniversalis4.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.EuropaUniversalis4.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.EuropaUniversalis4.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.EuropaUniversalis4.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.None;
            game.ParadoxGameId = Shared.Constants.GamesTypes.EuropaUniversalis4.ParadoxGameId;
            game.SupportedMergeTypes = SupportedMergeTypes.Zip | SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.DescriptorMod;
            game.GameIndexCacheVersion = 1;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the EUV registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        // ReSharper disable once InconsistentNaming
        private IGameType GetEUV(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.EuropaUniversalis5.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.EuropaUniversalis5.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.EuropaUniversalis5.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.EuropaUniversalis5.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.EuropaUniversalis5.Id;
            game.Abrv = Shared.Constants.GamesTypes.EuropaUniversalis5.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.EuropaUniversalis5.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.EuropaUniversalis5.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.EuropaUniversalis5.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.EuropaUniversalis5.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.EuropaUniversalis5.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = Shared.Constants.GamesTypes.EuropaUniversalis5.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.None;
            game.ParadoxGameId = Shared.Constants.GamesTypes.EuropaUniversalis5.ParadoxGameId;
            game.SupportedMergeTypes = SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.JsonMetadataV2;
            game.GameIndexCacheVersion = 1;
            game.SignatureFiles = Shared.Constants.GamesTypes.EuropaUniversalis5.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;

            // Game is so far windows only it seems...
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.EuropaUniversalis5.DefaultWindowsGameBinaryPath;
            }

            game.SupportedOperatingSystems = SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the executable launcher path.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        private GameSettings GetExecutableSettings(IGameType game)
        {
            var basePath = game.BaseSteamGameDirectory;
            if (!string.IsNullOrWhiteSpace(game.LauncherSettingsFileName))
            {
                var path = Path.Combine(basePath, game.LauncherSettingsFileName);
                if (!File.Exists(path) && game.GogAppId.HasValue)
                {
                    basePath = GogDirectory.GetGameDirectory(game.GogAppId.GetValueOrDefault());
                    if (!string.IsNullOrWhiteSpace(basePath))
                    {
                        path = Path.Combine(basePath, game.LauncherSettingsFileName);
                    }
                }

                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(basePath))
                    {
                        try
                        {
                            var settings = JsonConvert.DeserializeObject<LauncherSettings>(text);
                            var exePath = PathOperations.ResolveRelativePath(basePath, settings.ExePath).StandardizeDirectorySeparator();
                            if (File.Exists(exePath))
                            {
                                return new GameSettings { ExecutableArgs = string.Join(" ", settings.ExeArgs), ExecutablePath = exePath, UserDir = pathResolver.Parse(settings.GameDataPath) };
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var file in game.SignatureFiles)
                {
                    var path = Path.Combine(basePath, file);
                    if (File.Exists(path))
                    {
                        var exePath = PathOperations.ResolveRelativePath(basePath, game.DefaultGameBinaryPath);
                        if (File.Exists(exePath))
                        {
                            return new GameSettings { ExecutableArgs = string.Empty, ExecutablePath = exePath, UserDir = string.Empty };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the hoi4 registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetHOI4(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.HeartsOfIron4.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.HeartsOfIron4.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.HeartsOfIron4.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.HeartsOfIron4.Id;
            game.Abrv = Shared.Constants.GamesTypes.HeartsOfIron4.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.HeartsOfIron4.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.HeartsOfIron4.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.HeartsOfIron4.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.HeartsOfIron4.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.ReadOnly;
            game.ParadoxGameId = Shared.Constants.GamesTypes.HeartsOfIron4.ParadoxGameId;
            game.SupportedMergeTypes = SupportedMergeTypes.Zip | SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.DescriptorMod;
            game.GameIndexCacheVersion = HOI4CacheVersion;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the imperator registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetImperator(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.ImperatorRome.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.ImperatorRome.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.ImperatorRome.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.ImperatorRome.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.ImperatorRome.Id;
            game.Abrv = Shared.Constants.GamesTypes.ImperatorRome.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.ImperatorRome.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.ImperatorRome.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.ImperatorRome.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.ImperatorRome.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.ImperatorRome.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = Shared.Constants.GamesTypes.ImperatorRome.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.None;
            game.ParadoxGameId = Shared.Constants.GamesTypes.ImperatorRome.ParadoxGameId;
            game.GogAppId = Shared.Constants.GamesTypes.ImperatorRome.GogId;
            game.SupportedMergeTypes = SupportedMergeTypes.Zip | SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.DescriptorMod;
            game.GameIndexCacheVersion = 1;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the stellaris registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetStellaris(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.Stellaris.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.Stellaris.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Stellaris.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.Stellaris.Id;
            game.Abrv = Shared.Constants.GamesTypes.Stellaris.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.Stellaris.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Stellaris.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.Full;
            game.ParadoxGameId = Shared.Constants.GamesTypes.Stellaris.ParadoxGameId;
            game.GogAppId = Shared.Constants.GamesTypes.Stellaris.GogId;
            game.SupportedMergeTypes = SupportedMergeTypes.Zip | SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.DescriptorMod;
            game.GameIndexCacheVersion = StellarisCacheVersion;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the Star Trek infinite registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        // ReSharper disable once InconsistentNaming
        private IGameType GetSTInfinite(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.STInfinite.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.STInfinite.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.STInfinite.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.STInfinite.Id;
            game.Abrv = Shared.Constants.GamesTypes.STInfinite.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.STInfinite.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.STInfinite.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.STInfinite.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.STInfinite.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.LauncherSettingsFileName;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.None;
            game.ParadoxGameId = Shared.Constants.GamesTypes.STInfinite.ParadoxGameId;
            game.SupportedMergeTypes = SupportedMergeTypes.Zip | SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.DescriptorMod;
            game.GameIndexCacheVersion = 1;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
            return game;
        }

        /// <summary>
        /// Gets the vicky3 registration.
        /// </summary>
        /// <param name="baseUserDir">The base user dir.</param>
        /// <returns>IGameType.</returns>
        private IGameType GetVicky3(string baseUserDir)
        {
            var game = DIResolver.Get<IGameType>();
            game.DLCContainer = Shared.Constants.GamesTypes.Victoria3.DLCContainer;
            game.ChecksumFolders = Shared.Constants.GamesTypes.Victoria3.ChecksumFolders;
            game.GameFolders = Shared.Constants.GamesTypes.Victoria3.GameFolders;
            game.LogLocation = Path.Combine(Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Victoria3.DocsPath), Shared.Constants.GamesTypes.LogLocation).StandardizeDirectorySeparator();
            game.Name = Shared.Constants.GamesTypes.Victoria3.Id;
            game.Abrv = Shared.Constants.GamesTypes.Victoria3.Abrv;
            game.SteamAppId = Shared.Constants.GamesTypes.Victoria3.SteamAppId;
            game.UserDirectory = Path.Combine(baseUserDir, Shared.Constants.GamesTypes.Victoria3.DocsPath).StandardizeDirectorySeparator();
            game.WorkshopDirectory = [.. SteamDirectory.GetWorkshopDirectory(Shared.Constants.GamesTypes.Victoria3.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.BaseSteamGameDirectory = SteamDirectory.GetGameDirectory(Shared.Constants.GamesTypes.Victoria3.SteamAppId).StandardizeDirectorySeparator();
            game.LauncherSettingsFileName = Shared.Constants.GamesTypes.Victoria3.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = Shared.Constants.GamesTypes.Victoria3.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = [.. SteamDirectory.GetUserDataFolders(game.SteamAppId).Select(p => p.StandardizeDirectorySeparator())];
            game.AdvancedFeatures = GameAdvancedFeatures.None;
            game.ParadoxGameId = Shared.Constants.GamesTypes.Victoria3.ParadoxGameId;
            game.SupportedMergeTypes = SupportedMergeTypes.Basic;
            game.ModDescriptorType = ModDescriptorType.JsonMetadata;
            game.GameIndexCacheVersion = 1;
            game.SignatureFiles = Shared.Constants.GamesTypes.SignatureFiles;
            game.DefaultGameBinaryPath = Shared.Constants.GamesTypes.DefaultGameBinaryPath;
            game.SupportedOperatingSystems = SupportedOperatingSystems.Linux | SupportedOperatingSystems.OSX | SupportedOperatingSystems.Windows;
            MapGameSettings(game, GetExecutableSettings(game));
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
            public string ExecutableArgs { get; init; }

            /// <summary>
            /// Gets or sets the executable path.
            /// </summary>
            /// <value>The executable path.</value>
            public string ExecutablePath { get; init; }

            /// <summary>
            /// Gets or sets the user dir.
            /// </summary>
            /// <value>The user dir.</value>
            public string UserDir { get; init; }

            #endregion Properties
        }

        #endregion Classes
    }
}
