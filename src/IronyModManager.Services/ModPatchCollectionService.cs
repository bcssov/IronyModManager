// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 06-07-2020
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
using IronyModManager.Shared;
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
        /// The service lock
        /// </summary>
        private static readonly object serviceLock = new { };

        /// <summary>
        /// The definition information providers
        /// </summary>
        private readonly IEnumerable<IDefinitionInfoProvider> definitionInfoProviders;

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
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModPatchCollectionService(IParserManager parserManager, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IModPatchExporter modPatchExporter, IReader reader, IModWriter modWriter, IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.parserManager = parserManager;
            this.definitionInfoProviders = definitionInfoProviders;
            this.modPatchExporter = modPatchExporter;
            this.modPatchExporter.WriteOperationState += (args) =>
            {
                OnShutdownState(args);
            };
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [mod definition analyze].
        /// </summary>
        public event ModDefinitionAnalyzeDelegate ModDefinitionAnalyze;

        /// <summary>
        /// Occurs when [mod analyze].
        /// </summary>
        public event ModDefinitionLoadDelegate ModDefinitionLoad;

        /// <summary>
        /// Occurs when [mod definition patch load].
        /// </summary>
        public event ModDefinitionPatchLoadDelegate ModDefinitionPatchLoad;

        #endregion Events

        #region Methods

        /// <summary>
        /// apply mod patch as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ApplyModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName)
        {
            return ExportModPatchDefinitionAsync(conflictResult, definition, collectionName);
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
                RootDirectory = GetPatchDirectory(game, patchName)
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
                RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
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
                    RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
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
        public IPriorityDefinitionResult EvalDefinitionPriority(IEnumerable<IDefinition> definitions)
        {
            var game = GameService.GetSelected();
            var result = GetModelInstance<IPriorityDefinitionResult>();
            if (game != null && definitions?.Count() > 1)
            {
                var definitionEvals = new List<DefinitionEval>();
                var provider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
                bool isFios = false;
                if (provider != null)
                {
                    bool overrideSkipped = false;
                    isFios = provider.DefinitionUsesFIOSRules(definitions.First());
                    foreach (var item in definitions)
                    {
                        var hasOverrides = definitions.Any(p => (p.Dependencies?.Any(p => p.Equals(item.ModName))).GetValueOrDefault());
                        if (hasOverrides)
                        {
                            overrideSkipped = true;
                            continue;
                        }
                        if (isFios)
                        {
                            definitionEvals.Add(new DefinitionEval()
                            {
                                Definition = item,
                                FileName = item.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).First()
                            });
                        }
                        else
                        {
                            definitionEvals.Add(new DefinitionEval()
                            {
                                Definition = item,
                                FileName = item.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).Last()
                            });
                        }
                    }
                    var uniqueDefinitions = definitionEvals.GroupBy(p => p.Definition.ModName).Select(p => p.First());
                    if (uniqueDefinitions.Count() == 1 && overrideSkipped)
                    {
                        result.Definition = definitionEvals.First().Definition;
                        result.PriorityType = DefinitionPriorityType.ModOverride;
                    }
                    else if (uniqueDefinitions.Count() > 1)
                    {
                        // Has same filenames?
                        if (uniqueDefinitions.GroupBy(p => p.FileNameCI).Count() == 1)
                        {
                            result.Definition = uniqueDefinitions.Last().Definition;
                            result.PriorityType = DefinitionPriorityType.ModOrder;
                        }
                        else
                        {
                            // Using FIOS or LIOS?
                            if (isFios)
                            {
                                result.Definition = uniqueDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.FileName), StringComparer.Ordinal).First().Definition;
                                result.PriorityType = DefinitionPriorityType.FIOS;
                            }
                            else
                            {
                                result.Definition = uniqueDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.FileName), StringComparer.Ordinal).Last().Definition;
                                result.PriorityType = DefinitionPriorityType.LIOS;
                            }
                        }
                    }
                    else
                    {
                        result.Definition = definitions.FirstOrDefault();
                    }
                }
                else
                {
                    result.Definition = definitions.FirstOrDefault();
                }
            }
            else
            {
                result.Definition = definitions?.FirstOrDefault();
            }
            return result;
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

            double total = fileKeys.Count() + typeAndIdKeys.Count() + overwritten.Count();
            double processed = 0;
            ModDefinitionAnalyze?.Invoke(0);

            static IDefinition copyDefinition(IDefinition definition)
            {
                var newDefinition = DIResolver.Get<IDefinition>();
                newDefinition.Code = definition.Code;
                newDefinition.ContentSHA = definition.ContentSHA;
                newDefinition.DefinitionSHA = definition.DefinitionSHA;
                newDefinition.Dependencies = definition.Dependencies;
                newDefinition.ErrorColumn = definition.ErrorColumn;
                newDefinition.ErrorLine = definition.ErrorLine;
                newDefinition.ErrorMessage = definition.ErrorMessage;
                newDefinition.File = definition.File;
                newDefinition.GeneratedFileNames = definition.GeneratedFileNames;
                newDefinition.OverwrittenFileNames = definition.OverwrittenFileNames;
                newDefinition.AdditionalFileNames = definition.AdditionalFileNames;
                newDefinition.Id = definition.Id;
                newDefinition.IsFirstLevel = definition.IsFirstLevel;
                newDefinition.ModName = definition.ModName;
                newDefinition.Type = definition.Type;
                newDefinition.UsedParser = definition.UsedParser;
                newDefinition.ValueType = definition.ValueType;
                return newDefinition;
            }

            // Create virtual empty objects from an empty file
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
                            copy = copyDefinition(emptyConflict);
                            copy.DefinitionSHA = item.DefinitionSHA;
                            copy.Dependencies = item.Dependencies;
                            copy.ModName = item.ModName;
                            copy.Code = item.Code;
                            copy.ContentSHA = item.ContentSHA;
                            copy.UsedParser = item.UsedParser;
                            indexedDefinitions.AddToMap(copy);
                        }
                        foreach (var fileName in item.AdditionalFileNames)
                        {
                            copy.AdditionalFileNames.Add(fileName);
                        }
                    }
                }
            }

            foreach (var item in fileKeys)
            {
                var definitions = indexedDefinitions.GetByFile(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, patchStateMode, fileConflictCache);
                processed++;
                var perc = GetProgressPercentage(total, processed);
                ModDefinitionAnalyze?.Invoke(perc);
            }

            foreach (var item in typeAndIdKeys)
            {
                var definitions = indexedDefinitions.GetByTypeAndId(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, patchStateMode, fileConflictCache);
                processed++;
                var perc = GetProgressPercentage(total, processed);
                ModDefinitionAnalyze?.Invoke(perc);
            }

            var overwrittenDefs = new Dictionary<string, IDefinition>();
            foreach (var item in overwritten.GroupBy(p => p.TypeAndId))
            {
                var conflicted = conflicts.Where(p => p.TypeAndId.Equals(item.First().TypeAndId));
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
                    var newDefinition = copyDefinition(definition);
                    var provider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(GameService.GetSelected().Type));
                    newDefinition.File = Path.Combine(definition.ParentDirectory, definition.Id.GenerateValidFileName() + Path.GetExtension(definition.File));
                    newDefinition.OverwrittenFileNames = definition.OverwrittenFileNames;
                    foreach (var file in definitions.SelectMany(p => p.OverwrittenFileNames))
                    {
                        newDefinition.OverwrittenFileNames.Add(file);
                    }
                    overwrittenDefs.Add(definition.TypeAndId, newDefinition);
                }
                processed++;
                var perc = GetProgressPercentage(total, processed);
                ModDefinitionAnalyze?.Invoke(perc);
            }

            ModDefinitionAnalyze?.Invoke(99);
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
            ModDefinitionAnalyze?.Invoke(100);

            return result;
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
            ModDefinitionLoad?.Invoke(0);

            mods.AsParallel().ForAll((m) =>
            {
                IEnumerable<IDefinition> result = null;
                result = ParseModFiles(game, Reader.Read(m.FullPath), m);
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
                    ModDefinitionLoad?.Invoke(perc);
                }
            });

            ModDefinitionLoad?.Invoke(99);
            var indexed = DIResolver.Get<IIndexedDefinitions>();
            indexed.InitMap(definitions);
            ModDefinitionLoad?.Invoke(100);
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
                    RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
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
            return ExportModPatchDefinitionAsync(conflictResult, definition, collectionName, false);
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
                RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                ModPath = oldPatchName,
                PatchName = newPathName
            });
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
                RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
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
            if (game != null && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                ModDefinitionPatchLoad?.Invoke(0);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                    PatchName = patchName
                });
                if (state != null)
                {
                    var resolvedConflicts = new List<IDefinition>(state.ResolvedConflicts);
                    var ignoredConflicts = new List<IDefinition>();
                    var total = state.Conflicts.Count() + state.OrphanConflicts.Count() + state.OverwrittenConflicts.Count();
                    int processed = 0;
                    foreach (var item in state.OrphanConflicts.GroupBy(p => p.TypeAndId))
                    {
                        var files = ProcessPatchStateFiles(state, item, ref processed);
                        var matchedConflicts = FindPatchStateMatchedConflicts(conflictResult.OrphanConflicts, state, ignoredConflicts, item);
                        await SyncPatchStateAsync(game.UserDirectory, patchName, resolvedConflicts, item, files, matchedConflicts);
                        var perc = GetProgressPercentage(total, processed, 96);
                        ModDefinitionPatchLoad?.Invoke(perc);
                    }
                    foreach (var item in state.Conflicts.GroupBy(p => p.TypeAndId))
                    {
                        var files = ProcessPatchStateFiles(state, item, ref processed);
                        var matchedConflicts = FindPatchStateMatchedConflicts(conflictResult.Conflicts, state, ignoredConflicts, item);
                        await SyncPatchStateAsync(game.UserDirectory, patchName, resolvedConflicts, item, files, matchedConflicts);
                        var perc = GetProgressPercentage(total, processed, 96);
                        ModDefinitionPatchLoad?.Invoke(perc);
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
                        var perc = GetProgressPercentage(total, processed, 96);
                        ModDefinitionPatchLoad?.Invoke(perc);
                    }

                    if (conflictResult.OrphanConflicts.GetAll().Count() > 0)
                    {
                        ModDefinitionPatchLoad?.Invoke(97);
                        if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OrphanConflicts = PopulateModPath(conflictResult.OrphanConflicts.GetAll(), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                            PatchName = patchName
                        }))
                        {
                            foreach (var item in conflictResult.OrphanConflicts.GetAll())
                            {
                                var existing = conflictResult.ResolvedConflicts.GetByTypeAndId(item.TypeAndId);
                                if (existing.Count() == 0)
                                {
                                    conflictResult.ResolvedConflicts.AddToMap(item);
                                }
                            }
                        }
                    }

                    if (conflictResult.OverwrittenConflicts.GetAll().Count() > 0)
                    {
                        ModDefinitionPatchLoad?.Invoke(98);
                        await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OverwrittenConflicts = PopulateModPath(conflictResult.OverwrittenConflicts.GetAll(), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                            PatchName = patchName
                        });
                    }

                    ModDefinitionPatchLoad?.Invoke(99);
                    var conflicts = GetModelInstance<IConflictResult>();
                    conflicts.AllConflicts = conflictResult.AllConflicts;
                    conflicts.Conflicts = conflictResult.Conflicts;
                    conflicts.OrphanConflicts = conflictResult.OrphanConflicts;
                    var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                    resolvedIndex.InitMap(resolvedConflicts, true);
                    conflicts.ResolvedConflicts = resolvedIndex;
                    var ignoredIndex = DIResolver.Get<IIndexedDefinitions>();
                    ignoredIndex.InitMap(ignoredConflicts, true);
                    conflicts.IgnoredConflicts = ignoredIndex;
                    conflicts.IgnoredPaths = state.IgnoreConflictPaths ?? string.Empty;
                    conflicts.OverwrittenConflicts = conflictResult.OverwrittenConflicts;
                    conflicts.Mode = conflictResult.Mode;
                    EvalModIgnoreDefinitions(conflicts);

                    await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        Mode = MapPatchStateMode(conflicts.Mode),
                        IgnoreConflictPaths = conflicts.IgnoredPaths,
                        Conflicts = GetDefinitionOrDefault(conflicts.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflicts.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflicts.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflicts.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflicts.OverwrittenConflicts),
                        RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                        PatchName = patchName
                    });

                    ModDefinitionPatchLoad?.Invoke(100);

                    return conflicts;
                }
                else
                {
                    var exportedConflicts = false;
                    if (conflictResult.OrphanConflicts.GetAll().Count() > 0)
                    {
                        ModDefinitionPatchLoad?.Invoke(96);
                        if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OrphanConflicts = PopulateModPath(conflictResult.OrphanConflicts.GetAll(), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                            PatchName = patchName
                        }))
                        {
                            foreach (var item in conflictResult.OrphanConflicts.GetAll())
                            {
                                var existing = conflictResult.ResolvedConflicts.GetByTypeAndId(item.TypeAndId);
                                if (existing.Count() == 0)
                                {
                                    conflictResult.ResolvedConflicts.AddToMap(item);
                                }
                            }
                            exportedConflicts = true;
                        }
                        ModDefinitionPatchLoad?.Invoke(97);
                    }

                    if (conflictResult.OverwrittenConflicts.GetAll().Count() > 0)
                    {
                        exportedConflicts = true;
                        ModDefinitionPatchLoad?.Invoke(98);
                        await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                        {
                            Game = game.Type,
                            OverwrittenConflicts = PopulateModPath(conflictResult.OverwrittenConflicts.GetAll(), GetCollectionMods()),
                            RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                            PatchName = patchName
                        });
                        ModDefinitionPatchLoad?.Invoke(99);
                    }

                    ModDefinitionPatchLoad?.Invoke(100);
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
                            RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
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
                var ignoreRules = new List<string>();
                var includeRules = new List<string>();
                var lines = conflictResult.IgnoredPaths.SplitOnNewLine().Where(p => !p.Trim().StartsWith("#"));
                foreach (var line in lines)
                {
                    var parsed = line.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Trim().TrimStart(Path.DirectorySeparatorChar);
                    if (!parsed.StartsWith("!"))
                    {
                        ignoreRules.Add(parsed);
                    }
                    else
                    {
                        includeRules.Add(parsed.TrimStart('!'));
                    }
                }
                foreach (var topConflict in conflictResult.Conflicts.GetHierarchicalDefinitions())
                {
                    var name = topConflict.Name;
                    if (!name.EndsWith(Path.DirectorySeparatorChar))
                    {
                        name = $"{name}{Path.DirectorySeparatorChar}";
                    }
                    if (ignoreRules.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase)) && !includeRules.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        foreach (var item in topConflict.Children)
                        {
                            ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
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
        /// <param name="resolve">if set to <c>true</c> [resolve].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> ExportModPatchDefinitionAsync(IConflictResult conflictResult, IDefinition definition, string collectionName, bool resolve = true)
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
                        Path = Constants.ModDirectory
                    });
                    await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = mod,
                        RootDirectory = game.UserDirectory,
                        Path = mod.DescriptorFile
                    });
                    allMods.Add(mod);
                }
                else
                {
                    mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
                }
                var definitionMod = allMods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                if (definitionMod != null)
                {
                    if (resolve)
                    {
                        conflictResult.ResolvedConflicts.AddToMap(definition);
                    }
                    else
                    {
                        conflictResult.IgnoredConflicts.AddToMap(definition);
                    }
                    await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = game.UserDirectory,
                        Path = Path.Combine(Constants.ModDirectory, patchName)
                    });
                    var exportPatches = new HashSet<IDefinition>();
                    if (resolve)
                    {
                        exportPatches.Add(definition);
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
                            RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
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
                        RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                        PatchName = patchName
                    });
                    return resolve ? exportResult && stateResult : stateResult;
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
            static string mergeCode(string code, IEnumerable<string> lines)
            {
                if (lines.Count() > 0)
                {
                    var index = code.IndexOf("{") + 1;
                    var result = code.Insert(index, Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine);
                    return string.Join(Environment.NewLine, result.SplitOnNewLine());
                }
                return code;
            }
            static List<string> cleanCode(IEnumerable<IDefinition> defs)
            {
                var result = new List<string>();
                if (defs.Count() > 0)
                {
                    foreach (var item in defs)
                    {
                        var filtered = item.Code.Substring(item.Code.IndexOf("{") + 1).Replace("\r", string.Empty).Replace("\n", string.Empty);
                        result.Add(filtered.Substring(0, filtered.LastIndexOf("}")).Trim());
                    }
                }
                return result;
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
                        var variables = variableDefinitions.Where(p => definition.Code.Contains(p.Id));
                        if (definition.IsFirstLevel)
                        {
                            StringBuilder sb = new StringBuilder();
                            appendLine(sb, namespaces.Select(p => p.Code));
                            appendLine(sb, variables.Select(p => p.Code));
                            appendLine(sb, new List<string> { definition.Code });
                            definition.Code = sb.ToString();
                        }
                        else
                        {
                            definition.Code = mergeCode(definition.Code, cleanCode(variables));
                            definition.Code = mergeCode(definition.Code, cleanCode(namespaces));
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
        /// Populates the mod path.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="collectionMods">The collection mods.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> PopulateModPath(IEnumerable<IDefinition> definitions, IEnumerable<IMod> collectionMods)
        {
            if (definitions?.Count() > 0)
            {
                foreach (var item in definitions)
                {
                    if (IsPatchMod(item.ModName))
                    {
                        item.ModPath = GetPatchDirectory(GameService.GetSelected(), item.ModName);
                    }
                    else
                    {
                        item.ModPath = collectionMods.FirstOrDefault(p => p.Name.Equals(item.ModName)).FullPath;
                    }
                }
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
                        RootDirectory = Path.Combine(userDirectory, Constants.ModDirectory, patchName),
                        Path = file
                    });
                }
                return true;
            }
            return false;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DefinitionEval.
        /// </summary>
        private class DefinitionEval
        {
            #region Properties

            /// <summary>
            /// Gets or sets the definition.
            /// </summary>
            /// <value>The definition.</value>
            public IDefinition Definition { get; set; }

            /// <summary>
            /// Gets or sets the name of the file.
            /// </summary>
            /// <value>The name of the file.</value>
            public string FileName { get; set; }

            /// <summary>
            /// Gets the file name ci.
            /// </summary>
            /// <value>The file name ci.</value>
            public string FileNameCI
            {
                get
                {
                    return FileName.ToLowerInvariant();
                }
            }

            #endregion Properties
        }

        #endregion Classes
    }
}
