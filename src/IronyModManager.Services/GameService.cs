// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 11-18-2022
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
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Services.Resolver;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;
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
        /// The game path resolver
        /// </summary>
        private readonly GameRootPathResolver gamePathResolver;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

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

        /// <summary>
        /// The report export service
        /// </summary>
        private readonly IReportExportService reportExportService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="reportExportService">The report export service.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="mapper">The mapper.</param>
        public GameService(IMessageBus messageBus, IReportExportService reportExportService, IReader reader, IStorageProvider storageProvider,
            IPreferencesService preferencesService, IMapper mapper) : base(storageProvider, mapper)
        {
            this.messageBus = messageBus;
            this.reportExportService = reportExportService;
            this.preferencesService = preferencesService;
            this.reader = reader;
            pathResolver = new PathResolver();
            gamePathResolver = new GameRootPathResolver();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// export hash report as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ExportHashReportAsync(IGame game, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(game.ExecutableLocation))
            {
                var basePath = Path.GetDirectoryName(Path.Combine(gamePathResolver.GetPath(game), gamePathResolver.ResolveDLCDirectory(game.DLCContainer, GameRootPathResolver.DLCFolder)));
                if (!string.IsNullOrWhiteSpace(basePath))
                {
                    var files = reader.GetFiles(basePath);
                    if (files != null && files.Any())
                    {
                        var reports = await ParseReportAsync(game, basePath, files);
                        return await reportExportService.ExportAsync(reports, path);
                    }
                }
            }
            return false;
        }

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
            else if (!string.IsNullOrWhiteSpace(game.BaseSteamGameDirectory))
            {
                model.ExecutableLocation = $"{SteamLaunchArgs}{game.SteamAppId}";
            }
            else if (!string.IsNullOrWhiteSpace(game.ExecutableLocation) && !string.IsNullOrWhiteSpace(game.LauncherSettingsFileName))
            {
                var basePath = Path.GetDirectoryName(game.ExecutableLocation);
                var jsonData = GetGameSettingsFromJson(game, basePath);
                if (jsonData != null)
                {
                    model.LaunchArguments = jsonData.LaunchArguments;
                    model.UserDirectory = jsonData.UserDirectory;
                }
            }
            if (string.IsNullOrWhiteSpace(model.UserDirectory))
            {
                model.UserDirectory = game.UserDirectory;
            }
            model.CustomModDirectory = string.Empty;
            return model;
        }

        /// <summary>
        /// Gets the game settings from json.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>IGameSettings.</returns>
        public virtual IGameSettings GetGameSettingsFromJson(IGame game, string path)
        {
            var settingsObject = GetGameLauncherSettings(game, path);
            if (settingsObject != null)
            {
                path = settingsObject.BasePath;
                var model = GetModelInstance<IGameSettings>();
                model.LaunchArguments = string.Join(" ", settingsObject.ExeArgs);
                model.UserDirectory = pathResolver.Parse(settingsObject.GameDataPath);
                model.ExecutableLocation = PathOperations.ResolveRelativePath(path, settingsObject.ExePath).StandardizeDirectorySeparator();
                model.CustomModDirectory = string.Empty;
                return model;
            }
            return null;
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
            model.CloseAppAfterGameLaunch = game.CloseAppAfterGameLaunch;
            model.Type = game.Type;
            model.CustomModDirectory = string.Empty;
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
        /// Gets the version.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        public virtual string GetVersion(IGame game)
        {
            if (game != null)
            {
                var path = Path.GetDirectoryName(game.ExecutableLocation);
                var settingsObject = GetGameLauncherSettings(game, path);
                // Some games use version field (inconsistency again)
                if (settingsObject != null)
                {
                    if (!string.IsNullOrWhiteSpace(settingsObject.RawVersion) && Shared.Version.TryParse(settingsObject.RawVersion, out _))
                    {
                        return settingsObject.RawVersion;
                    }
                    if (!string.IsNullOrWhiteSpace(settingsObject.Version) && Shared.Version.TryParse(settingsObject.Version, out _))
                    {
                        return settingsObject.Version;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        public virtual IEnumerable<string> GetVersions(IGame game)
        {
            if (game != null)
            {
                var path = Path.GetDirectoryName(game.ExecutableLocation);
                var settingsObject = GetGameLauncherSettings(game, path);
                if (settingsObject != null)
                {
                    var result = new List<string>();
                    if (!string.IsNullOrWhiteSpace(settingsObject.RawVersion))
                    {
                        result.Add(settingsObject.RawVersion);
                    }
                    if (!string.IsNullOrWhiteSpace(settingsObject.Version))
                    {
                        result.Add(settingsObject.Version);
                    }
                    return result;
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// import hash report as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>Task&lt;IEnumerable&lt;IHashReport&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<IHashReport>> ImportHashReportAsync(IGame game, IReadOnlyCollection<IHashReport> hashReports)
        {
            var importedReports = reportExportService.GetGameReports(hashReports);
            if (importedReports == null || !importedReports.Any())
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(game.ExecutableLocation))
            {
                var basePath = Path.GetDirectoryName(Path.Combine(gamePathResolver.GetPath(game), gamePathResolver.ResolveDLCDirectory(game.DLCContainer, GameRootPathResolver.DLCFolder)));
                if (!string.IsNullOrWhiteSpace(basePath))
                {
                    var files = reader.GetFiles(basePath);
                    var currentReports = await ParseReportAsync(game, basePath, files);
                    return reportExportService.CompareReports(currentReports.ToList(), importedReports.ToList());
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether [is continue game allowed] [the specified game].
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if [is continue game allowed] [the specified game]; otherwise, <c>false</c>.</returns>
        public virtual bool IsContinueGameAllowed(IGame game)
        {
            const string saveGames = "save games";
            try
            {
                var continueGameFile = Path.Combine(game.UserDirectory, ContinueGameFileName);
                var parsed = reader.Read(continueGameFile);
                if (parsed?.Count() == 1)
                {
                    var data = JsonConvert.DeserializeObject<Models.ContinueGame>(string.Join(Environment.NewLine, parsed.FirstOrDefault().Content));
                    var path = string.IsNullOrWhiteSpace(data.Filename) ? data.Title : data.Filename;
                    var fileWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    var fileWithExtension = Path.GetFileName(path);
                    var saveDir = Path.GetDirectoryName(path).Replace(saveGames, string.Empty).StandardizeDirectorySeparator();
                    var files = new List<string>();
                    if (!string.IsNullOrWhiteSpace(saveDir))
                    {
                        var saves = new List<string>() { Path.Combine(game.UserDirectory, saveGames, saveDir.Trim(Path.DirectorySeparatorChar)) };
                        foreach (var item in game.RemoteSteamUserDirectory)
                        {
                            saves.Add(Path.Combine(item, saveGames, saveDir.Trim(Path.DirectorySeparatorChar)));
                        }
                        foreach (var item in saves)
                        {
                            if (Directory.Exists(item))
                            {
                                files.AddRange(Directory.GetFiles(item, "*"));
                            }
                        }
                    }
                    else
                    {
                        var saves = new List<string>() { Path.Combine(game.UserDirectory, saveGames) };
                        foreach (var item in game.RemoteSteamUserDirectory)
                        {
                            saves.Add(Path.Combine(item, saveGames));
                        }
                        foreach (var item in saves)
                        {
                            if (Directory.Exists(item))
                            {
                                files.AddRange(Directory.GetFiles(item, "*"));
                            }
                        }
                    }
                    return files.Any(p => Path.GetFileNameWithoutExtension(p).Equals(fileWithoutExtension, StringComparison.OrdinalIgnoreCase) || Path.GetFileNameWithoutExtension(p).Equals(fileWithExtension, StringComparison.OrdinalIgnoreCase));
                }
            }
            catch
            {
                return false;
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
            var game = StorageProvider.GetGames().FirstOrDefault(p => p.Name.Equals(settings.Type));
            var baseGameDirectory = (game.BaseSteamGameDirectory ?? string.Empty).StandardizeDirectorySeparator();
            var settingsExe = (settings.ExecutableLocation ?? string.Empty).StandardizeDirectorySeparator();
            return !string.IsNullOrWhiteSpace(baseGameDirectory) && !string.IsNullOrWhiteSpace(settingsExe) && settingsExe.StartsWith(baseGameDirectory, StringComparison.OrdinalIgnoreCase);
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
        /// <exception cref="System.InvalidOperationException">Game not selected</exception>
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
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SetSelected(IEnumerable<IGame> games, IGame selectedGame)
        {
            if (games == null || !games.Any() || selectedGame == null)
            {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException($"{nameof(games)} or {nameof(selectedGame)}.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }

            var currentSelection = GetSelected();
            if (currentSelection?.Type == selectedGame.Type)
            {
                return false;
            }

            foreach (var item in games)
            {
                item.IsSelected = item.Type == selectedGame.Type;
            }
            selectedGame.IsSelected = true;
            var storedSelectedGame = Get().FirstOrDefault(p => p.Type.Equals(selectedGame.Type));
            storedSelectedGame.IsSelected = true;
            return Save(storedSelectedGame);
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Double.</returns>
        protected virtual double GetProgressPercentage(double total, double processed, double maxPerc = 100)
        {
            var perc = Math.Round(processed / total * 100, 2);
            if (perc < 0)
            {
                perc = 0;
            }
            else if (perc > maxPerc)
            {
                perc = maxPerc;
            }
            return perc;
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
            game.DLCContainer = gameType.DLCContainer;
            game.GameFolders = gameType.GameFolders ?? new List<string>();
            game.BaseSteamGameDirectory = gameType.BaseSteamGameDirectory;
            game.AdvancedFeatures = gameType.AdvancedFeatures;
            game.SupportedMergeTypes = gameType.SupportedMergeTypes;
            game.ModDescriptorType = gameType.ModDescriptorType;
            game.GameIndexCacheVersion = gameType.GameIndexCacheVersion;
            game.ParadoxGameId = gameType.ParadoxGameId;
            game.LauncherSettingsFileName = gameType.LauncherSettingsFileName;
            game.LauncherSettingsPrefix = gameType.LauncherSettingsPrefix;
            game.RemoteSteamUserDirectory = gameType.RemoteSteamUserDirectory;
            game.Abrv = gameType.Abrv;
            game.CloseAppAfterGameLaunch = true;
            game.CustomModDirectory = string.Empty;
            game.GogAppId = gameType.GogAppId;
            var setExeLocation = true;
            var setUserDirLocation = true;
            if (gameSettings != null)
            {
                game.CloseAppAfterGameLaunch = gameSettings.CloseAppAfterGameLaunch.GetValueOrDefault(true);
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
                if (!string.IsNullOrWhiteSpace(gameSettings.CustomModDirectory))
                {
                    game.CustomModDirectory = gameSettings.CustomModDirectory;
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
        /// parse report as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="basePath">The base path.</param>
        /// <param name="files">The files.</param>
        /// <returns>IEnumerable&lt;IHashReport&gt;.</returns>
        protected virtual async Task<IEnumerable<IHashReport>> ParseReportAsync(IGame game, string basePath, IEnumerable<string> files)
        {
            var reports = new List<IHashReport>();

            var total = files.Where(p => game.GameFolders.Any(x => p.StartsWith(x))).Count();
            var progress = 0;
            double lastPercentage = 0;

            foreach (var item in game.GameFolders)
            {
                var report = GetModelInstance<IHashReport>();
                report.Name = item;
                report.ReportType = HashReportType.Game;
                var hashReports = new List<IHashFileReport>();
                foreach (var file in files.Where(p => p.StartsWith(item)))
                {
                    var info = reader.GetFileInfo(basePath, file);
                    if (info != null)
                    {
                        var fileReport = GetModelInstance<IHashFileReport>();
                        fileReport.File = file;
                        fileReport.Hash = info.ContentSHA;
                        hashReports.Add(fileReport);
                    }
                    progress++;
                    var percentage = GetProgressPercentage(total, progress);
                    if (percentage != lastPercentage)
                    {
                        await messageBus.PublishAsync(new ModReportExportEvent(1, percentage));
                    }
                    lastPercentage = percentage;
                }
                report.Reports = hashReports;
                reports.Add(report);
            }
            return reports;
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
            settings.CloseAppAfterGameLaunch = game.CloseAppAfterGameLaunch;
            settings.RefreshDescriptors = game.RefreshDescriptors;
            settings.UserDirectory = game.UserDirectory;
            settings.CustomModDirectory = game.CustomModDirectory;
            StorageProvider.SetGameSettings(gameSettings);
        }

        /// <summary>
        /// Gets the game launcher settings.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Models.LauncherSettings.</returns>
        private Models.LauncherSettings GetGameLauncherSettings(IGame game, string path)
        {
            string settingsFile;
            if (string.IsNullOrWhiteSpace(game.LauncherSettingsPrefix))
            {
                settingsFile = game.LauncherSettingsFileName;
            }
            else
            {
                settingsFile = game.LauncherSettingsPrefix + game.LauncherSettingsFileName;
            }
            var infoPath = PathOperations.ResolveRelativePath(path, settingsFile);
            var info = reader.Read(infoPath);
            if (info == null)
            {
                // Try to traverse down
                var gamePath = Path.GetDirectoryName(path);
                while (!string.IsNullOrWhiteSpace(gamePath))
                {
                    infoPath = PathOperations.ResolveRelativePath(gamePath, settingsFile);
                    info = reader.Read(infoPath);
                    if (info != null)
                    {
                        break;
                    }
                    gamePath = Path.GetDirectoryName(gamePath);
                }
                path = gamePath;
            }
            if (info != null && info.Any())
            {
                var text = string.Join(Environment.NewLine, info.FirstOrDefault().Content);
                try
                {
                    var model = GetModelInstance<IGameSettings>();
                    var settingsObject = JsonConvert.DeserializeObject<Models.LauncherSettings>(text);
                    settingsObject.BasePath = path;
                    return settingsObject;
                }
                catch
                {
                }
            }
            return null;
        }

        #endregion Methods
    }
}
