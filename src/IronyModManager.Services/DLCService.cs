// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="DLCService.cs" company="Mario">
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
using IronyModManager.IO.Common.DLC;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Services.Common;
using IronyModManager.Services.Resolver;
using IronyModManager.Shared.Models;
using IronyModManager.Shared.Cache;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class DLCService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IDLCService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IDLCService" />
    /// <remarks>Initializes a new instance of the <see cref="DLCService" /> class.</remarks>
    public class DLCService(IDLCExporter dlcExporter, ICache cache, IReader reader, IDLCParser dlcParser, IStorageProvider storage, IMapper mapper,
        IGameStateSafetyService gameStateSafetyService) : BaseService(storage, mapper), IDLCService
    {
        #region Fields

        /// <summary>
        /// The cache region
        /// </summary>
        private const string CacheRegion = "DLC";

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache = cache;

        /// <summary>
        /// The DLC directories
        /// </summary>
        private readonly string[] dlcDirectories = [GameRootPathResolver.DLCFolder, "builtin_dlc"];

        /// <summary>
        /// The DLC exporter
        /// </summary>
        private readonly IDLCExporter dlcExporter = dlcExporter;

        /// <summary>
        /// The DLC parser
        /// </summary>
        private readonly IDLCParser dlcParser = dlcParser;

        /// <summary>
        /// The path resolver
        /// </summary>
#pragma warning disable CA1859
        private readonly IGameRootPathResolver pathResolver = new GameRootPathResolver();
#pragma warning restore CA1859

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader = reader;

        /// <summary>
        /// The per-game filesystem safety state.
        /// </summary>
        private readonly IGameStateSafetyService gameStateSafetyService = gameStateSafetyService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ExportAsync(IGame game, IReadOnlyCollection<IDLC> dlc)
        {
            if (game != null && dlc != null && dlc.Count != 0)
            {
                var disabledDLC = dlc.Where(p => !p.IsEnabled).ToList();
                var parameters = new DLCParameters
                {
                    RootPath = game.UserDirectory,
                    DLC = disabledDLC,
                    DescriptorType = MapDescriptorType(game.ModDescriptorType)
                };
                var sourceAvailable = await gameStateSafetyService.ExecuteReadAsync(game, async () =>
                {
                    _ = await dlcExporter.GetDisabledDLCAsync(parameters);
                    return true;
                }, false, "Read DLC state for export");
                if (!sourceAvailable)
                {
                    return false;
                }

                return await gameStateSafetyService.ExecuteMutationAsync(game,
                    () => dlcExporter.ExportDLCAsync(parameters), false, "Export DLC state");
            }

            return false;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IDLC&gt;.</returns>
        public virtual async Task<IReadOnlyCollection<IDLC>> GetAsync(IGame game)
        {
            if (game == null)
            {
                return [];
            }

            var cached = cache.Get<DLCCacheHolder>(new CacheGetParameters { Region = CacheRegion, Key = game.Type });
            var exeLoc = !string.IsNullOrWhiteSpace(game.ExecutableLocation) ? game.ExecutableLocation : string.Empty;
            if (cached != null && cached.GameExe.Equals(exeLoc))
            {
                return cached.DLC;
            }

            if (gameStateSafetyService.IsLocked(game))
            {
                return cached?.DLC ?? [];
            }

            return await gameStateSafetyService.ExecuteReadAsync(game, () => GetInternalAsync(game), cached?.DLC,
                "Read DLC catalog") ?? [];
        }

        private Task<IReadOnlyCollection<IDLC>> GetInternalAsync(IGame game)
        {
            var result = new List<IDLC>();
            if (!string.IsNullOrWhiteSpace(game.ExecutableLocation))
            {
                var cleanedExePath = pathResolver.GetPath(game);
                if (!string.IsNullOrWhiteSpace(cleanedExePath))
                {
                    foreach (var dlcFolder in dlcDirectories)
                    {
                        var directory = Path.Combine(cleanedExePath, pathResolver.ResolveDLCDirectory(game.DLCContainer, dlcFolder));
                        if (Directory.Exists(directory))
                        {
                            var infos = reader.Read(directory);
                            if (infos != null && infos.Any())
                            {
                                result.AddRange(infos.Select(item => dlcParser.Parse(Path.Combine(dlcFolder, item.FileName), item.Content, MapDescriptorModType(game.ModDescriptorType))).Select(dlcObject => Mapper.Map<IDLC>(dlcObject)));
                            }
                        }
                    }
                }

                cache.Set(new CacheAddParameters<DLCCacheHolder> { Region = CacheRegion, Key = game.Type, Value = new DLCCacheHolder(result, game.ExecutableLocation) });
            }

            return Task.FromResult<IReadOnlyCollection<IDLC>>(result);
        }

        /// <summary>
        /// synchronize state as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> SyncStateAsync(IGame game, IReadOnlyCollection<IDLC> dlc)
        {
            if (game != null && dlc != null && dlc.Count != 0)
            {
                var disabledDLC = await dlcExporter.GetDisabledDLCAsync(new DLCParameters { RootPath = game.UserDirectory, DescriptorType = MapDescriptorType(game.ModDescriptorType) });

                var disabledPaths = disabledDLC?.Select(p => p.Path).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];
                var disabledAppIds = disabledDLC?.Select(p => p.AppId).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

                foreach (var item in dlc)
                {
                    item.IsEnabled = game.ModDescriptorType switch
                    {
                        ModDescriptorType.DescriptorMod => !disabledPaths.Contains(item.Path),
                        _ => !disabledAppIds.Contains(item.AppId)
                    };
                }

                return true;
            }

            return false;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DLCCacheHolder.
        /// </summary>
        /// <remarks>Initializes a new instance of the <see cref="DLCCacheHolder" /> class.</remarks>
        public class DLCCacheHolder(List<IDLC> dlc, string gameExe)
        {
            #region Properties

            /// <summary>
            /// Gets or sets the DLC.
            /// </summary>
            /// <value>The DLC.</value>
            public List<IDLC> DLC { get; set; } = dlc;

            /// <summary>
            /// Gets or sets the game executable.
            /// </summary>
            /// <value>The game executable.</value>
            public string GameExe { get; set; } = gameExe ?? string.Empty;

            #endregion Properties
        }

        #endregion Classes
    }
}
