// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 02-11-2025
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
using System.Threading;
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
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using Nito.AsyncEx;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class GameIndexService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IGameIndexService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IGameIndexService" />
    public class GameIndexService(
        IParametrizedParser parametrizedParser,
        IMessageBus messageBus,
        IParserManager parserManager,
        IGameIndexer gameIndexer,
        ICache cache,
        IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
        IReader reader,
        IModWriter modWriter,
        IModParser modParser,
        IGameService gameService,
        IStorageProvider storageProvider,
        IMapper mapper) : ModBaseService(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper), IGameIndexService
    {
        #region Fields

        /// <summary>
        /// The maximum folders to process
        /// </summary>
        private const int MaxFoldersToProcess = 4;

        /// <summary>
        /// The service lock
        /// </summary>
        private static readonly AsyncLock asyncServiceLock = new();

        /// <summary>
        /// The game indexer
        /// </summary>
        private readonly IGameIndexer gameIndexer = gameIndexer;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus = messageBus;

        /// <summary>
        /// The parser manager
        /// </summary>
        private readonly IParserManager parserManager = parserManager;

        /// <summary>
        /// The parametrized parser
        /// </summary>
        private readonly IParametrizedParser parametrizedParser = parametrizedParser;

        #endregion Fields

        #region Methods

        /// <summary>
        /// index definitions as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <returns><c>true</c> if indexed, <c>false</c> otherwise.</returns>
        public virtual async Task<bool> IndexDefinitionsAsync(IGame game, IEnumerable<string> versions, IIndexedDefinitions indexedDefinitions)
        {
            if (game != null && versions != null && versions.Any())
            {
                await messageBus.PublishAsync(new GameIndexProgressEvent(0));
                if (!await gameIndexer.GameVersionsSameAsync(GetStoragePath(), game, versions) || !await gameIndexer.CachedDefinitionsSameAsync(GetStoragePath(), game, game.GameIndexCacheVersion))
                {
                    await gameIndexer.ClearDefinitionAsync(GetStoragePath(), game);
                    await gameIndexer.WriteVersionAsync(GetStoragePath(), game, versions, game.GameIndexCacheVersion);
                }

                var gamePath = PathResolver.GetPath(game);
                var files = Reader.GetFiles(gamePath);

                // No clue how someone got reader to return 0 based on configuration alone but just in case to ignore this mess
                if (files != null && files.Any())
                {
                    var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
                    files = files.Where(p => game.GameFolders.Any(p.StartsWith));

                    // Inline stuff
                    var gameInlineAndVarFolders = new List<string>();
                    if (provider!.SupportsInlineScripts)
                    {
                        var gameInlineScriptFiles = files.Where(p => p.StartsWith(provider.InlineScriptsPath, StringComparison.OrdinalIgnoreCase) || p.StartsWith(provider.GlobalVariablesPath, StringComparison.OrdinalIgnoreCase));
                        gameInlineAndVarFolders = gameInlineScriptFiles.Select(Path.GetDirectoryName).GroupBy(p => p).Select(p => p.FirstOrDefault()).ToList();
                    }

                    var indexedFolders = (await indexedDefinitions.GetAllDirectoryKeysAsync()).Select(p => p.ToLowerInvariant());
                    var validFolders = files.Select(Path.GetDirectoryName).GroupBy(p => p).Select(p => p.FirstOrDefault()).Where(p => indexedFolders.Any(a => a.ToLowerInvariant().Equals(p!.ToLowerInvariant())));
                    var folders = new List<string>();
                    foreach (var item in validFolders)
                    {
                        if (!await gameIndexer.FolderCachedAsync(GetStoragePath(), game, item))
                        {
                            folders.Add(item);
                        }
                    }

                    if (provider.SupportsInlineScripts)
                    {
                        // Inline scripts folders must always be indexed
                        foreach (var folder in gameInlineAndVarFolders)
                        {
                            if (!await gameIndexer.FolderCachedAsync(GetStoragePath(), game, folder))
                            {
                                folders.Add(folder);
                            }
                        }
                    }

                    folders = folders.Distinct().ToList();

                    if (folders.Count != 0)
                    {
                        double processed = 0;
                        double total = folders.Count;
                        double previousProgress = 0;

                        using var semaphore = new SemaphoreSlim(MaxFoldersToProcess);
                        var inlineDefinitions = DIResolver.Get<IIndexedDefinitions>();
                        var scriptedVars = DIResolver.Get<IIndexedDefinitions>();

                        // First index inline scripts folder and vars folders
                        if (provider.SupportsInlineScripts)
                        {
                            total += gameInlineAndVarFolders.Count;
                            var inlineAndVarsFolders = folders.Where(p => p.StartsWith(provider.InlineScriptsPath, StringComparison.OrdinalIgnoreCase) || p.StartsWith(provider.GlobalVariablesPath, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                            var inlineAndVarTasks = inlineAndVarsFolders.AsParallel().Select(async folder =>
                            {
                                await semaphore.WaitAsync();
                                try
                                {
                                    await Task.Run(async () =>
                                    {
                                        var result = await ParseGameFiles(game, Reader.Read(Path.Combine(gamePath, folder), searchSubFolders: false), folder, null, null);
                                        if ((result?.Any()).GetValueOrDefault())
                                        {
                                            await gameIndexer.SaveDefinitionsAsync(GetStoragePath(), game, result);
                                        }
                                    });
                                    using var mutex = await asyncServiceLock.LockAsync();
                                    processed++;
                                    var perc = GetProgressPercentage(total, processed, 100);
                                    if (perc.IsNotNearlyEqual(previousProgress))
                                    {
                                        await messageBus.PublishAsync(new GameIndexProgressEvent(perc));
                                        previousProgress = perc;
                                    }

                                    GCRunner.RunGC(GCCollectionMode.Optimized);

                                    // ReSharper disable once DisposeOnUsingVariable
                                    mutex.Dispose();
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            });
                            await Task.WhenAll(inlineAndVarTasks);

                            folders = folders.Where(p => !p.StartsWith(provider.InlineScriptsPath, StringComparison.OrdinalIgnoreCase) && !p.StartsWith(provider.GlobalVariablesPath, StringComparison.OrdinalIgnoreCase)).ToList();

                            var loadTasks = gameInlineAndVarFolders.Select(async directory =>
                            {
                                await semaphore.WaitAsync();
                                try
                                {
                                    if (directory.StartsWith(provider.InlineScriptsPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        inlineDefinitions = await LoadDefinitionsInternalAsync(inlineDefinitions, game, versions, null, directory);
                                    }
                                    else
                                    {
                                        scriptedVars = await LoadDefinitionsInternalAsync(scriptedVars, game, versions, null, directory);
                                    }

                                    using var mutex = await asyncServiceLock.LockAsync();
                                    processed++;
                                    var perc = GetProgressPercentage(total, processed, 100);
                                    if (perc.IsNotNearlyEqual(previousProgress))
                                    {
                                        await messageBus.PublishAsync(new GameIndexProgressEvent(perc));
                                        previousProgress = perc;
                                    }

                                    GCRunner.RunGC(GCCollectionMode.Optimized);

                                    // ReSharper disable once DisposeOnUsingVariable
                                    mutex.Dispose();
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }).ToList();
                            await Task.WhenAll(loadTasks);
                        }

                        var tasks = folders.AsParallel().Select(async folder =>
                        {
                            await semaphore.WaitAsync();
                            try
                            {
                                await Task.Run(async () =>
                                {
                                    var result = await ParseGameFiles(game, Reader.Read(Path.Combine(gamePath, folder), searchSubFolders: false), folder, inlineDefinitions, scriptedVars);
                                    if ((result?.Any()).GetValueOrDefault())
                                    {
                                        await gameIndexer.SaveDefinitionsAsync(GetStoragePath(), game, result);
                                    }
                                });
                                using var mutex = await asyncServiceLock.LockAsync();
                                processed++;
                                var perc = GetProgressPercentage(total, processed, 100);
                                if (perc.IsNotNearlyEqual(previousProgress))
                                {
                                    await messageBus.PublishAsync(new GameIndexProgressEvent(perc));
                                    previousProgress = perc;
                                }

                                GCRunner.RunGC(GCCollectionMode.Optimized);

                                // ReSharper disable once DisposeOnUsingVariable
                                mutex.Dispose();
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        });

                        await Task.WhenAll(tasks);
                        inlineDefinitions.Dispose();
                        inlineDefinitions = null;
                        scriptedVars.Dispose();
                        scriptedVars = null;
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
        /// <param name="gameLanguages">The game languages.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public virtual async Task<IIndexedDefinitions> LoadDefinitionsAsync(IIndexedDefinitions modDefinitions, IGame game, IEnumerable<string> versions, IReadOnlyCollection<IGameLanguage> gameLanguages)
        {
            return await LoadDefinitionsInternalAsync(modDefinitions, game, versions, gameLanguages);
        }

        /// <summary>
        /// Load definitions internal as an asynchronous operation.
        /// </summary>
        /// <param name="modDefinitions">The mod definitions.</param>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <param name="gameLanguages">The game languages.</param>
        /// <param name="directoryOverride">The directory override.</param>
        /// <returns>A Task&lt;IIndexedDefinitions&gt; representing the asynchronous operation.</returns>
        protected virtual async Task<IIndexedDefinitions> LoadDefinitionsInternalAsync(IIndexedDefinitions modDefinitions, IGame game, IEnumerable<string> versions, IReadOnlyCollection<IGameLanguage> gameLanguages,
            string directoryOverride = Shared.Constants.EmptyParam)
        {
            if (game != null && versions != null && versions.Any() && await gameIndexer.GameVersionsSameAsync(GetStoragePath(), game, versions))
            {
                var gameDefinitions = new ConcurrentBag<IDefinition>();
                var directories = (await modDefinitions.GetAllDirectoryKeysAsync()).ToList();
                if (!string.IsNullOrWhiteSpace(directoryOverride))
                {
                    directories = [directoryOverride];
                }

                // Kinda need to force insert localisation directory itself
                if (directories.Any(p => p.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase)) &&
                    !directories.Any(p => p.Equals(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase)))
                {
                    var newDirs = new List<string>(directories) { Shared.Constants.LocalizationDirectory };
                    directories = newDirs;
                }

                // No directory override this means we want to ensure inline directories are loaded even if not requested
                if (string.IsNullOrWhiteSpace(directoryOverride))
                {
                    var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
                    if (provider!.SupportsInlineScripts)
                    {
                        var gamePath = PathResolver.GetPath(game);
                        var files = Reader.GetFiles(gamePath);
                        files = files.Where(p => game.GameFolders.Any(p.StartsWith));
                        var gameInlineScriptFiles = files.Where(p => p.StartsWith(provider!.InlineScriptsPath, StringComparison.OrdinalIgnoreCase));
                        var gameInlineFolders = gameInlineScriptFiles.Select(Path.GetDirectoryName).GroupBy(p => p).Select(p => p.FirstOrDefault()).ToList();
                        directories.AddRange(gameInlineFolders);
                        directories = directories.Distinct().ToList();
                    }
                }

                if (gameLanguages != null && gameLanguages.Count != 0)
                {
                    var folders = gameLanguages.Select(p => p.Type[2..]);
                    var filtered = new List<string>();
                    foreach (var dir in directories)
                    {
                        // So far games that support CS have structure localisation -> l_english -> files though language id part is stripped (l_ part)
                        if (dir.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
                        {
                            if (dir.StandardizeDirectorySeparator().Any(p => p == Path.DirectorySeparatorChar))
                            {
                                if (folders.Any(p => dir!.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
                                {
                                    filtered.Add(dir);
                                }
                            }
                            else
                            {
                                filtered.Add(dir);
                            }
                        }
                        else
                        {
                            filtered.Add(dir);
                        }
                    }

                    directories = filtered;
                }

                double processed = 0;
                double total = directories.Count;
                double previousProgress = 0;
                using var semaphore = new SemaphoreSlim(MaxFoldersToProcess);
                var tasks = directories.Select(async directory =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var definitions = await Task.Run(async () => await gameIndexer.GetDefinitionsAsync(GetStoragePath(), game, directory));
                        if ((definitions?.Any()).GetValueOrDefault())
                        {
                            foreach (var def in definitions!)
                            {
                                def.ModName = game.Name;
                                def.IsFromGame = true;
                            }

                            return definitions;
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }

                    return null;
                }).ToList();
                while (tasks.Count != 0)
                {
                    var result = await Task.WhenAny(tasks);
                    tasks.Remove(result);
                    if (result.Result != null)
                    {
                        foreach (var item in result.Result)
                        {
                            gameDefinitions.Add(item);
                        }
                    }

                    processed++;
                    var perc = GetProgressPercentage(total, processed, 100);
                    if (perc.IsNotNearlyEqual(previousProgress) && string.IsNullOrWhiteSpace(directoryOverride)) // Directory override present, probably an internal call
                    {
                        await messageBus.PublishAsync(new GameDefinitionLoadProgressEvent(perc));
                        previousProgress = perc;
                    }

                    GCRunner.RunGC(GCCollectionMode.Optimized);
                }

                foreach (var item in gameDefinitions)
                {
                    await modDefinitions.AddToMapAsync(item);
                }
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
        /// <param name="inlineDefinitions">The inline definitions.</param>
        /// <param name="scriptedVariables">The scripted variables.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        /// <exception cref="ArgumentException">$"Inline code-block from file: {item.File} could not be parsed from the game., e</exception>
        protected virtual async Task<IEnumerable<IDefinition>> ParseGameFiles(IGame game, IEnumerable<IFileInfo> fileInfos, string folder, IIndexedDefinitions inlineDefinitions, IIndexedDefinitions scriptedVariables)
        {
            if (fileInfos == null)
            {
                return null;
            }

            var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));

            var definitions = new List<IDefinition>();
            foreach (var fileInfo in fileInfos)
            {
                var fileDefs = parserManager.Parse(new ParserManagerArgs
                {
                    ContentSHA = fileInfo.ContentSHA,
                    File = Path.Combine(folder, fileInfo.FileName),
                    GameType = game.Type,
                    Lines = fileInfo.Content,
                    ModName = game.Name,
                    ValidationType = ValidationType.SkipAll
                }).Where(p => p.ValueType != ValueType.Invalid);
                MergeDefinitions(fileDefs);
                definitions.AddRange(fileDefs);
            }

            List<IDefinition> prunedInlineDefinitions = null;
            if (provider!.SupportsInlineScripts && !folder.StandardizeDirectorySeparator().StartsWith(provider.InlineScriptsPath, StringComparison.OrdinalIgnoreCase))
            {
                scriptedVariables ??= DIResolver.Get<IIndexedDefinitions>();
                var scriptedVarsBucket = await scriptedVariables.GetAllAsync();
                if (scriptedVarsBucket != null)
                {
                    var varFiles = scriptedVarsBucket.GroupBy(p => p.File).Select(p => p.FirstOrDefault()).OrderBy(p => p!.FileCI, StringComparer.Ordinal).Select(p => p.FileCI).ToList();
                    scriptedVarsBucket = scriptedVarsBucket.GroupBy(p => p.Id).Select(p => EvalDefinitionPriorityInternal(p.OrderBy(f => varFiles.IndexOf(f.FileCI))).Definition).ToList();
                }

                inlineDefinitions ??= DIResolver.Get<IIndexedDefinitions>();
                prunedInlineDefinitions = [];

                foreach (var item in definitions)
                {
                    var def = item;
                    var addDefault = true;
                    while (def.Id.Equals(Parser.Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase) || def.ContainsInlineIdentifier)
                    {
                        addDefault = false;
                        var path = Path.Combine(Parser.Common.Constants.Stellaris.InlineScripts, parametrizedParser.GetScriptPath(def.Code));
                        var pathCI = path.ToLowerInvariant();
                        var files = (await inlineDefinitions.GetByParentDirectoryAsync(Path.GetDirectoryName(path))).Where(p => Path.Combine(Path.GetDirectoryName(p.FileCI)!, Path.GetFileNameWithoutExtension(p.FileCI)!).Equals(pathCI))
                            .ToList();
                        if (files.Count != 0)
                        {
                            var priorityDefinition = EvalDefinitionPriorityInternal([.. files.OrderBy(p => p.FileCI, StringComparer.Ordinal)]);
                            if (priorityDefinition is { Definition: not null })
                            {
                                var vars = new List<IDefinition>();
                                if (item.Variables != null)
                                {
                                    vars.AddRange(item.Variables);
                                }

                                var parametrizedCode = string.Empty;
                                IEnumerable<IDefinition> ids = null;
                                try
                                {
                                    var processedInline = ProcessInlineConstants(def.Code, vars, scriptedVarsBucket, out _);
                                    parametrizedCode = parametrizedParser.Process(priorityDefinition.Definition.Code, processedInline);
                                }
                                catch (Exception e)
                                {
                                    throw new ArgumentException($"Inline code-block from file: {item.File} could not be parsed from the game.", e);
                                }

                                if (!string.IsNullOrWhiteSpace(parametrizedCode))
                                {
                                    var results = parserManager.Parse(new ParserManagerArgs
                                    {
                                        ContentSHA = item.ContentSHA, // Want original file sha id
                                        File = item.File, // To trigger right parser
                                        GameType = game.Type,
                                        Lines = parametrizedCode.SplitOnNewLine(),
                                        FileLastModified = item.LastModified,
                                        ModDependencies = item.Dependencies,
                                        IsBinary = item.ValueType == ValueType.Binary,
                                        ModName = item.ModName,
                                        ValidationType = ValidationType.SkipAll
                                    });
                                    if (item.Variables != null && item.Variables.Any() && results != null)
                                    {
                                        MergeDefinitions(results.Concat(item.Variables));
                                    }

                                    if (results != null && results.Any())
                                    {
                                        var inline = results.FirstOrDefault(p => p.Id.Equals(Parser.Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase) || p.ContainsInlineIdentifier);
                                        if (inline == null)
                                        {
                                            prunedInlineDefinitions.AddRange(results);
                                            break;
                                        }
                                        else
                                        {
                                            def = inline;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (addDefault)
                    {
                        prunedInlineDefinitions.Add(item);
                    }
                }

                GCRunner.RunGC(GCCollectionMode.Optimized);
            }
            else
            {
                prunedInlineDefinitions = [.. definitions];
            }

            return prunedInlineDefinitions;
        }
    }

    #endregion Methods
}
