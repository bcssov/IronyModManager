// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
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
    public class DLCService : BaseService, IDLCService
    {
        #region Fields

        /// <summary>
        /// The cache region
        /// </summary>
        private const string CacheRegion = "DLC";

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache;

        /// <summary>
        /// The DLC directories
        /// </summary>
        private readonly string[] dlcDirectories = [GameRootPathResolver.DLCFolder, "builtin_dlc"];

        /// <summary>
        /// The DLC exporter
        /// </summary>
        private readonly IDLCExporter dlcExporter;

        /// <summary>
        /// The DLC parser
        /// </summary>
        private readonly IDLCParser dlcParser;

        /// <summary>
        /// The path resolver
        /// </summary>
        private readonly GameRootPathResolver pathResolver;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCService" /> class.
        /// </summary>
        /// <param name="dlcExporter">The DLC exporter.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="dlcParser">The DLC parser.</param>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        public DLCService(IDLCExporter dlcExporter, ICache cache, IReader reader, IDLCParser dlcParser, IStorageProvider storage, IMapper mapper) : base(storage, mapper)
        {
            this.dlcExporter = dlcExporter;
            this.reader = reader;
            this.dlcParser = dlcParser;
            this.cache = cache;
            pathResolver = new GameRootPathResolver();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExportAsync(IGame game, IReadOnlyCollection<IDLC> dlc)
        {
            if (game != null && dlc != null && dlc.Count != 0)
            {
                var disabledDLC = dlc.Where(p => !p.IsEnabled).ToList();
                return dlcExporter.ExportDLCAsync(new DLCParameters { RootPath = game.UserDirectory, DLC = disabledDLC, DescriptorType = MapDescriptorType(game.ModDescriptorType) });
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IDLC&gt;.</returns>
        public virtual Task<IReadOnlyCollection<IDLC>> GetAsync(IGame game)
        {
            if (game != null)
            {
                var cached = cache.Get<DLCCacheHolder>(new CacheGetParameters { Region = CacheRegion, Key = game.Type });
                var exeLoc = !string.IsNullOrWhiteSpace(game.ExecutableLocation) ? game.ExecutableLocation : string.Empty;
                if (cached != null && cached.GameExe.Equals(exeLoc))
                {
                    return Task.FromResult<IReadOnlyCollection<IDLC>>(cached.DLC);
                }

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

            return Task.FromResult<IReadOnlyCollection<IDLC>>(new List<IDLC>());
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
                foreach (var item in dlc)
                {
                    item.IsEnabled = true;
                }

                var disabledDLC = await dlcExporter.GetDisabledDLCAsync(new DLCParameters { RootPath = game.UserDirectory, DescriptorType = MapDescriptorType(game.ModDescriptorType) });
                if (disabledDLC != null && disabledDLC.Count != 0)
                {
                    foreach (var item in disabledDLC)
                    {
                        var matchedDLC = game.ModDescriptorType switch
                        {
                            ModDescriptorType.DescriptorMod => dlc.FirstOrDefault(p => p.Path.Equals(item.Path, StringComparison.OrdinalIgnoreCase)),
                            _ => dlc.FirstOrDefault(p => p.AppId.Equals(item.AppId, StringComparison.OrdinalIgnoreCase))
                        };

                        if (matchedDLC != null)
                        {
                            matchedDLC.IsEnabled = false;
                        }
                    }
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
        public class DLCCacheHolder
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DLCCacheHolder" /> class.
            /// </summary>
            /// <param name="dlc">The DLC.</param>
            /// <param name="gameExe">The game executable.</param>
            public DLCCacheHolder(List<IDLC> dlc, string gameExe)
            {
                DLC = dlc;
                GameExe = gameExe ?? string.Empty;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets or sets the DLC.
            /// </summary>
            /// <value>The DLC.</value>
            public List<IDLC> DLC { get; set; }

            /// <summary>
            /// Gets or sets the game executable.
            /// </summary>
            /// <value>The game executable.</value>
            public string GameExe { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
