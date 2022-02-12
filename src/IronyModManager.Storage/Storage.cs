// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-11-2022
// ***********************************************************************
// <copyright file="Storage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;
using KellermanSoftware.CompareNetObjects;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Storage.
    /// Implements the <see cref="IronyModManager.Storage.Common.IStorageProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.Common.IStorageProvider" />
    public class Storage : IStorageProvider
    {
        #region Fields

        /// <summary>
        /// The database lock
        /// </summary>
        private static readonly object dbLock = new { };

        /// <summary>
        /// The compare logic
        /// </summary>
        private static CompareLogic compareLogic;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="mapper">The mapper.</param>
        public Storage(IDatabase database, IMapper mapper)
        {
            Database = database;
            Mapper = mapper;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        protected IDatabase Database { get; private set; }

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        protected IMapper Mapper { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <returns>IAppState.</returns>
        public virtual IAppState GetAppState()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<IAppState, IAppState>(Database.AppState);
                return result;
            }
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <returns>IEnumerable&lt;IGameType&gt;.</returns>
        public virtual IEnumerable<IGameType> GetGames()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<List<IGameType>>(Database.Games);
                return result;
            }
        }

        /// <summary>
        /// Gets the game settings.
        /// </summary>
        /// <returns>IEnumerable&lt;IGameSettings&gt;.</returns>
        public virtual IEnumerable<IGameSettings> GetGameSettings()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<List<IGameSettings>>(Database.GameSettings);
                return result;
            }
        }

        /// <summary>
        /// Gets the mod collections.
        /// </summary>
        /// <returns>IEnumerable&lt;IModCollection&gt;.</returns>
        public virtual IEnumerable<IModCollection> GetModCollections()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<List<IModCollection>>(Database.ModCollection);
                return result;
            }
        }

        /// <summary>
        /// Gets the notification positions.
        /// </summary>
        /// <returns>IEnumerable&lt;INotificationPositionType&gt;.</returns>
        public virtual IEnumerable<INotificationPositionType> GetNotificationPositions()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<List<INotificationPositionType>>(Database.NotificationPosition);
                return result;
            }
        }

        /// <summary>
        /// Gets the preferences.
        /// </summary>
        /// <returns>IPreferences.</returns>
        public virtual IPreferences GetPreferences()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<IPreferences, IPreferences>(Database.Preferences);
                return result;
            }
        }

        /// <summary>
        /// Gets the root storage path.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetRootStoragePath()
        {
            return JsonStore.RootPaths.FirstOrDefault();
        }

        /// <summary>
        /// Gets the themes.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, IEnumerable&lt;System.String&gt;&gt;.</returns>
        public virtual IEnumerable<IThemeType> GetThemes()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<List<IThemeType>>(Database.Themes);
                return result;
            }
        }

        /// <summary>
        /// Gets the state of the window.
        /// </summary>
        /// <returns>IWindowState.</returns>
        public virtual IWindowState GetWindowState()
        {
            lock (dbLock)
            {
                var result = Mapper.Map<IWindowState, IWindowState>(Database.WindowState);
                return result;
            }
        }

        /// <summary>
        /// Registers the game.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public virtual bool RegisterGame(IGameType gameType)
        {
            lock (dbLock)
            {
                if (gameType == null || Database.Games.Any(s => s.Name.Equals(gameType.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"{gameType?.Name} game is already registered or invalid registration.");
                }
                var game = DIResolver.Get<IGameType>();
                game.Name = gameType.Name;
                game.UserDirectory = gameType.UserDirectory ?? string.Empty;
                game.SteamAppId = gameType.SteamAppId;
                game.WorkshopDirectory = gameType.WorkshopDirectory ?? new List<string>();
                game.LogLocation = gameType.LogLocation;
                game.ChecksumFolders = gameType.ChecksumFolders ?? new List<string>();
                game.GameFolders = gameType.GameFolders ?? new List<string>();
                game.BaseSteamGameDirectory = gameType.BaseSteamGameDirectory ?? string.Empty;
                game.ExecutablePath = gameType.ExecutablePath ?? string.Empty;
                game.ExecutableArgs = gameType.ExecutableArgs ?? string.Empty;
                game.LauncherSettingsFileName = gameType.LauncherSettingsFileName ?? string.Empty;
                game.LauncherSettingsPrefix = gameType.LauncherSettingsPrefix ?? string.Empty;
                game.AdvancedFeatures = gameType.AdvancedFeatures;
                game.GameIndexCacheVersion = gameType.GameIndexCacheVersion;
                game.ParadoxGameId = gameType.ParadoxGameId;
                game.RemoteSteamUserDirectory = gameType.RemoteSteamUserDirectory ?? new List<string>();
                game.Abrv = gameType.Abrv ?? string.Empty;
                game.DLCContainer = gameType.DLCContainer ?? string.Empty;
                Database.Games.Add(game);
                return true;
            }
        }

        /// <summary>
        /// Registers the notification position.
        /// </summary>
        /// <param name="notificationPosition">The notification position.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.InvalidOperationException">Notification position is null or already registered.</exception>
        public virtual bool RegisterNotificationPosition(INotificationPositionType notificationPosition)
        {
            lock (dbLock)
            {
                if (notificationPosition == null || Database.NotificationPosition.Any(p => p.Position == notificationPosition.Position) || (notificationPosition.IsDefault && Database.NotificationPosition.Any(p => p.IsDefault)))
                {
                    throw new InvalidOperationException("Notification position is null or already registered.");
                }
                var model = DIResolver.Get<INotificationPositionType>();
                model.IsDefault = notificationPosition.IsDefault;
                model.Position = notificationPosition.Position;
                Database.NotificationPosition.Add(model);
                return true;
            }
        }

        /// <summary>
        /// Registers the theme.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.InvalidOperationException">There is already a default theme registered.</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public virtual bool RegisterTheme(string name, bool isDefault = false)
        {
            lock (dbLock)
            {
                if (isDefault && Database.Themes.Any(s => s.IsDefault))
                {
                    throw new InvalidOperationException("There is already a default theme registered.");
                }
                if (Database.Themes.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"{name} theme is already registered.");
                }
                var themeType = DIResolver.Get<IThemeType>();
                themeType.IsDefault = isDefault;
                themeType.Name = name;
                Database.Themes.Add(themeType);
                return true;
            }
        }

        /// <summary>
        /// Sets the state of the application.
        /// </summary>
        /// <param name="appState">State of the application.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetAppState(IAppState appState)
        {
            lock (dbLock)
            {
                var data = Mapper.Map<IAppState>(appState);
                if (IsDifferent(data, Database.AppState))
                {
                    Database.AppState = data;
                }
                return true;
            }
        }

        /// <summary>
        /// Sets the game settings.
        /// </summary>
        /// <param name="gameSettings">The game settings.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetGameSettings(IEnumerable<IGameSettings> gameSettings)
        {
            lock (dbLock)
            {
                var data = Mapper.Map<List<IGameSettings>>(gameSettings);
                if (IsDifferent(data, Database.GameSettings))
                {
                    Database.GameSettings = data;
                }
                return true;
            }
        }

        /// <summary>
        /// Sets the mod collections.
        /// </summary>
        /// <param name="modCollections">The mod collections.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetModCollections(IEnumerable<IModCollection> modCollections)
        {
            lock (dbLock)
            {
                var data = Mapper.Map<List<IModCollection>>(modCollections);
                if (IsDifferent(data, Database.ModCollection))
                {
                    Database.ModCollection = data;
                }
                return true;
            }
        }

        /// <summary>
        /// Sets the preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetPreferences(IPreferences preferences)
        {
            lock (dbLock)
            {
                var data = Mapper.Map<IPreferences>(preferences);
                if (IsDifferent(data, Database.Preferences))
                {
                    Database.Preferences = data;
                }
                return true;
            }
        }

        /// <summary>
        /// Sets the state of the window.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool SetWindowState(IWindowState state)
        {
            lock (dbLock)
            {
                var data = Mapper.Map<IWindowState>(state);
                if (IsDifferent(data, Database.WindowState))
                {
                    Database.WindowState = data;
                }
                return true;
            }
        }

        /// <summary>
        /// Determines whether the specified source is different.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if the specified source is different; otherwise, <c>false</c>.</returns>
        protected virtual bool IsDifferent<T>(T source, T destination)
        {
            var compareLogic = GetComparer();
            var result = compareLogic.Compare(source, destination);
            return !result.AreEqual;
        }

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        /// <returns>CompareLogic.</returns>
        private static CompareLogic GetComparer()
        {
            if (compareLogic == null)
            {
                compareLogic = new CompareLogic();
                compareLogic.Config.IgnoreConcreteTypes = true;
                compareLogic.Config.IgnoreObjectTypes = true;
            }
            return compareLogic;
        }

        #endregion Methods
    }
}
