// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 10-31-2021
// ***********************************************************************
// <copyright file="GameIndexService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Game;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Services.Resolver;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using Nito.AsyncEx;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class GameIndexService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IGameIndexService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IGameIndexService" />
    public class GameIndexService : ModBaseService, IGameIndexService
    {
        #region Fields

        /// <summary>
        /// The service lock
        /// </summary>
        private static readonly AsyncLock asyncServiceLock = new();

        /// <summary>
        /// The current cache version
        /// </summary>
        private readonly int CurrentCacheVersion = 4;

        /// <summary>
        /// The game indexer
        /// </summary>
        private readonly IGameIndexer gameIndexer;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The parser manager
        /// </summary>
        private readonly IParserManager parserManager;

        /// <summary>
        /// The path resolver
        /// </summary>
        private readonly GameRootPathResolver pathResolver;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameIndexService" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="gameIndexer">The game indexer.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public GameIndexService(IMessageBus messageBus, IParserManager parserManager, IGameIndexer gameIndexer, ICache cache, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IReader reader,
            IModWriter modWriter, IModParser modParser, IGameService gameService, IStorageProvider storageProvider, IMapper mapper) :
            base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.gameIndexer = gameIndexer;
            this.messageBus = messageBus;
            this.parserManager = parserManager;
            pathResolver = new GameRootPathResolver();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// index definitions as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual async Task<bool> IndexDefinitionsAsync(IGame game, IEnumerable<string> versions, IIndexedDefinitions indexedDefinitions)
        {
            if (game != null && versions != null && versions.Any())
            {
                await messageBus.PublishAsync(new GameIndexProgressEvent(0));
                if (!await gameIndexer.GameVersionsSameAsync(GetStoragePath(), game, versions) || !await gameIndexer.CachedDefinitionsSameAsync(GetStoragePath(), game, CurrentCacheVersion))
                {
                    await gameIndexer.ClearDefinitionAsync(GetStoragePath(), game);
                    await gameIndexer.WriteVersionAsync(GetStoragePath(), game, versions, CurrentCacheVersion);
                }
                var gamePath = pathResolver.GetPath(game);
                var files = Reader.GetFiles(gamePath);
                if (files.Any())
                {
                    files = files.Where(p => game.GameFolders.Any(x => p.StartsWith(x)));
                    var indexedFolders = indexedDefinitions.GetAllDirectoryKeys().Select(p => p.ToLowerInvariant());
                    var validFolders = files.Select(p => Path.GetDirectoryName(p)).GroupBy(p => p).Select(p => p.FirstOrDefault()).Where(p => indexedFolders.Any(a => a.ToLowerInvariant().Equals(p.ToLowerInvariant())));
                    var folders = new List<string>();
                    foreach (var item in validFolders)
                    {
                        if (!await gameIndexer.FolderCachedAsync(GetStoragePath(), game, item))
                        {
                            folders.Add(item);
                        }
                    }
                    if (folders.Any())
                    {
                        double processed = 0;
                        double total = folders.Count;
                        double previousProgress = 0;
                        var tasks = folders.AsParallel().Select(async folder =>
                        {
                            await Task.Run(async () =>
                            {
                                var result = ParseGameFiles(game, Reader.Read(Path.Combine(gamePath, folder), searchSubFolders: false), folder);
                                if ((result?.Any()).GetValueOrDefault())
                                {
                                    await gameIndexer.SaveDefinitionsAsync(GetStoragePath(), game, result);
                                }
                            });
                            using var mutex = await asyncServiceLock.LockAsync();
                            processed++;
                            var perc = GetProgressPercentage(total, processed, 100);
                            if (perc != previousProgress)
                            {
                                await messageBus.PublishAsync(new GameIndexProgressEvent(perc));
                                previousProgress = perc;
                            }
                            mutex.Dispose();
                        });
                        await Task.WhenAll(tasks);
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// load definitions as an asynchronous operation.
        /// </summary>
        /// <param name="modDefinitions">The mod definitions.</param>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public virtual async Task<IIndexedDefinitions> LoadDefinitionsAsync(IIndexedDefinitions modDefinitions, IGame game, IEnumerable<string> versions)
        {
            if (game != null && versions != null && versions.Any() && await gameIndexer.GameVersionsSameAsync(GetStoragePath(), game, versions))
            {
                var gameDefinitions = new ConcurrentBag<IDefinition>();
                var directories = modDefinitions.GetAllDirectoryKeys();
                double processed = 0;
                double total = directories.Count();
                double previousProgress = 0;
                var tasks = directories.Select(async directory =>
                {
                    var definitions = await Task.Run(async () => await gameIndexer.GetDefinitionsAsync(GetStoragePath(), game, directory));
                    if ((definitions?.Any()).GetValueOrDefault())
                    {
                        foreach (var def in definitions)
                        {
                            def.ModName = game.Name;
                            def.IsFromGame = true;
                        }
                        foreach (var item in definitions)
                        {
                            gameDefinitions.Add(item);
                        }
                        processed++;
                        var perc = GetProgressPercentage(total, processed, 100);
                        if (perc != previousProgress)
                        {
                            await messageBus.PublishAsync(new GameDefinitionLoadProgressEvent(perc));
                            previousProgress = perc;
                        }
                    }
                });
                await Task.WhenAll(tasks);
                var indexed = DIResolver.Get<IIndexedDefinitions>();
                indexed.InitMap(modDefinitions.GetAll().Concat(gameDefinitions));
                return indexed;
            }
            return modDefinitions;
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
        /// Gets the storage path.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetStoragePath()
        {
            return StorageProvider.GetRootStoragePath();
        }

        /// <summary>
        /// Parses the game files.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="fileInfos">The file infos.</param>
        /// <param name="folder">The folder.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseGameFiles(IGame game, IEnumerable<IFileInfo> fileInfos, string folder)
        {
            if (fileInfos == null)
            {
                return null;
            }
            var definitions = new List<IDefinition>();
            foreach (var fileInfo in fileInfos)
            {
                var fileDefs = parserManager.Parse(new ParserManagerArgs()
                {
                    ContentSHA = fileInfo.ContentSHA,
                    File = Path.Combine(folder, fileInfo.FileName),
                    GameType = game.Type,
                    Lines = fileInfo.Content,
                    ModName = game.Name
                }).Where(p => p.ValueType != Shared.Models.ValueType.Invalid);
                MergeDefinitions(fileDefs);
                definitions.AddRange(fileDefs);
            }
            return definitions;
        }

        #endregion Methods
    }
}
