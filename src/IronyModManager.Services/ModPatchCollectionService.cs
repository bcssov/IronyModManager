// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 09-02-2020
// ***********************************************************************
// <copyright file="ModPatchCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModPatchCollectionService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModPatchCollectionService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModPatchCollectionService" />
    public class ModPatchCollectionService : ModBaseService, IModPatchCollectionService
    {
        #region Fields

        /// <summary>
        /// The mod name ignore identifier
        /// </summary>
        private const string ModNameIgnoreId = "modName:";

        /// <summary>
        /// The service lock
        /// </summary>
        private static readonly object serviceLock = new { };

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The mod patch exporter
        /// </summary>
        private readonly IModPatchExporter modPatchExporter;

        /// <summary>
        /// The parser manager
        /// </summary>
        private readonly IParserManager parserManager;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModPatchCollectionService" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModPatchCollectionService(ICache cache, IMessageBus messageBus, IParserManager parserManager, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IModPatchExporter modPatchExporter, IReader reader, IModWriter modWriter, IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.messageBus = messageBus;
            this.parserManager = parserManager;
            this.modPatchExporter = modPatchExporter;
        }

        #endregion Constructors

        #region Enums

        /// <summary>
        /// Enum ExportType
        /// </summary>
        protected enum ExportType
        {
            /// <summary>
            /// The resolved
            /// </summary>
            Resolved,

            /// <summary>
            /// The ignored
            /// </summary>
            Ignored,

            /// <summary>
            /// The custom
            /// </summary>
            Custom
        }

        #endregion Enums

        #region Methods

        /// <summary>
        /// Adds the custom mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> AddCustomModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName)
        {
            if (definition != null)
            {
                definition.ModName = GenerateCollectionPatchName(collectionName);
            }
            return ExportModPatchDefinitionAsync(conflictResult, definition, collectionName, ExportType.Custom);
        }

        /// <summary>
        /// Adds the mods to ignore list.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="mods">The mods.</param>
        /// <returns>IConflictResult.</returns>
        public virtual void AddModsToIgnoreList(IConflictResult conflictResult, IEnumerable<string> mods)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var lines = ignoredPaths.SplitOnNewLine().Where(p => !p.Trim().StartsWith("#"));
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    var parsed = line.StandardizeDirectorySeparator().Trim().TrimStart(Path.DirectorySeparatorChar);
                    if (!parsed.StartsWith(ModNameIgnoreId))
                    {
                        sb.AppendLine(line);
                    }
                }
                if (mods != null)
                {
                    foreach (var item in mods)
                    {
                        sb.AppendLine($"{ModNameIgnoreId}{item}");
                    }
                }
                conflictResult.IgnoredPaths = sb.ToString().Trim(Environment.NewLine.ToCharArray());
            }
        }

        /// <summary>
        /// apply mod patch as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ApplyModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName)
        {
            return ExportModPatchDefinitionAsync(conflictResult, definition, collectionName, ExportType.Resolved);
        }

        /// <summary>
        /// clean patch collection as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> CleanPatchCollectionAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            var patchName = GenerateCollectionPatchName(collectionName);
            var allMods = GetInstalledModsInternal(game, false).ToList();
            var mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
            if (mod != null)
            {
                await DeleteDescriptorsInternalAsync(new List<IMod> { mod });
            }
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = GetModDirectory(game, patchName)
            }, true);
            return true;
        }

        /// <summary>
        /// Copies the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="newCollectionName">New name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CopyPatchCollectionAsync(string collectionName, string newCollectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return Task.FromResult(false);
            }
            var oldPatchName = GenerateCollectionPatchName(collectionName);
            var newPathName = GenerateCollectionPatchName(newCollectionName);
            return modPatchExporter.CopyPatchModAsync(new ModPatchExporterParameters()
            {
                RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                ModPath = oldPatchName,
                PatchName = newPathName
            });
        }

        /// <summary>
        /// create patch definition as an asynchronous operation.
        /// </summary>
        /// <param name="copy">The copy.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IDefinition&gt;.</returns>
        public virtual async Task<IDefinition> CreatePatchDefinitionAsync(IDefinition copy, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null && copy != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patch = Mapper.Map<IDefinition>(copy);
                patch.ModName = GenerateCollectionPatchName(collectionName);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patch.ModName
                });
                if (state != null && state.ConflictHistory?.Count() > 0)
                {
                    var history = state.ConflictHistory.FirstOrDefault(p => p.TypeAndId.Equals(copy.TypeAndId));
                    if (history != null)
                    {
                        patch.Code = history.Code;
                    }
                }
                return patch;
            }
            return null;
        }

        /// <summary>
        /// Evals the definition priority.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IPriorityDefinitionResult.</returns>
        public virtual IPriorityDefinitionResult EvalDefinitionPriority(IEnumerable<IDefinition> definitions)
        {
            return EvalDefinitionPriorityInternal(definitions);
        }

        /// <summary>
        /// Finds the conflicts.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="patchStateMode">The patch state mode.</param>
        /// <returns>IConflictResult.</returns>
        public virtual IConflictResult FindConflicts(IIndexedDefinitions indexedDefinitions, IList<string> modOrder, PatchStateMode patchStateMode)
        {
            var conflicts = new HashSet<IDefinition>();
            var fileConflictCache = new Dictionary<string, bool>();
            var fileKeys = indexedDefinitions.GetAllFileKeys();
            var typeAndIdKeys = indexedDefinitions.GetAllTypeAndIdKeys();
            var overwritten = indexedDefinitions.GetByValueType(Parser.Common.ValueType.OverwrittenObject);
            var empty = indexedDefinitions.GetByValueType(Parser.Common.ValueType.EmptyFile);

            double total = fileKeys.Count() + typeAndIdKeys.Count() + overwritten.Count() + empty.Count();
            double processed = 0;
            var previousProgress = 0;
            messageBus.Publish(new ModDefinitionAnalyzeEvent(0));

            // Create virtual empty objects from an empty file
            bool searchNeedsRefresh = false;
            foreach (var item in empty)
            {
                var emptyConflicts = indexedDefinitions.GetByFile(item.File);
                if (emptyConflicts.Count() > 0)
                {
                    foreach (var emptyConflict in emptyConflicts.Where(p => p.ValueType != Parser.Common.ValueType.Invalid && !p.ModName.Equals(item.ModName)))
                    {
                        var copy = indexedDefinitions.GetByTypeAndId(emptyConflict.TypeAndId).FirstOrDefault(p => p.ModName.Equals(item.ModName));
                        if (copy == null)
                        {
                            copy = CopyDefinition(emptyConflict);
                            copy.DefinitionSHA = item.DefinitionSHA;
                            copy.Dependencies = item.Dependencies;
                            copy.ModName = item.ModName;
                            copy.Code = item.Code;
                            copy.ContentSHA = item.ContentSHA;
                            copy.UsedParser = item.UsedParser;
                            copy.OriginalCode = item.OriginalCode;
                            copy.CodeSeparator = item.CodeSeparator;
                            copy.CodeTag = item.CodeTag;
                            indexedDefinitions.AddToMap(copy);
                            searchNeedsRefresh = true;
                        }
                        var fileNames = copy.AdditionalFileNames;
                        foreach (var fileName in item.AdditionalFileNames)
                        {
                            fileNames.Add(fileName);
                        }
                        copy.AdditionalFileNames = fileNames;
                    }
                }
                processed++;
                var perc = GetProgressPercentage(total, processed);
                if (perc != previousProgress)
                {
                    messageBus.Publish(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }
            if (searchNeedsRefresh)
            {
                indexedDefinitions.InitSearch();
            }

            foreach (var item in fileKeys)
            {
                var definitions = indexedDefinitions.GetByFile(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, patchStateMode, fileConflictCache);
                processed++;
                var perc = GetProgressPercentage(total, processed);
                if (perc != previousProgress)
                {
                    messageBus.Publish(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            foreach (var item in typeAndIdKeys)
            {
                var definitions = indexedDefinitions.GetByTypeAndId(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, patchStateMode, fileConflictCache);
                processed++;
                var perc = GetProgressPercentage(total, processed);
                if (perc != previousProgress)
                {
                    messageBus.Publish(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            var indexedConflicts = DIResolver.Get<IIndexedDefinitions>();
            indexedConflicts.InitMap(conflicts);

            var overwrittenDefs = new Dictionary<string, IDefinition>();
            foreach (var item in overwritten.GroupBy(p => p.TypeAndId))
            {
                var conflicted = indexedConflicts.GetByTypeAndId(item.First().TypeAndId);
                IEnumerable<IDefinition> definitions;
                IDefinition definition;
                if (conflicted.Count() > 0)
                {
                    definition = EvalDefinitionPriority(conflicted.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition;
                    definitions = conflicted;
                }
                else
                {
                    var valid = new List<IDefinition>();
                    var all = item.Select(p => p);
                    foreach (var def in all)
                    {
                        var hasOverrides = all.Any(p => (p.Dependencies?.Any(p => p.Equals(def.ModName))).GetValueOrDefault());
                        if (!hasOverrides || patchStateMode == PatchStateMode.Advanced)
                        {
                            valid.Add(def);
                        }
                    }
                    definitions = valid;
                    definition = EvalDefinitionPriority(valid.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition;
                }
                if (!overwrittenDefs.ContainsKey(definition.TypeAndId))
                {
                    var newDefinition = CopyDefinition(definition);
                    var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(GameService.GetSelected().Type));
                    newDefinition.File = Path.Combine(definition.ParentDirectory, definition.Id.GenerateValidFileName() + Path.GetExtension(definition.File));
                    var overwrittenFileNames = definition.OverwrittenFileNames;
                    foreach (var file in definitions.SelectMany(p => p.OverwrittenFileNames))
                    {
                        overwrittenFileNames.Add(file);
                    }
                    newDefinition.OverwrittenFileNames = overwrittenFileNames;
                    newDefinition.File = provider.GetFileName(newDefinition);
                    overwrittenDefs.Add(definition.TypeAndId, newDefinition);
                }
                processed++;
                var perc = GetProgressPercentage(total, processed);
                if (perc != previousProgress)
                {
                    messageBus.Publish(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            messageBus.Publish(new ModDefinitionAnalyzeEvent(99));
            var groupedConflicts = conflicts.GroupBy(p => p.TypeAndId);
            var result = GetModelInstance<IConflictResult>();
            result.Mode = patchStateMode;
            var conflictsIndexed = DIResolver.Get<IIndexedDefinitions>();
            conflictsIndexed.InitMap(groupedConflicts.Where(p => p.Count() > 1).SelectMany(p => p), true);
            var orphanedConflictsIndexed = DIResolver.Get<IIndexedDefinitions>();
            orphanedConflictsIndexed.InitMap(groupedConflicts.Where(p => p.Count() == 1).SelectMany(p => p), false);
            result.AllConflicts = indexedDefinitions;
            result.Conflicts = conflictsIndexed;
            result.OrphanConflicts = orphanedConflictsIndexed;
            var resolvedConflicts = DIResolver.Get<IIndexedDefinitions>();
            resolvedConflicts.InitMap(null, true);
            result.ResolvedConflicts = resolvedConflicts;
            var ignoredConflicts = DIResolver.Get<IIndexedDefinitions>();
            ignoredConflicts.InitMap(null, true);
            result.IgnoredConflicts = ignoredConflicts;
            var ruleIgnoredDefinitions = DIResolver.Get<IIndexedDefinitions>();
            ruleIgnoredDefinitions.InitMap(null, true);
            result.RuleIgnoredConflicts = ruleIgnoredDefinitions;
            var overwrittenDefinitions = DIResolver.Get<IIndexedDefinitions>();
            overwrittenDefinitions.InitMap(overwrittenDefs.Select(a => a.Value));
            result.OverwrittenConflicts = overwrittenDefinitions;
            var customConflicts = DIResolver.Get<IIndexedDefinitions>();
            customConflicts.InitMap(null, true);
            result.CustomConflicts = customConflicts;
            messageBus.Publish(new ModDefinitionAnalyzeEvent(100));

            return result;
        }

        /// <summary>
        /// Gets the ignored mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        public virtual IReadOnlyList<string> GetIgnoredMods(IConflictResult conflictResult)
        {
            var mods = new List<string>();
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var lines = ignoredPaths.SplitOnNewLine().Where(p => !p.Trim().StartsWith("#"));
                foreach (var line in lines)
                {
                    var parsed = line.StandardizeDirectorySeparator().Trim().TrimStart(Path.DirectorySeparatorChar);
                    if (parsed.StartsWith(ModNameIgnoreId))
                    {
                        var ignoredModName = parsed.Replace(ModNameIgnoreId, string.Empty);
                        mods.Add(ignoredModName);
                    }
                }
            }
            return mods;
        }

        /// <summary>
        /// Gets the mod objects.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public virtual IIndexedDefinitions GetModObjects(IGame game, IEnumerable<IMod> mods)
        {
            if (game == null || mods == null || mods.Count() == 0)
            {
                return null;
            }
            var definitions = new ConcurrentBag<IDefinition>();

            double processed = 0;
            double total = mods.Count();
            var previousProgress = 0;
            messageBus.Publish(new ModDefinitionLoadEvent(0));

            mods.AsParallel().ForAll((m) =>
            {
                IEnumerable<IDefinition> result = null;
                result = ParseModFiles(game, Reader.Read(m.FullPath, game.GameFolders), m);
                if (result?.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        definitions.Add(item);
                    }
                }
                lock (serviceLock)
                {
                    processed++;
                    var perc = GetProgressPercentage(total, processed);
                    if (perc != previousProgress)
                    {
                        messageBus.Publish(new ModDefinitionLoadEvent(perc));
                        previousProgress = perc;
                    }
                }
            });

            messageBus.Publish(new ModDefinitionLoadEvent(99));
            var indexed = DIResolver.Get<IIndexedDefinitions>();
            indexed.InitMap(definitions);
            indexed.InitSearch();
            messageBus.Publish(new ModDefinitionLoadEvent(100));
            return indexed;
        }

        /// <summary>
        /// get patch state mode as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;PatchStateMode&gt;.</returns>
        public virtual async Task<PatchStateMode> GetPatchStateModeAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                }, false);
                if (state != null)
                {
                    return MapPatchStateMode(state.Mode);
                }
            }
            return PatchStateMode.None;
        }

        /// <summary>
        /// Ignores the mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> IgnoreModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName)
        {
            return ExportModPatchDefinitionAsync(conflictResult, definition, collectionName, ExportType.Ignored);
        }

        /// <summary>
        /// Invalidates the state of the patch mod.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool InvalidatePatchModState(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null)
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                var cachePrefix = $"CollectionPatchState-{game.Type}";
                Cache.Invalidate(cachePrefix, patchName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod].
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod]; otherwise, <c>false</c>.</returns>
        public virtual bool IsPatchMod(IMod mod)
        {
            return IsPatchModInternal(mod);
        }

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod name].
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod name]; otherwise, <c>false</c>.</returns>
        public virtual bool IsPatchMod(string modName)
        {
            return IsPatchModInternal(modName);
        }

        /// <summary>
        /// Loads the definition contents asynchronous.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public virtual Task<string> LoadDefinitionContentsAsync(IDefinition definition, string collectionName)
        {
            var game = GameService.GetSelected();
            if (definition == null || string.IsNullOrWhiteSpace(collectionName) || game == null)
            {
                return Task.FromResult(string.Empty);
            }
            var patchName = GenerateCollectionPatchName(collectionName);
            return modPatchExporter.LoadDefinitionContentsAsync(new ModPatchExporterParameters()
            {
                RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                PatchName = patchName
            }, definition.File);
        }

        /// <summary>
        /// patch mod needs update as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> PatchModNeedsUpdateAsync(string collectionName)
        {
            async Task<bool> evalState(IGame game, string cachePrefix, string patchName)
            {
                Cache.Set(cachePrefix, patchName, new PatchCollectionState() { CheckInProgress = true });
                var mods = GetCollectionMods();
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                }, false);
                if (state == null)
                {
                    Cache.Set(cachePrefix, patchName, new PatchCollectionState() { NeedsUpdate = false, CheckInProgress = false });
                    return false;
                }
                foreach (var groupedMods in state.Conflicts.GroupBy(p => p.ModName))
                {
                    foreach (var item in groupedMods.GroupBy(p => p.File))
                    {
                        var definition = item.FirstOrDefault();
                        var mod = mods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                        if (mod == null)
                        {
                            // Mod no longer in collection, needs refresh break further checks...
                            Cache.Set(cachePrefix, patchName, new PatchCollectionState() { NeedsUpdate = true, CheckInProgress = false });
                            return true;
                        }
                        else
                        {
                            var info = Reader.GetFileInfo(mod.FullPath, definition.File);
                            if (info == null || !info.ContentSHA.Equals(definition.ContentSHA))
                            {
                                // File no longer in collection or content does not match, break further checks
                                Cache.Set(cachePrefix, patchName, new PatchCollectionState() { NeedsUpdate = true, CheckInProgress = false });
                                return true;
                            }
                        }
                    }
                }
                Cache.Set(cachePrefix, patchName, new PatchCollectionState() { NeedsUpdate = false, CheckInProgress = false });
                return false;
            }
            var game = GameService.GetSelected();
            if (game != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var activeCollection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
                if (activeCollection == null)
                {
                    return false;
                }
                var patchName = GenerateCollectionPatchName(collectionName);
                var cachePrefix = $"CollectionPatchState-{game.Type}";
                var result = Cache.Get<PatchCollectionState>(cachePrefix, patchName);
                if (result != null)
                {
                    while (result.CheckInProgress)
                    {
                        // Since another check is queued, wait and periodically check if the task is done...
                        await Task.Delay(10);
                        result = Cache.Get<PatchCollectionState>(cachePrefix, patchName);
                        if (result == null)
                        {
                            await evalState(game, cachePrefix, patchName);
                            result = Cache.Get<PatchCollectionState>(cachePrefix, patchName);
                        }
                    }
                    return result.NeedsUpdate;
                }
                else
                {
                    return await evalState(game, cachePrefix, patchName);
                }
            }
            return false;
        }

        /// <summary>
        /// Renames the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="newCollectionName">New name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> RenamePatchCollectionAsync(string collectionName, string newCollectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return Task.FromResult(false);
            }
            var oldPatchName = GenerateCollectionPatchName(collectionName);
            var newPathName = GenerateCollectionPatchName(newCollectionName);
            return modPatchExporter.RenamePatchModAsync(new ModPatchExporterParameters()
            {
                RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                ModPath = oldPatchName,
                PatchName = newPathName
            });
        }

        /// <summary>
        /// Resets the custom conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ResetCustomConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName)
        {
            return UnResolveConflictAsync(conflictResult, typeAndId, GenerateCollectionPatchName(collectionName), ExportType.Custom);
        }

        /// <summary>
        /// Resets the ignored conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ResetIgnoredConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName)
        {
            return UnResolveConflictAsync(conflictResult, typeAndId, GenerateCollectionPatchName(collectionName), ExportType.Ignored);
        }

        /// <summary>
        /// Resets the patch state cache.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool ResetPatchStateCache()
        {
            modPatchExporter.ResetCache();
            return true;
        }

        /// <summary>
        /// Resets the resolved conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ResetResolvedConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName)
        {
            return UnResolveConflictAsync(conflictResult, typeAndId, GenerateCollectionPatchName(collectionName), ExportType.Resolved);
        }

        /// <summary>
        /// Resolves the full definition path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        public virtual string ResolveFullDefinitionPath(IDefinition definition)
        {
            if (definition == null || GameService.GetSelected() == null)
            {
                return string.Empty;
            }
            var mods = GetCollectionMods();
            if (mods?.Count() > 0)
            {
                var mod = mods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                if (mod != null && !string.IsNullOrWhiteSpace(mod.FullPath))
                {
                    if (mod.FullPath.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        return mod.FullPath;
                    }
                    return Path.Combine(mod.FullPath, definition.File);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Saves the ignored paths asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> SaveIgnoredPathsAsync(IConflictResult conflictResult, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return Task.FromResult(false);
            }
            EvalModIgnoreDefinitions(conflictResult);
            var patchName = GenerateCollectionPatchName(collectionName);
            return modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
            {
                Mode = MapPatchStateMode(conflictResult.Mode),
                IgnoreConflictPaths = conflictResult.IgnoredPaths,
                Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                PatchName = patchName
            });
        }

        /// <summary>
        /// synchronize patch state as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        public virtual async Task<IConflictResult> SyncPatchStateAsync(IConflictResult conflictResult, string collectionName)
        {
            var game = GameService.GetSelected();
            var previousProgress = 0;
            async Task syncPatchFiles(IConflictResult conflicts, IEnumerable<string> patchFiles, string patchName, int total, int processed, int maxProgress)
            {
                foreach (var file in patchFiles)
                {
                    if (conflicts.CustomConflicts.ExistsByFile(file) &&
                        conflicts.OrphanConflicts.ExistsByFile(file) &&
                        conflicts.OverwrittenConflicts.ExistsByFile(file) &&
                        conflicts.ResolvedConflicts.ExistsByFile(file))
                    {
                        await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                        {
                            RootDirectory = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, patchName),
                            Path = file
                        });
                    }
                    processed++;
                    var perc = GetProgressPercentage(total, processed, maxProgress);
                    if (previousProgress != perc)
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                        previousProgress = perc;
                    }
                }
            }
            if (game != null && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(0));
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                });
                var patchFiles = modPatchExporter.GetPatchFiles(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                });
                var total = patchFiles.Count();
                if (state != null)
                {
                    var perc = 0;
                    var resolvedConflicts = new List<IDefinition>(state.ResolvedConflicts);
                    var ignoredConflicts = new List<IDefinition>();
                    total += state.Conflicts.Count() + state.OrphanConflicts.Count() + state.OverwrittenConflicts.Count() + 3; // as in 3 additional steps performed
                    int processed = 0;
                    foreach (var item in state.OrphanConflicts.GroupBy(p => p.TypeAndId))
                    {
                        var files = ProcessPatchStateFiles(state, item, ref processed);
                        var matchedConflicts = FindPatchStateMatchedConflicts(conflictResult.OrphanConflicts, state, ignoredConflicts, item);
                        await SyncPatchStateAsync(game.UserDirectory, patchName, resolvedConflicts, item, files, matchedConflicts);
                        perc = GetProgressPercentage(total, processed, 99);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }
                    foreach (var item in state.Conflicts.GroupBy(p => p.TypeAndId))
                    {
                        var files = ProcessPatchStateFiles(state, item, ref processed);
                        var matchedConflicts = FindPatchStateMatchedConflicts(conflictResult.Conflicts, state, ignoredConflicts, item);
                        await SyncPatchStateAsync(game.UserDirectory, patchName, resolvedConflicts, item, files, matchedConflicts);
                        perc = GetProgressPercentage(total, processed, 99);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }
                    foreach (var item in state.OverwrittenConflicts.GroupBy(p => p.TypeAndId))
                    {
                        processed += item.Count();
                        var files = new List<string>();
                        files.AddRange(item.SelectMany(p => p.OverwrittenFileNames));
                        if (state.ResolvedConflicts != null)
                        {
                            var resolved = state.ResolvedConflicts.Where(p => p.TypeAndId.Equals(item.First().TypeAndId));
                            if (resolved.Count() > 0)
                            {
                                var fileNames = resolved.Select(p => p.File);
                                files.RemoveAll(p => fileNames.Any(a => a.Equals(p, StringComparison.OrdinalIgnoreCase)));
                            }
                        }
                        var matchedConflicts = conflictResult.OverwrittenConflicts.GetByTypeAndId(item.First().TypeAndId);
                        await SyncPatchStatesAsync(matchedConflicts, item, patchName, game.UserDirectory, files.ToArray());
                        perc = GetProgressPercentage(total, processed, 99);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }

                    if (conflictResult.OrphanConflicts.GetAll().Count() > 0)
                    {
                        processed++;
                        perc = GetProgressPercentage(total, processed, 99);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                        if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OrphanConflicts = PopulateModPath(conflictResult.OrphanConflicts.GetAll(), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                            PatchName = patchName
                        }))
                        {
                            foreach (var item in conflictResult.OrphanConflicts.GetAll())
                            {
                                var existing = conflictResult.ResolvedConflicts.GetByTypeAndId(item.TypeAndId);
                                if (existing.Count() == 0)
                                {
                                    conflictResult.ResolvedConflicts.AddToMap(item, true);
                                }
                            }
                        }
                    }

                    var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                    resolvedIndex.InitMap(resolvedConflicts, true);

                    if (conflictResult.OverwrittenConflicts.GetAll().Count() > 0)
                    {
                        processed++;
                        perc = GetProgressPercentage(total, processed, 99);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                        await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OverwrittenConflicts = PopulateModPath(conflictResult.OverwrittenConflicts.GetAll().Where(p => resolvedIndex.GetByTypeAndId(p.TypeAndId).Count() == 0), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                            PatchName = patchName
                        });
                    }

                    processed++;
                    perc = GetProgressPercentage(total, processed, 99);
                    if (previousProgress != perc)
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                        previousProgress = perc;
                    }
                    var conflicts = GetModelInstance<IConflictResult>();
                    conflicts.AllConflicts = conflictResult.AllConflicts;
                    conflicts.Conflicts = conflictResult.Conflicts;
                    conflicts.OrphanConflicts = conflictResult.OrphanConflicts;
                    conflicts.ResolvedConflicts = resolvedIndex;
                    var ignoredIndex = DIResolver.Get<IIndexedDefinitions>();
                    ignoredIndex.InitMap(ignoredConflicts, true);
                    conflicts.IgnoredConflicts = ignoredIndex;
                    conflicts.IgnoredPaths = state.IgnoreConflictPaths ?? string.Empty;
                    conflicts.OverwrittenConflicts = conflictResult.OverwrittenConflicts;
                    var customConflicts = DIResolver.Get<IIndexedDefinitions>();
                    customConflicts.InitMap(state.CustomConflicts, true);
                    conflicts.CustomConflicts = customConflicts;
                    conflicts.Mode = conflictResult.Mode;
                    EvalModIgnoreDefinitions(conflicts);
                    await syncPatchFiles(conflicts, patchFiles, patchName, total, processed, 100);

                    await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        Mode = MapPatchStateMode(conflicts.Mode),
                        IgnoreConflictPaths = conflicts.IgnoredPaths,
                        Conflicts = GetDefinitionOrDefault(conflicts.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflicts.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflicts.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflicts.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflicts.OverwrittenConflicts),
                        CustomConflicts = GetDefinitionOrDefault(conflicts.CustomConflicts),
                        RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                        PatchName = patchName
                    });
                    await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(100));

                    return conflicts;
                }
                else
                {
                    var processed = 0;
                    await syncPatchFiles(conflictResult, patchFiles, patchName, total, processed, 96);

                    var exportedConflicts = false;
                    if (conflictResult.OrphanConflicts.GetAll().Count() > 0)
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(96));
                        if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OrphanConflicts = PopulateModPath(conflictResult.OrphanConflicts.GetAll(), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                            PatchName = patchName
                        }))
                        {
                            foreach (var item in conflictResult.OrphanConflicts.GetAll())
                            {
                                var existing = conflictResult.ResolvedConflicts.GetByTypeAndId(item.TypeAndId);
                                if (existing.Count() == 0)
                                {
                                    conflictResult.ResolvedConflicts.AddToMap(item, true);
                                }
                            }
                            exportedConflicts = true;
                        }
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(97));
                    }

                    if (conflictResult.OverwrittenConflicts.GetAll().Count() > 0)
                    {
                        exportedConflicts = true;
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(98));
                        await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OverwrittenConflicts = PopulateModPath(conflictResult.OverwrittenConflicts.GetAll().Where(p => conflictResult.ResolvedConflicts.GetByTypeAndId(p.TypeAndId).Count() == 0), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                            PatchName = patchName
                        });
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(99));
                    }

                    await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(100));
                    if (exportedConflicts)
                    {
                        await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                        {
                            Mode = MapPatchStateMode(conflictResult.Mode),
                            IgnoreConflictPaths = conflictResult.IgnoredPaths,
                            Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                            OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                            ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                            IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                            OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                            CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                            RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                            PatchName = patchName
                        });
                    }
                }
            };
            return null;
        }

        /// <summary>
        /// Evals the definitions.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="conflicts">The conflicts.</param>
        /// <param name="definitions">The definitions.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="patchStateMode">The patch state mode.</param>
        /// <param name="fileConflictCache">The file conflict cache.</param>
        protected virtual void EvalDefinitions(IIndexedDefinitions indexedDefinitions, HashSet<IDefinition> conflicts, IEnumerable<IDefinition> definitions, IList<string> modOrder, PatchStateMode patchStateMode, Dictionary<string, bool> fileConflictCache)
        {
            var validDefinitions = new HashSet<IDefinition>();
            foreach (var item in definitions.Where(p => IsValidDefinitionType(p)))
            {
                validDefinitions.Add(item);
            }
            var processed = new HashSet<IDefinition>();
            foreach (var def in validDefinitions)
            {
                if (processed.Contains(def) || conflicts.Contains(def))
                {
                    continue;
                }
                var allConflicts = indexedDefinitions.GetByTypeAndId(def.Type, def.Id).Where(p => IsValidDefinitionType(p));
                foreach (var conflict in allConflicts)
                {
                    processed.Add(conflict);
                }
                if (allConflicts.Count() > 1)
                {
                    if (!allConflicts.All(p => p.DefinitionSHA.Equals(def.DefinitionSHA)))
                    {
                        var validConflicts = new HashSet<IDefinition>();
                        foreach (var conflict in allConflicts)
                        {
                            if (conflicts.Contains(conflict) || validConflicts.Contains(conflict))
                            {
                                continue;
                            }
                            var hasOverrides = allConflicts.Any(p => (p.Dependencies?.Any(p => p.Equals(conflict.ModName))).GetValueOrDefault());
                            if (hasOverrides && patchStateMode == PatchStateMode.Default)
                            {
                                continue;
                            }
                            validConflicts.Add(conflict);
                        }

                        var validConflictsGroup = validConflicts.GroupBy(p => p.DefinitionSHA);
                        if (validConflictsGroup.Count() > 1)
                        {
                            var filteredConflicts = validConflictsGroup.Select(p => EvalDefinitionPriority(p.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition);
                            foreach (var item in filteredConflicts)
                            {
                                if (!conflicts.Contains(item) && IsValidDefinitionType(item))
                                {
                                    conflicts.Add(item);
                                }
                            }
                        }
                    }
                }
                else if (allConflicts.Count() == 1)
                {
                    if (allConflicts.FirstOrDefault().ValueType == Parser.Common.ValueType.Binary)
                    {
                        fileConflictCache.TryAdd(def.FileCI, false);
                    }
                    else
                    {
                        if (fileConflictCache.TryGetValue(def.FileCI, out var result))
                        {
                            if (result)
                            {
                                if (!conflicts.Contains(def) && IsValidDefinitionType(def))
                                {
                                    conflicts.Add(def);
                                }
                            }
                        }
                        else
                        {
                            var fileDefs = indexedDefinitions.GetByFile(def.FileCI);
                            if (fileDefs.GroupBy(p => p.ModName).Count() > 1)
                            {
                                var hasOverrides = def.Dependencies?.Any(p => fileDefs.Any(s => s.ModName.Equals(p)));
                                if (hasOverrides.GetValueOrDefault() && patchStateMode == PatchStateMode.Default)
                                {
                                    fileConflictCache.TryAdd(def.FileCI, false);
                                }
                                else
                                {
                                    fileConflictCache.TryAdd(def.FileCI, true);
                                    if (!conflicts.Contains(def) && IsValidDefinitionType(def))
                                    {
                                        conflicts.Add(def);
                                    }
                                }
                            }
                            else
                            {
                                fileConflictCache.TryAdd(def.FileCI, false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evals the mod ignore definitions.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        protected virtual void EvalModIgnoreDefinitions(IConflictResult conflictResult)
        {
            var ruleIgnoredDefinitions = DIResolver.Get<IIndexedDefinitions>();
            ruleIgnoredDefinitions.InitMap(null, true);
            if (!string.IsNullOrEmpty(conflictResult.IgnoredPaths))
            {
                var allowedMods = GetCollectionMods().Select(p => p.Name).ToList();
                var ignoreRules = new List<string>();
                var includeRules = new List<string>();
                var lines = conflictResult.IgnoredPaths.SplitOnNewLine().Where(p => !p.Trim().StartsWith("#"));
                foreach (var line in lines)
                {
                    var parsed = line.StandardizeDirectorySeparator().Trim().TrimStart(Path.DirectorySeparatorChar);
                    if (parsed.StartsWith(ModNameIgnoreId))
                    {
                        var ignoredModName = parsed.Replace(ModNameIgnoreId, string.Empty);
                        allowedMods.Remove(ignoredModName);
                    }
                    else
                    {
                        if (!parsed.StartsWith("!"))
                        {
                            ignoreRules.Add(parsed);
                        }
                        else
                        {
                            includeRules.Add(parsed.TrimStart('!'));
                        }
                    }
                }
                var alreadyIgnored = new HashSet<string>();
                foreach (var topConflict in conflictResult.Conflicts.GetHierarchicalDefinitions())
                {
                    if (topConflict.Mods.Any(x => allowedMods.Contains(x)))
                    {
                        foreach (var item in topConflict.Children)
                        {
                            if (!item.Mods.Any(x => allowedMods.Contains(x)))
                            {
                                if (!alreadyIgnored.Contains(item.Key))
                                {
                                    alreadyIgnored.Add(item.Key);
                                    ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
                                }
                            }
                        }
                        var name = topConflict.Name;
                        if (!name.EndsWith(Path.DirectorySeparatorChar))
                        {
                            name = $"{name}{Path.DirectorySeparatorChar}";
                        }
                        if (ignoreRules.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase)) && !includeRules.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                        {
                            foreach (var item in topConflict.Children)
                            {
                                if (!alreadyIgnored.Contains(item.Key))
                                {
                                    alreadyIgnored.Add(item.Key);
                                    ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in topConflict.Children)
                        {
                            if (!alreadyIgnored.Contains(item.Key))
                            {
                                alreadyIgnored.Add(item.Key);
                                ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
                            }
                        }
                    }
                }
            }
            conflictResult.RuleIgnoredConflicts = ruleIgnoredDefinitions;
        }

        /// <summary>
        /// export mod patch definition as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="exportType">Type of the export.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> ExportModPatchDefinitionAsync(IConflictResult conflictResult, IDefinition definition, string collectionName, ExportType exportType)
        {
            var game = GameService.GetSelected();
            if (definition != null && game != null && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                var allMods = GetInstalledModsInternal(game, false).ToList();
                IMod mod;
                if (!allMods.Any(p => p.Name.Equals(patchName)))
                {
                    mod = GeneratePatchModDescriptor(allMods, game, patchName);
                    await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = game.UserDirectory,
                        Path = Shared.Constants.ModDirectory
                    });
                    await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = game.UserDirectory,
                        Path = Path.Combine(Shared.Constants.ModDirectory, patchName)
                    });
                    await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = mod,
                        RootDirectory = game.UserDirectory,
                        Path = mod.DescriptorFile
                    }, IsPatchMod(mod));
                    allMods.Add(mod);
                    Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));
                }
                else
                {
                    mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
                }
                var definitionMod = allMods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                if (definitionMod != null || IsPatchMod(definitionMod))
                {
                    var exportPatches = new HashSet<IDefinition>();
                    switch (exportType)
                    {
                        case ExportType.Ignored:
                            conflictResult.IgnoredConflicts.AddToMap(definition);
                            break;

                        case ExportType.Custom:
                            conflictResult.CustomConflicts.AddToMap(definition);
                            exportPatches.Add(definition);
                            break;

                        default:
                            conflictResult.ResolvedConflicts.AddToMap(definition);
                            exportPatches.Add(definition);
                            break;
                    }

                    await ModWriter.ApplyModsAsync(new ModWriterParameters()
                    {
                        AppendOnly = true,
                        TopPriorityMods = new List<IMod>() { mod },
                        RootDirectory = game.UserDirectory
                    });

                    bool exportResult = false;
                    if (exportPatches.Count > 0)
                    {
                        exportResult = await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            Definitions = PopulateModPath(exportPatches, GetCollectionMods(allMods)),
                            RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                            PatchName = patchName
                        });
                    }

                    var stateResult = await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        Mode = MapPatchStateMode(conflictResult.Mode),
                        IgnoreConflictPaths = conflictResult.IgnoredPaths,
                        Definitions = exportPatches,
                        Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                        CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                        RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                        PatchName = patchName
                    });
                    return exportPatches.Count > 0 ? exportResult && stateResult : stateResult;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the patch state matched conflicts.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="state">The state.</param>
        /// <param name="ignoredConflicts">The ignored conflicts.</param>
        /// <param name="item">The item.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> FindPatchStateMatchedConflicts(IIndexedDefinitions indexedDefinitions, IPatchState state, List<IDefinition> ignoredConflicts, IGrouping<string, IDefinition> item)
        {
            var matchedConflicts = indexedDefinitions.GetByTypeAndId(item.First().TypeAndId);
            if (state.IgnoredConflicts != null)
            {
                var ignored = state.IgnoredConflicts.Where(p => p.TypeAndId.Equals(item.First().TypeAndId));
                if (ignored.Count() > 0 && !IsCachedDefinitionDifferent(matchedConflicts, item))
                {
                    ignoredConflicts.AddRange(ignored);
                }
            }
            return matchedConflicts;
        }

        /// <summary>
        /// Gets the definition or default.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IList&lt;IDefinition&gt;.</returns>
        protected virtual IList<IDefinition> GetDefinitionOrDefault(IIndexedDefinitions definitions)
        {
            return definitions != null && definitions.GetAll() != null ? definitions.GetAll().ToList() : new List<IDefinition>();
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Int32.</returns>
        protected virtual int GetProgressPercentage(double total, double processed, int maxPerc = 98)
        {
            var perc = Convert.ToInt32((processed / total * 100) - 2);
            if (perc < 1)
            {
                perc = 1;
            }
            else if (perc > maxPerc)
            {
                perc = maxPerc;
            }
            return perc;
        }

        /// <summary>
        /// Determines whether [is cached definition different] [the specified current conflicts].
        /// </summary>
        /// <param name="currentConflicts">The current conflicts.</param>
        /// <param name="cachedConflicts">The cached conflicts.</param>
        /// <returns><c>true</c> if [is cached definition different] [the specified current conflicts]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsCachedDefinitionDifferent(IEnumerable<IDefinition> currentConflicts, IEnumerable<IDefinition> cachedConflicts)
        {
            if (currentConflicts.Count() != cachedConflicts.Count())
            {
                return true;
            }
            var cachedDiffs = cachedConflicts.Where(p => currentConflicts.Any(a => a.ModName.Equals(p.ModName) && a.FileCI.Equals(p.FileCI) && a.DefinitionSHA.Equals(p.DefinitionSHA)));
            return cachedDiffs.Count() != cachedConflicts.Count();
        }

        /// <summary>
        /// Determines whether [is valid definition type] [the specified definition].
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if [is valid definition type] [the specified definition]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValidDefinitionType(IDefinition definition)
        {
            return definition != null && definition.ValueType != Parser.Common.ValueType.Variable &&
                definition.ValueType != Parser.Common.ValueType.Namespace &&
                definition.ValueType != Parser.Common.ValueType.Invalid &&
                definition.ValueType != Parser.Common.ValueType.EmptyFile;
        }

        /// <summary>
        /// Maps the patch state mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>Models.Common.PatchStateMode.</returns>
        protected virtual PatchStateMode MapPatchStateMode(IO.Common.PatchStateMode mode)
        {
            return mode switch
            {
                IO.Common.PatchStateMode.Default => PatchStateMode.Default,
                IO.Common.PatchStateMode.Advanced => PatchStateMode.Advanced,
                _ => PatchStateMode.None,
            };
        }

        /// <summary>
        /// Maps the patch state mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>IO.Common.PatchStateMode.</returns>
        protected virtual IO.Common.PatchStateMode MapPatchStateMode(PatchStateMode mode)
        {
            return mode switch
            {
                PatchStateMode.Default => IO.Common.PatchStateMode.Default,
                PatchStateMode.Advanced => IO.Common.PatchStateMode.Advanced,
                _ => IO.Common.PatchStateMode.None,
            };
        }

        /// <summary>
        /// Merges the definitions.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        protected virtual void MergeDefinitions(IEnumerable<IDefinition> definitions)
        {
            static void appendLine(StringBuilder sb, IEnumerable<string> lines)
            {
                if (lines?.Count() > 0)
                {
                    sb.AppendLine(string.Join(Environment.NewLine, lines));
                }
            }
            static string mergeCode(string codeTag, string separator, IEnumerable<string> lines)
            {
                if (Shared.Constants.CodeSeparators.ClosingSeparators.Map.ContainsKey(separator))
                {
                    var closingTag = Shared.Constants.CodeSeparators.ClosingSeparators.Map[separator];
                    var sb = new StringBuilder();
                    sb.AppendLine($"{codeTag} = {separator}");
                    foreach (var item in lines)
                    {
                        var splitLines = item.SplitOnNewLine();
                        foreach (var split in splitLines)
                        {
                            sb.AppendLine($"{new string(' ', 4)}{split}");
                        }
                    }
                    sb.Append(closingTag);
                    return sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"{codeTag}{separator}");
                    foreach (var item in lines)
                    {
                        var splitLines = item.SplitOnNewLine();
                        foreach (var split in splitLines)
                        {
                            sb.AppendLine($"{new string(' ', 4)}{split}");
                        }
                    }
                    return sb.ToString();
                }
            }

            if (definitions?.Count() > 0)
            {
                var otherDefinitions = definitions.Where(p => IsValidDefinitionType(p));
                var variableDefinitions = definitions.Where(p => !IsValidDefinitionType(p));
                if (variableDefinitions.Count() > 0)
                {
                    foreach (var definition in otherDefinitions)
                    {
                        var namespaces = variableDefinitions.Where(p => p.ValueType == Parser.Common.ValueType.Namespace);
                        var variables = variableDefinitions.Where(p => definition.Code.ReplaceTabs().Replace(" ", string.Empty).Contains($"={p.Id}"));
                        if (string.IsNullOrWhiteSpace(definition.CodeTag))
                        {
                            StringBuilder sb = new StringBuilder();
                            appendLine(sb, namespaces.Select(p => p.Code));
                            appendLine(sb, variables.Select(p => p.Code));
                            appendLine(sb, new List<string> { definition.Code });
                            definition.Code = sb.ToString();
                        }
                        else
                        {
                            definition.Code = mergeCode(definition.CodeTag, definition.CodeSeparator, namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode)).Concat(new List<string>() { definition.OriginalCode }));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the mod files.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="fileInfos">The file infos.</param>
        /// <param name="modObject">The mod object.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseModFiles(IGame game, IEnumerable<IFileInfo> fileInfos, IModObject modObject)
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
                    File = fileInfo.FileName,
                    GameType = game.Type,
                    Lines = fileInfo.Content,
                    ModDependencies = modObject.Dependencies,
                    ModName = modObject.Name
                });
                MergeDefinitions(fileDefs);
                definitions.AddRange(fileDefs);
            }
            return definitions;
        }

        /// <summary>
        /// Processes the patch state files.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="item">The item.</param>
        /// <param name="processed">The processed.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        protected virtual IList<string> ProcessPatchStateFiles(IPatchState state, IGrouping<string, IDefinition> item, ref int processed)
        {
            processed += item.Count();
            var files = new List<string>();
            if (state.ResolvedConflicts != null)
            {
                var resolved = state.ResolvedConflicts.Where(p => p.TypeAndId.Equals(item.First().TypeAndId));
                if (resolved.Count() > 0)
                {
                    files.AddRange(resolved.Select(p => p.File));
                }
            }

            return files;
        }

        /// <summary>
        /// synchronize patch state as an asynchronous operation.
        /// </summary>
        /// <param name="userDirectory">The user directory.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="resolvedConflicts">The resolved conflicts.</param>
        /// <param name="item">The item.</param>
        /// <param name="files">The files.</param>
        /// <param name="matchedConflicts">The matched conflicts.</param>
        protected virtual async Task SyncPatchStateAsync(string userDirectory, string patchName, List<IDefinition> resolvedConflicts, IGrouping<string, IDefinition> item, IList<string> files, IEnumerable<IDefinition> matchedConflicts)
        {
            var synced = await SyncPatchStatesAsync(matchedConflicts, item, patchName, userDirectory, files.ToArray());
            if (synced)
            {
                foreach (var diff in item)
                {
                    var existingConflict = resolvedConflicts.FirstOrDefault(p => p.TypeAndId.Equals(diff.TypeAndId));
                    if (existingConflict != null)
                    {
                        resolvedConflicts.Remove(existingConflict);
                    }
                }
            }
        }

        /// <summary>
        /// synchronize patch states as an asynchronous operation.
        /// </summary>
        /// <param name="currentConflicts">The current conflicts.</param>
        /// <param name="cachedConflicts">The cached conflicts.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="userDirectory">The user directory.</param>
        /// <param name="files">The files.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> SyncPatchStatesAsync(IEnumerable<IDefinition> currentConflicts, IEnumerable<IDefinition> cachedConflicts, string patchName, string userDirectory, params string[] files)
        {
            if (IsCachedDefinitionDifferent(currentConflicts, cachedConflicts))
            {
                foreach (var file in files)
                {
                    await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = Path.Combine(userDirectory, Shared.Constants.ModDirectory, patchName),
                        Path = file
                    });
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// un resolve conflict as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="exportType">Type of the export.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> UnResolveConflictAsync(IConflictResult conflictResult, string typeAndId, string patchName, ExportType exportType)
        {
            var game = GameService.GetSelected();
            if (conflictResult != null && game != null)
            {
                bool purgeFiles = true;
                IIndexedDefinitions indexed;
                switch (exportType)
                {
                    case ExportType.Ignored:
                        indexed = conflictResult.IgnoredConflicts;
                        purgeFiles = false;
                        break;

                    case ExportType.Custom:
                        indexed = conflictResult.CustomConflicts;
                        break;

                    default:
                        indexed = conflictResult.ResolvedConflicts;
                        break;
                }

                var result = indexed.GetByTypeAndId(typeAndId);
                if (result.Count() > 0)
                {
                    IEnumerable<IMod> collectionMods = null;
                    foreach (var item in result)
                    {
                        indexed.Remove(item);
                        if (purgeFiles)
                        {
                            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                            {
                                RootDirectory = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, patchName),
                                Path = item.File
                            });
                            if (item.ValueType == Parser.Common.ValueType.OverwrittenObject)
                            {
                                if (collectionMods == null)
                                {
                                    collectionMods = GetCollectionMods();
                                }
                                var overwritten = conflictResult.OverwrittenConflicts.GetByTypeAndId(typeAndId);
                                if (overwritten.Count() > 0)
                                {
                                    await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                                    {
                                        Game = game.Type,
                                        OverwrittenConflicts = PopulateModPath(overwritten.Where(p => conflictResult.ResolvedConflicts.GetByTypeAndId(p.TypeAndId).Count() == 0), collectionMods),
                                        RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                                        PatchName = patchName
                                    });
                                }
                            }
                        }
                    }
                    var allMods = GetInstalledModsInternal(game, false).ToList();
                    IMod mod;
                    if (!allMods.Any(p => p.Name.Equals(patchName)))
                    {
                        mod = GeneratePatchModDescriptor(allMods, game, patchName);
                        await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                        {
                            RootDirectory = game.UserDirectory,
                            Path = Shared.Constants.ModDirectory
                        });
                        await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                        {
                            RootDirectory = game.UserDirectory,
                            Path = Path.Combine(Shared.Constants.ModDirectory, patchName)
                        });
                        await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                        {
                            Mod = mod,
                            RootDirectory = game.UserDirectory,
                            Path = mod.DescriptorFile
                        }, IsPatchMod(mod));
                        allMods.Add(mod);
                        Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));
                    }
                    else
                    {
                        mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
                    }
                    await ModWriter.ApplyModsAsync(new ModWriterParameters()
                    {
                        AppendOnly = true,
                        TopPriorityMods = new List<IMod>() { mod },
                        RootDirectory = game.UserDirectory
                    });
                    await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        Mode = MapPatchStateMode(conflictResult.Mode),
                        IgnoreConflictPaths = conflictResult.IgnoredPaths,
                        Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                        CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                        RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                        PatchName = patchName
                    });
                    return true;
                }
            }
            return false;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class PatchCollectionState.
        /// </summary>
        private class PatchCollectionState
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether [check in progress].
            /// </summary>
            /// <value><c>true</c> if [check in progress]; otherwise, <c>false</c>.</value>
            public bool CheckInProgress { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [needs update].
            /// </summary>
            /// <value><c>true</c> if [needs update]; otherwise, <c>false</c>.</value>
            public bool NeedsUpdate { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
