// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
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
        private readonly string[] DLCDirectories = new string[] { GameRootPathResolver.DLCFolder, "builtin_dlc" };

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
            if (game != null && (dlc?.Any()).GetValueOrDefault())
            {
                var disabledDLC = dlc.Where(p => !p.IsEnabled).ToList();
                return dlcExporter.ExportDLCAsync(new DLCParameters()
                {
                    RootPath = game.UserDirectory,
                    DLC = disabledDLC,
                    DescriptorType = MapDescriptorType(game.ModDescriptorType)
                });
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
                var cached = cache.Get<List<IDLC>>(new CacheGetParameters() { Region = CacheRegion, Key = game.Type });
                if (cached != null)
                {
                    return Task.FromResult((IReadOnlyCollection<IDLC>)cached);
                }
                var result = new List<IDLC>();
                if (!string.IsNullOrWhiteSpace(game.ExecutableLocation))
                {
                    var cleanedExePath = pathResolver.GetPath(game);
                    if (!string.IsNullOrWhiteSpace(cleanedExePath))
                    {
                        foreach (var dlcFolder in DLCDirectories)
                        {
                            var directory = Path.Combine(cleanedExePath, pathResolver.ResolveDLCDirectory(game.DLCContainer, dlcFolder));
                            if (Directory.Exists(directory))
                            {
                                var infos = reader.Read(directory);
                                if (infos != null && infos.Any())
                                {
                                    foreach (var item in infos)
                                    {
                                        var dlcObject = dlcParser.Parse(Path.Combine(dlcFolder, item.FileName), item.Content, MapDescriptorModType(game.ModDescriptorType));
                                        result.Add(Mapper.Map<IDLC>(dlcObject));
                                    }
                                }
                            }
                        }
                    }
                    cache.Set(new CacheAddParameters<List<IDLC>>() { Region = CacheRegion, Key = game.Type, Value = result });
                }
                return Task.FromResult((IReadOnlyCollection<IDLC>)result);
            }
            return Task.FromResult((IReadOnlyCollection<IDLC>)new List<IDLC>());
        }

        /// <summary>
        /// synchronize state as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> SyncStateAsync(IGame game, IReadOnlyCollection<IDLC> dlc)
        {
            if (game != null && (dlc?.Any()).GetValueOrDefault())
            {
                foreach (var item in dlc)
                {
                    item.IsEnabled = true;
                }
                var disabledDLC = await dlcExporter.GetDisabledDLCAsync(new DLCParameters()
                {
                    RootPath = game.UserDirectory,
                    DescriptorType = MapDescriptorType(game.ModDescriptorType)
                });
                if ((disabledDLC?.Any()).GetValueOrDefault())
                {
                    foreach (var item in disabledDLC)
                    {
                        IDLC matchedDLC;
                        if (game.ModDescriptorType == ModDescriptorType.DescriptorMod)
                        {
                            matchedDLC = dlc.FirstOrDefault(p => p.Path.Equals(item.Path, StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            matchedDLC = dlc.FirstOrDefault(p => p.AppId.Equals(item.AppId, StringComparison.OrdinalIgnoreCase));
                        }
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
    }
}
