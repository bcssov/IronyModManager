// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 09-22-2020
// ***********************************************************************
// <copyright file="GameService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Services.Resolver;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using Newtonsoft.Json;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class GameService.
    /// Implements the <see cref="IronyModManager.Services.Common.IGameService" />
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IGameService" />
    public class GameService : BaseService, IGameService
    {
        #region Fields

        /// <summary>
        /// The continue game arguments
        /// </summary>
        private const string ContinueGameArgs = "--continuelastsave";

        /// <summary>
        /// The continue game file name
        /// </summary>
        private const string ContinueGameFileName = "continue_game.json";

        /// <summary>
        /// The steam launch arguments
        /// </summary>
        private const string SteamLaunchArgs = "steam://run/";

        /// <summary>
        /// The path resolver
        /// </summary>
        private readonly PathResolver pathResolver;

        /// <summary>
        /// The preferences service
        /// </summary>
        private readonly IPreferencesService preferencesService;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="mapper">The mapper.</param>
        public GameService(IReader reader, IStorageProvider storageProvider, IPreferencesService preferencesService, IMapper mapper) : base(storageProvider, mapper)
        {
            this.preferencesService = preferencesService;
            this.reader = reader;
            pathResolver = new PathResolver();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;IGame&gt;.</returns>
        public virtual IEnumerable<IGame> Get()
        {
            var prefs = preferencesService.Get();

            var registeredGames = StorageProvider.GetGames();
            var gameSettings = StorageProvider.GetGameSettings();
            var games = new List<IGame>();

            foreach (var item in registeredGames)
            {
                var game = InitModel(item, gameSettings.FirstOrDefault(p => p.Type.Equals(item.Name)), prefs.Game);
                games.Add(game);
            }

            return games.OrderBy(p => p.Type);
        }

        /// <summary>
        /// Gets the default game settings.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IGameSettings.</returns>
        public virtual IGameSettings GetDefaultGameSettings(IGame game)
        {
            var model = GetModelInstance<IGameSettings>();
            var settings = StorageProvider.GetGames().FirstOrDefault(p => p.Name.Equals(game.Type));
            if (!string.IsNullOrWhiteSpace(settings.ExecutablePath))
            {
                model.ExecutableLocation = settings.ExecutablePath;
                model.LaunchArguments = settings.ExecutableArgs;
                model.UserDirectory = settings.UserDirectory;
            }
            else if (!string.IsNullOrWhiteSpace(game.WorkshopDirectory))
            {
                model.ExecutableLocation = $"{SteamLaunchArgs}{game.SteamAppId}";
            }
            else if (!string.IsNullOrWhiteSpace(game.ExecutableLocation) && !string.IsNullOrWhiteSpace(game.LauncherSettingsFileName))
            {
                var basePath = Path.GetDirectoryName(game.ExecutableLocation);
                string settingsFile;
                if (string.IsNullOrWhiteSpace(game.LauncherSettingsPrefix))
                {
                    settingsFile = game.LauncherSettingsFileName;
                }
                else
                {
                    settingsFile = game.LauncherSettingsPrefix + game.LauncherSettingsFileName;
                }
                var info = reader.Read(Path.Combine(basePath, settingsFile));
                if (info?.Count() > 0)
                {
                    var text = string.Join(Environment.NewLine, info.FirstOrDefault().Content);
                    try
                    {
                        var settingsObject = JsonConvert.DeserializeObject<Models.LauncherSettings>(text);
                        model.LaunchArguments = string.Join(" ", settingsObject.ExeArgs);
                        model.UserDirectory = pathResolver.Parse(settingsObject.GameDataPath);
                    }
                    catch
                    {
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(model.UserDirectory))
            {
                model.UserDirectory = game.UserDirectory;
            }
            return model;
        }

        /// <summary>
        /// Gets the launch arguments.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="appendContinueGame">if set to <c>true</c> [append continue game].</param>
        /// <returns>System.String.</returns>
        public virtual IGameSettings GetLaunchSettings(IGame game, bool appendContinueGame = false)
        {
            var model = GetModelInstance<IGameSettings>();
            var exeLoc = game.ExecutableLocation ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(exeLoc))
            {
                if (exeLoc.StartsWith(SteamLaunchArgs, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(game.LaunchArguments))
                    {
                        model.ExecutableLocation = $"{exeLoc.Trim('/')}//{game.LaunchArguments}";
                    }
                    else
                    {
                        model.ExecutableLocation = $"{exeLoc.Trim('/')}";
                    }
                }
                else
                {
                    model.ExecutableLocation = exeLoc;
                    if (!string.IsNullOrWhiteSpace(game.LaunchArguments))
                    {
                        model.LaunchArguments = game.LaunchArguments;
                    }
                }
            }
            if (appendContinueGame)
            {
                model.LaunchArguments = $"{ContinueGameArgs} {model.LaunchArguments.Trim()}";
            }
            model.UserDirectory = game.UserDirectory;
            model.RefreshDescriptors = game.RefreshDescriptors;
            return model;
        }

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>IGame.</returns>
        public virtual IGame GetSelected()
        {
            return Get().FirstOrDefault(s => s.IsSelected);
        }

        /// <summary>
        /// Determines whether [is continue game allowed] [the specified game].
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if [is continue game allowed] [the specified game]; otherwise, <c>false</c>.</returns>
        public virtual bool IsContinueGameAllowed(IGame game)
        {
            var continueGameFile = Path.Combine(game.UserDirectory, ContinueGameFileName);
            var parsed = reader.Read(continueGameFile);
            if (parsed?.Count() == 1)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<Models.ContinueGame>(string.Join(Environment.NewLine, parsed.FirstOrDefault().Content));
                    var fileName = string.IsNullOrWhiteSpace(data.Filename) ? data.Title : data.Filename;
                    var path = Path.GetFileNameWithoutExtension(fileName);
                    var saveDir = Path.GetDirectoryName(fileName).Replace("save games", string.Empty).StandardizeDirectorySeparator();
                    string[] files;
                    if (!string.IsNullOrWhiteSpace(saveDir))
                    {
                        files = Directory.GetFiles(Path.Combine(game.UserDirectory, "save games", saveDir.Trim(Path.DirectorySeparatorChar)), "*");
                    }
                    else
                    {
                        files = Directory.GetFiles(Path.Combine(game.UserDirectory, "save games"), "*");
                    }
                    return files.Any(p => Path.GetFileNameWithoutExtension(p).Contains(path, StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is steam game] [the specified settings].
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if [is steam game] [the specified settings]; otherwise, <c>false</c>.</returns>
        public virtual bool IsSteamGame(IGameSettings settings)
        {
            var game = GetSelected();
            var gameExe = (game.ExecutableLocation ?? string.Empty).StandardizeDirectorySeparator();
            var settingsExe = (settings.ExecutableLocation ?? string.Empty).StandardizeDirectorySeparator();
            return gameExe.Equals(settingsExe);
        }

        /// <summary>
        /// Determines whether [is steam launch path] [the specified settings].
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if [is steam launch path] [the specified settings]; otherwise, <c>false</c>.</returns>
        public virtual bool IsSteamLaunchPath(IGameSettings settings)
        {
            return settings.ExecutableLocation.StartsWith(SteamLaunchArgs);
        }

        /// <summary>
        /// Saves the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Game not selected</exception>
        public virtual bool Save(IGame game)
        {
            if (!game.IsSelected)
            {
                throw new InvalidOperationException("Game not selected");
            }
            var prefs = preferencesService.Get();

            SaveGameSettings(game);

            return preferencesService.Save(Mapper.Map(game, prefs));
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="games">The games.</param>
        /// <param name="selectedGame">The selected game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">games or selectedGame.</exception>
        public virtual bool SetSelected(IEnumerable<IGame> games, IGame selectedGame)
        {
            if (games == null || games.Count() == 0 || selectedGame == null)
            {
                throw new ArgumentNullException("games or selectedGame.");
            }

            var currentSelection = GetSelected();
            if (currentSelection?.Type == selectedGame.Type)
            {
                return false;
            }

            var type = currentSelection != null ? currentSelection.Type : selectedGame.Type;
            foreach (var item in games)
            {
                if (item.Type != type)
                {
                    item.IsSelected = false;
                }
            }
            selectedGame.IsSelected = true;
            var storedSelectedGame = Get().FirstOrDefault(p => p.Type.Equals(selectedGame.Type));
            storedSelectedGame.IsSelected = true;
            return Save(storedSelectedGame);
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <param name="gameSettings">The game settings.</param>
        /// <param name="selectedGame">The selected game.</param>
        /// <returns>IGame.</returns>
        protected virtual IGame InitModel(IGameType gameType, IGameSettings gameSettings, string selectedGame)
        {
            var game = GetModelInstance<IGame>();
            game.Type = gameType.Name;
            game.Name = gameType.Name;
            game.IsSelected = selectedGame == gameType.Name;
            game.UserDirectory = gameType.UserDirectory;
            game.SteamAppId = gameType.SteamAppId;
            game.WorkshopDirectory = gameType.WorkshopDirectory;
            game.LogLocation = gameType.LogLocation;
            game.ChecksumFolders = gameType.ChecksumFolders;
            game.GameFolders = gameType.GameFolders ?? new List<string>();
            game.BaseGameDirectory = gameType.BaseGameDirectory;
            game.AdvancedFeaturesSupported = gameType.AdvancedFeaturesSupported;
            game.LauncherSettingsFileName = gameType.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = gameType.LauncherSettingsPrefix;
            var setExeLocation = true;
            var setUserDirLocation = true;
            if (gameSettings != null)
            {
                game.RefreshDescriptors = gameSettings.RefreshDescriptors;
                if (!string.IsNullOrWhiteSpace(gameSettings.LaunchArguments))
                {
                    game.LaunchArguments = gameSettings.LaunchArguments;
                }
                if (!string.IsNullOrWhiteSpace(gameSettings.ExecutableLocation))
                {
                    setExeLocation = false;
                    game.ExecutableLocation = gameSettings.ExecutableLocation;
                }
                if (!string.IsNullOrWhiteSpace(gameSettings.UserDirectory))
                {
                    setUserDirLocation = false;
                    game.UserDirectory = gameSettings.UserDirectory;
                }
            }
            if (setExeLocation || setUserDirLocation)
            {
                var settings = GetDefaultGameSettings(game);
                if (setExeLocation)
                {
                    game.ExecutableLocation = settings.ExecutableLocation ?? string.Empty;
                    game.LaunchArguments = settings.LaunchArguments ?? string.Empty;
                }
                if (setUserDirLocation)
                {
                    game.UserDirectory = settings.UserDirectory;
                }
            }
            if (string.IsNullOrWhiteSpace(game.UserDirectory))
            {
                game.UserDirectory = gameType.UserDirectory;
            }
            return game;
        }

        /// <summary>
        /// Saves the game settings.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void SaveGameSettings(IGame game)
        {
            var gameSettings = StorageProvider.GetGameSettings().ToList();
            var settings = gameSettings.FirstOrDefault(p => p.Type.Equals(game.Type));
            if (settings == null)
            {
                settings = GetModelInstance<IGameSettings>();
                settings.Type = game.Type;
                gameSettings.Add(settings);
            }
            settings.ExecutableLocation = game.ExecutableLocation;
            settings.LaunchArguments = game.LaunchArguments;
            settings.RefreshDescriptors = game.RefreshDescriptors;
            settings.UserDirectory = game.UserDirectory;
            StorageProvider.SetGameSettings(gameSettings);
        }

        #endregion Methods
    }
}
