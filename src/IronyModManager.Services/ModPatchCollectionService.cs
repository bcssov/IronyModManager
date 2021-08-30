﻿// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 08-27-2021
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
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
        /// The cache region
        /// </summary>
        private const string CacheRegion = "CollectionPatchState";

        /// <summary>
        /// The maximum mod conflicts to check
        /// </summary>
        private const int MaxModConflictsToCheck = 4;

        /// <summary>
        /// The mod name ignore identifier
        /// </summary>
        private const string ModNameIgnoreId = "modName:";

        /// <summary>
        /// The ignore game mods identifier
        /// </summary>
        private const string ShowGameModsId = "--showGameMods";

        /// <summary>
        /// The show self conflicts identifier
        /// </summary>
        private const string ShowSelfConflictsId = "--showSelfConflicts";

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

        private readonly IValidateParser validateParser;

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
            IStorageProvider storageProvider, IMapper mapper, IValidateParser validateParser) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.messageBus = messageBus;
            this.parserManager = parserManager;
            this.modPatchExporter = modPatchExporter;
            this.validateParser = validateParser;
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
                var lines = ignoredPaths.SplitOnNewLine();
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
            return await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = GetPatchModDirectory(game, patchName)
            }, true);
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
            var modDirRootPath = GetModDirectoryRootPath(game);
            var oldPatchName = GenerateCollectionPatchName(collectionName);
            var newPatchName = GenerateCollectionPatchName(newCollectionName);
            return modPatchExporter.CopyPatchModAsync(new ModPatchExporterParameters()
            {
                RootPath = modDirRootPath,
                ModPath = EvaluatePatchNamePath(game, oldPatchName, modDirRootPath),
                PatchPath = EvaluatePatchNamePath(game, newPatchName, modDirRootPath),
                RenamePairs = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(oldPatchName, newPatchName) }
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
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patch.ModName)
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
            var overwritten = indexedDefinitions.GetByValueType(ValueType.OverwrittenObject).Concat(indexedDefinitions.GetByValueType(ValueType.OverWrittenObjectWithPreserveFileName));
            var empty = indexedDefinitions.GetByValueType(ValueType.EmptyFile);

            double total = fileKeys.Count() + typeAndIdKeys.Count() + overwritten.Count() + empty.Count();
            double processed = 0;
            double previousProgress = 0;
            messageBus.Publish(new ModDefinitionAnalyzeEvent(0));

            // Create virtual empty objects from an empty file
            foreach (var item in empty)
            {
                var emptyConflicts = indexedDefinitions.GetByFile(item.File);
                if (emptyConflicts.Any())
                {
                    foreach (var emptyConflict in emptyConflicts.Where(p => p.ValueType != ValueType.Invalid && !p.ModName.Equals(item.ModName) && !p.File.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase)))
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
                            copy.OriginalModName = item.OriginalModName;
                            copy.OriginalFileName = item.OriginalFileName;
                            copy.Variables = item.Variables;
                            indexedDefinitions.AddToMap(copy);
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
                var perc = GetProgressPercentage(total, processed, 99.9);
                if (perc != previousProgress)
                {
                    messageBus.Publish(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            foreach (var item in fileKeys)
            {
                var definitions = indexedDefinitions.GetByFile(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, patchStateMode, fileConflictCache);
                processed++;
                var perc = GetProgressPercentage(total, processed, 99.9);
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
                var perc = GetProgressPercentage(total, processed, 99.9);
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
                if (conflicted.Any())
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
                        var hasOverrides = all.Any(p => !p.IsCustomPatch && p.Dependencies != null && p.Dependencies.Any(p => p.Equals(def.ModName)));
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
                    var oldFileName = newDefinition.File;
                    newDefinition.File = Path.Combine(definition.ParentDirectory, definition.Id.GenerateValidFileName() + Path.GetExtension(definition.File));
                    var overwrittenFileNames = definition.OverwrittenFileNames;
                    foreach (var file in definitions.SelectMany(p => p.OverwrittenFileNames))
                    {
                        overwrittenFileNames.Add(file);
                    }
                    newDefinition.OverwrittenFileNames = overwrittenFileNames.Distinct().ToList();
                    newDefinition.DiskFile = provider.GetDiskFileName(newDefinition);
                    var preserveOverwrittenFileName = oldFileName == newDefinition.File;
                    newDefinition.File = provider.GetFileName(newDefinition);
                    if (preserveOverwrittenFileName)
                    {
                        // What are the chances that generated filename will match
                        var preservedOverwrittenFileName = newDefinition.OverwrittenFileNames;
                        preservedOverwrittenFileName.Add(oldFileName);
                        newDefinition.OverwrittenFileNames = preservedOverwrittenFileName;
                    }
                    overwrittenDefs.Add(definition.TypeAndId, newDefinition);
                }
                processed++;
                var perc = GetProgressPercentage(total, processed, 99.9);
                if (perc != previousProgress)
                {
                    messageBus.Publish(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            messageBus.Publish(new ModDefinitionAnalyzeEvent(99.9));
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
                        var ignoredModName = line.Replace(ModNameIgnoreId, string.Empty).Trim();
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
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public virtual async Task<IIndexedDefinitions> GetModObjectsAsync(IGame game, IEnumerable<IMod> mods, string collectionName)
        {
            if (game == null || mods == null || !mods.Any())
            {
                return null;
            }
            var definitions = new ConcurrentBag<IDefinition>();

            double processed = 0;
            double total = mods.Count();
            double previousProgress = 0;
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
                    var perc = GetProgressPercentage(total, processed, 100);
                    if (perc != previousProgress)
                    {
                        messageBus.Publish(new ModDefinitionLoadEvent(perc));
                        previousProgress = perc;
                    }
                }
            });

            await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(0));
            processed = 0;
            total = definitions.Count;
            previousProgress = 0;
            List<IDefinition> prunedDefinitions;
            var patchName = GenerateCollectionPatchName(collectionName);
            var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
            {
                RootPath = GetModDirectoryRootPath(game),
                PatchPath = EvaluatePatchNamePath(game, patchName)
            });
            if (state != null && state.CustomConflicts.Any())
            {
                prunedDefinitions = new List<IDefinition>();
                var customIndexed = DIResolver.Get<IIndexedDefinitions>();
                customIndexed.InitMap(state.CustomConflicts);
                foreach (var item in definitions)
                {
                    bool addDefault = true;
                    if (item.ValueType == ValueType.Invalid)
                    {
                        var fileCodes = customIndexed.GetByFile(item.File);
                        if (fileCodes.Any())
                        {
                            var fileDefs = new List<IDefinition>();
                            foreach (var fileCode in fileCodes)
                            {
                                var history = state.ConflictHistory.FirstOrDefault(p => p.TypeAndId.Equals(fileCode.TypeAndId));
                                if (history != null && !string.IsNullOrWhiteSpace(history.Code))
                                {
                                    fileDefs.AddRange(parserManager.Parse(new ParserManagerArgs()
                                    {
                                        ContentSHA = item.ContentSHA,
                                        File = item.File,
                                        GameType = game.Type,
                                        Lines = history.Code.SplitOnNewLine(false),
                                        ModDependencies = item.Dependencies,
                                        ModName = item.ModName
                                    }));
                                }
                            }
                            if (fileDefs.Any())
                            {
                                foreach (var def in fileDefs)
                                {
                                    def.IsCustomPatch = true;
                                }
                                addDefault = false;
                                MergeDefinitions(fileDefs);
                                prunedDefinitions.AddRange(fileDefs);
                            }
                        }
                    }
                    if (addDefault)
                    {
                        prunedDefinitions.Add(item);
                    }
                    processed++;
                    var perc = GetProgressPercentage(total, processed, 100);
                    if (perc != previousProgress)
                    {
                        await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(perc));
                        previousProgress = perc;
                    }
                }
            }
            else
            {
                prunedDefinitions = definitions.ToList();
            }
            definitions.Clear();
            definitions = null;

            await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(99.9));
            var indexed = DIResolver.Get<IIndexedDefinitions>();
            indexed.InitMap(prunedDefinitions);
            await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(100));
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
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patchName)
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
        /// initialize patch state as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        public virtual async Task<IConflictResult> InitializePatchStateAsync(IConflictResult conflictResult, string collectionName)
        {
            var game = GameService.GetSelected();
            double previousProgress = 0;
            async Task<int> syncPatchFiles(IConflictResult conflicts, IEnumerable<string> patchFiles, string patchName, int total, int processed, int maxProgress)
            {
                var patchModDir = GetPatchModDirectory(game, patchName);
                foreach (var file in patchFiles.Distinct())
                {
                    var cleaned = false;
                    if (!conflicts.CustomConflicts.ExistsByFile(file) &&
                        !conflicts.OrphanConflicts.ExistsByFile(file) &&
                        !conflicts.OverwrittenConflicts.ExistsByFile(file) &&
                        !conflicts.ResolvedConflicts.ExistsByFile(file))
                    {
                        cleaned = await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                        {
                            RootDirectory = patchModDir,
                            Path = file
                        });
                    }
                    if (!cleaned)
                    {
                        var resolved = conflicts.ResolvedConflicts.GetByDiskFile(file);
                        if (resolved.Any())
                        {
                            var overwritten = conflicts.OverwrittenConflicts.GetByTypeAndId(resolved.FirstOrDefault().TypeAndId);
                            if (overwritten.Any() && overwritten.FirstOrDefault().DiskFileCI != resolved.FirstOrDefault().DiskFileCI)
                            {
                                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                                {
                                    RootDirectory = patchModDir,
                                    Path = overwritten.FirstOrDefault().DiskFile
                                });
                            }
                        }
                    }
                    processed++;
                    var perc = GetProgressPercentage(total, processed, maxProgress);
                    if (previousProgress != perc)
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                        previousProgress = perc;
                    }
                }
                return processed;
            }
            IDefinition partialDefinitionCopy(IDefinition definition)
            {
                if (definition.ValueType == ValueType.Invalid)
                {
                    // Perform a full copy for invalid items
                    return CopyDefinition(definition);
                }
                var copy = DIResolver.Get<IDefinition>();
                copy.DiskFile = definition.DiskFile;
                copy.File = definition.File;
                copy.Id = definition.Id;
                copy.ModName = definition.ModName;
                copy.Tags = definition.Tags;
                copy.Type = definition.Type;
                copy.ValueType = definition.ValueType;
                copy.IsFromGame = definition.IsFromGame;
                return copy;
            }
            async Task<(IIndexedDefinitions, int)> partialCopyIndexedDefinitions(IIndexedDefinitions indexedDefinitions, int total, int processed, int maxProgress)
            {
                var copy = DIResolver.Get<IIndexedDefinitions>();
                foreach (var item in indexedDefinitions.GetAll())
                {
                    copy.AddToMap(partialDefinitionCopy(item));
                    processed++;
                    var perc = GetProgressPercentage(total, processed, maxProgress);
                    if (previousProgress != perc)
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                        previousProgress = perc;
                    }
                }
                return (copy, processed);
            }

            if (game != null && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                double perc = 0;
                var patchName = GenerateCollectionPatchName(collectionName);
                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(0));
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patchName)
                });
                var patchFiles = modPatchExporter.GetPatchFiles(new ModPatchExporterParameters()
                {
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patchName)
                });
                var total = patchFiles.Count() + conflictResult.AllConflicts.GetAll().Count();
                if (state != null)
                {
                    var resolvedConflicts = new List<IDefinition>(state.ResolvedConflicts);
                    var ignoredConflicts = new List<IDefinition>();
                    total += state.Conflicts.Count() + (state.OrphanConflicts.Count() * 2) + (state.OverwrittenConflicts.Count() * 2) + 1;
                    int processed = 0;
                    foreach (var item in state.OrphanConflicts.GroupBy(p => p.TypeAndId))
                    {
                        var files = ProcessPatchStateFiles(state, item, ref processed);
                        var matchedConflicts = FindPatchStateMatchedConflicts(conflictResult.OrphanConflicts, state, ignoredConflicts, item);
                        await SyncPatchStateAsync(game, patchName, resolvedConflicts, item, files, matchedConflicts);
                        perc = GetProgressPercentage(total, processed);
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
                        await SyncPatchStateAsync(game, patchName, resolvedConflicts, item, files, matchedConflicts);
                        perc = GetProgressPercentage(total, processed);
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
                        files.AddRange(item.Where(p => !string.IsNullOrWhiteSpace(p.DiskFile)).ToList().Select(p => p.DiskFile));
                        if (state.ResolvedConflicts != null)
                        {
                            var resolved = state.ResolvedConflicts.Where(p => p.TypeAndId.Equals(item.First().TypeAndId));
                            if (resolved.Any())
                            {
                                var fileNames = resolved.Select(p => p.File).ToList();
                                fileNames.AddRange(resolved.Where(p => !string.IsNullOrWhiteSpace(p.DiskFile)).ToList().Select(p => p.DiskFile));
                                files.RemoveAll(p => fileNames.Any(a => a.Equals(p, StringComparison.OrdinalIgnoreCase)));
                            }
                        }
                        var matchedConflicts = conflictResult.OverwrittenConflicts.GetByTypeAndId(item.First().TypeAndId);
                        await SyncPatchStatesAsync(matchedConflicts, item, patchName, game, files.ToArray());
                        perc = GetProgressPercentage(total, processed);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }

                    if (conflictResult.OrphanConflicts.GetAll().Any())
                    {
                        var orphanConflicts = PopulateModPath(conflictResult.OrphanConflicts.GetAll(), GetCollectionMods());
                        foreach (var item in orphanConflicts)
                        {
                            if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                            {
                                Game = game.Type,
                                OrphanConflicts = new List<IDefinition>() { item },
                                RootPath = GetModDirectoryRootPath(game),
                                PatchPath = EvaluatePatchNamePath(game, patchName)
                            }))
                            {
                                var existing = conflictResult.ResolvedConflicts.GetByTypeAndId(item.TypeAndId);
                                if (!existing.Any())
                                {
                                    conflictResult.ResolvedConflicts.AddToMap(item, true);
                                }
                            }
                            processed++;
                            perc = GetProgressPercentage(total, processed);
                            if (previousProgress != perc)
                            {
                                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                                previousProgress = perc;
                            }
                        }
                    }

                    var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                    resolvedIndex.InitMap(resolvedConflicts, true);

                    if (conflictResult.OverwrittenConflicts.GetAll().Any())
                    {
                        var overwrittenConflicts = PopulateModPath(conflictResult.OverwrittenConflicts.GetAll().Where(p => !resolvedIndex.GetByTypeAndId(p.TypeAndId).Any()), GetCollectionMods());
                        foreach (var item in overwrittenConflicts)
                        {
                            await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                            {
                                Game = game.Type,
                                OverwrittenConflicts = new List<IDefinition>() { item },
                                RootPath = GetModDirectoryRootPath(game),
                                PatchPath = EvaluatePatchNamePath(game, patchName)
                            });
                        }
                        processed++;
                        perc = GetProgressPercentage(total, processed);
                        if (previousProgress != perc)
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }

                    var conflicts = GetModelInstance<IConflictResult>();

                    var partialCopyResult = await partialCopyIndexedDefinitions(conflictResult.AllConflicts, total, processed, 100);
                    conflictResult.AllConflicts.Dispose();
                    conflicts.AllConflicts = partialCopyResult.Item1;
                    processed = partialCopyResult.Item2;

                    var conflictsIndex = DIResolver.Get<IIndexedDefinitions>();
                    conflictsIndex.InitMap(conflictResult.Conflicts.GetAll(), true);
                    conflicts.Conflicts = conflictsIndex;
                    var orphanIndex = DIResolver.Get<IIndexedDefinitions>();
                    orphanIndex.InitMap(conflictResult.OrphanConflicts.GetAll(), false);
                    conflicts.OrphanConflicts = orphanIndex;
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
                    processed = await syncPatchFiles(conflicts, patchFiles, patchName, total, processed, 100);

                    await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                        Mode = MapPatchStateMode(conflicts.Mode),
                        IgnoreConflictPaths = conflicts.IgnoredPaths,
                        Conflicts = GetDefinitionOrDefault(conflicts.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflicts.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflicts.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflicts.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflicts.OverwrittenConflicts),
                        CustomConflicts = GetDefinitionOrDefault(conflicts.CustomConflicts),
                        RootPath = GetModDirectoryRootPath(game),
                        PatchPath = EvaluatePatchNamePath(game, patchName),
                        HasGameDefinitions = conflicts.AllConflicts.HasGameDefinitions()
                    });
                    await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(100));

                    // Initialize search here
                    conflicts.AllConflicts.InitSearch();

                    return conflicts;
                }
                else
                {
                    var processed = 0;
                    processed = await syncPatchFiles(conflictResult, patchFiles, patchName, total, processed, 100);

                    var exportedConflicts = false;
                    if (conflictResult.OrphanConflicts.GetAll().Any())
                    {
                        var orphanConflicts = PopulateModPath(conflictResult.OrphanConflicts.GetAll(), GetCollectionMods());
                        foreach (var item in orphanConflicts)
                        {
                            if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                            {
                                Game = game.Type,
                                OrphanConflicts = new List<IDefinition>() { item },
                                RootPath = GetModDirectoryRootPath(game),
                                PatchPath = EvaluatePatchNamePath(game, patchName)
                            }))
                            {
                                var existing = conflictResult.ResolvedConflicts.GetByTypeAndId(item.TypeAndId);
                                if (!existing.Any())
                                {
                                    conflictResult.ResolvedConflicts.AddToMap(item, true);
                                }
                                exportedConflicts = true;
                            }
                            processed++;
                            perc = GetProgressPercentage(total, processed);
                            if (previousProgress != perc)
                            {
                                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                                previousProgress = perc;
                            }
                        }
                    }

                    if (conflictResult.OverwrittenConflicts.GetAll().Any())
                    {
                        var overwrittenConflicts = PopulateModPath(conflictResult.OverwrittenConflicts.GetAll().Where(p => !conflictResult.ResolvedConflicts.GetByTypeAndId(p.TypeAndId).Any()), GetCollectionMods());
                        foreach (var item in overwrittenConflicts)
                        {
                            if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                            {
                                Game = game.Type,
                                OverwrittenConflicts = new List<IDefinition>() { item },
                                RootPath = GetModDirectoryRootPath(game),
                                PatchPath = EvaluatePatchNamePath(game, patchName)
                            }))
                            {
                                exportedConflicts = true;
                            }
                            processed++;
                            perc = GetProgressPercentage(total, processed);
                            if (previousProgress != perc)
                            {
                                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                                previousProgress = perc;
                            }
                        }
                    }

                    if (exportedConflicts)
                    {
                        EvalModIgnoreDefinitions(conflictResult);

                        await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                        {
                            LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                            Mode = MapPatchStateMode(conflictResult.Mode),
                            IgnoreConflictPaths = conflictResult.IgnoredPaths,
                            Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                            OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                            ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                            IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                            OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                            CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                            RootPath = GetModDirectoryRootPath(game),
                            PatchPath = EvaluatePatchNamePath(game, patchName),
                            HasGameDefinitions = conflictResult.AllConflicts.HasGameDefinitions()
                        });
                    }

                    var partialCopyResult = await partialCopyIndexedDefinitions(conflictResult.AllConflicts, total, processed, 100);
                    conflictResult.AllConflicts.Dispose();
                    conflictResult.AllConflicts = partialCopyResult.Item1;
                    processed = partialCopyResult.Item2;

                    await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(100));

                    // Initialize search here
                    conflictResult.AllConflicts.InitSearch();

                    return conflictResult;
                }
            };
            return null;
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
                Cache.Invalidate(new CacheInvalidateParameters() { Region = CacheRegion, Prefix = game.Type, Keys = new List<string>() { patchName } });
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
                RootPath = GetModDirectoryRootPath(game),
                PatchPath = EvaluatePatchNamePath(game, patchName)
            }, definition.File);
        }

        /// <summary>
        /// patch has game definitions as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> PatchHasGameDefinitionsAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patchName)
                }, false);
                if (state != null)
                {
                    return state.HasGameDefinitions;
                }
            }
            return false;
        }

        /// <summary>
        /// patch mod needs update as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadOrder">The load order.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> PatchModNeedsUpdateAsync(string collectionName, IReadOnlyCollection<string> loadOrder)
        {
            loadOrder ??= new List<string>();
            List<EvalState> mapEvalState(IEnumerable<IDefinition> definitions)
            {
                var result = new List<EvalState>();
                if ((definitions?.Any()).GetValueOrDefault())
                {
                    result.AddRange(definitions.Where(p => !p.IsFromGame).Select(m => new EvalState()
                    {
                        ContentSha = m.ContentSHA,
                        FileName = m.OriginalFileName,
                        FallBackFileName = m.File,
                        ModName = m.ModName
                    }));
                }
                return result;
            }
            async Task<bool> evalState(IGame game, string patchName)
            {
                Cache.Set(new CacheAddParameters<PatchCollectionState>() { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState() { CheckInProgress = true } });
                var allMods = GetInstalledModsInternal(game, false);
                var mods = allMods.Where(p => loadOrder.Any(x => x.Equals(p.DescriptorFile))).ToList();
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patchName)
                }, false);
                if (state == null)
                {
                    Cache.Set(new CacheAddParameters<PatchCollectionState>() { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState() { NeedsUpdate = false, CheckInProgress = false } });
                    return false;
                }
                // Check load order first
                if (!state.LoadOrder.SequenceEqual(loadOrder))
                {
                    Cache.Set(new CacheAddParameters<PatchCollectionState>() { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState() { NeedsUpdate = true, CheckInProgress = false } });
                    return true;
                }
                var conflicts = new List<EvalState>();
                conflicts.AddRange(mapEvalState(state.Conflicts));
                conflicts.AddRange(mapEvalState(state.OrphanConflicts));
                conflicts.AddRange(mapEvalState(state.OverwrittenConflicts));
                var cancellationToken = new CancellationTokenSource();
                var semaphore = new AsyncSemaphore(MaxModConflictsToCheck);
                var tasks = conflicts.GroupBy(p => p.ModName).Select(async groupedMods =>
                {
                    await semaphore.WaitAsync(cancellationToken.Token);
                    try
                    {
                        return await Task.Run(() =>
                        {
                            foreach (var item in groupedMods.GroupBy(p => p.FileName))
                            {
                                var definition = item.FirstOrDefault();
                                var mod = mods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                                if (mod == null)
                                {
                                    // Mod no longer in collection, needs refresh break further checks...
                                    Cache.Set(new CacheAddParameters<PatchCollectionState>() { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState() { NeedsUpdate = true, CheckInProgress = false } });
                                    return true;
                                }
                                else
                                {
                                    var info = Reader.GetFileInfo(mod.FullPath, definition.FileName);
                                    if (info == null)
                                    {
                                        info = Reader.GetFileInfo(mod.FullPath, definition.FallBackFileName);
                                    }
                                    if (info == null || !info.ContentSHA.Equals(definition.ContentSha))
                                    {
                                        // File no longer in collection or content does not match, break further checks
                                        Cache.Set(new CacheAddParameters<PatchCollectionState>() { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState() { NeedsUpdate = true, CheckInProgress = false } });
                                        return true;
                                    }
                                }
                            }
                            return false;
                        }, cancellationToken.Token);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }).ToList();
                while (tasks.Any())
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                    var result = await task;
                    if (result)
                    {
                        cancellationToken.Cancel();
                        return true;
                    }
                }
                Cache.Set(new CacheAddParameters<PatchCollectionState>() { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState() { NeedsUpdate = false, CheckInProgress = false } });
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
                var result = Cache.Get<PatchCollectionState>(new CacheGetParameters() { Region = CacheRegion, Prefix = game.Type, Key = patchName });
                if (result != null)
                {
                    while (result.CheckInProgress)
                    {
                        // Since another check is queued, wait and periodically check if the task is done...
                        await Task.Delay(10);
                        result = Cache.Get<PatchCollectionState>(new CacheGetParameters() { Region = CacheRegion, Prefix = game.Type, Key = patchName });
                        if (result == null)
                        {
                            await evalState(game, patchName);
                            result = Cache.Get<PatchCollectionState>(new CacheGetParameters() { Region = CacheRegion, Prefix = game.Type, Key = patchName });
                        }
                    }
                    return result.NeedsUpdate;
                }
                else
                {
                    return await evalState(game, patchName);
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
            var newPatchName = GenerateCollectionPatchName(newCollectionName);
            return modPatchExporter.RenamePatchModAsync(new ModPatchExporterParameters()
            {
                RootPath = GetModDirectoryRootPath(game),
                ModPath = EvaluatePatchNamePath(game, oldPatchName),
                PatchPath = EvaluatePatchNamePath(game, newPatchName),
                RenamePairs = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(oldPatchName, newPatchName) }
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
            return UnResolveConflictAsync(conflictResult, typeAndId, collectionName, ExportType.Custom);
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
            return UnResolveConflictAsync(conflictResult, typeAndId, collectionName, ExportType.Ignored);
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
            return UnResolveConflictAsync(conflictResult, typeAndId, collectionName, ExportType.Resolved);
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
                    if (mod.FullPath.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase) || mod.FullPath.EndsWith(Shared.Constants.BinExtension, StringComparison.OrdinalIgnoreCase))
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
                LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                Mode = MapPatchStateMode(conflictResult.Mode),
                IgnoreConflictPaths = conflictResult.IgnoredPaths,
                Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                RootPath = GetModDirectoryRootPath(game),
                PatchPath = EvaluatePatchNamePath(game, patchName),
                HasGameDefinitions = conflictResult.AllConflicts.HasGameDefinitions()
            });
        }

        /// <summary>
        /// Shoulds the ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool? ShouldIgnoreGameMods(IConflictResult conflictResult)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var lines = ignoredPaths.SplitOnNewLine();
                return !lines.Any(p => p.Equals(ShowGameModsId));
            }
            return null;
        }

        /// <summary>
        /// Shoulds the show self conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool? ShouldShowSelfConflicts(IConflictResult conflictResult)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var lines = ignoredPaths.SplitOnNewLine();
                return lines.Any(p => p.Equals(ShowSelfConflictsId));
            }
            return null;
        }

        /// <summary>
        /// Toggles the ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool? ToggleIgnoreGameMods(IConflictResult conflictResult)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var shouldIgnore = ShouldIgnoreGameMods(conflictResult);
                var lines = ignoredPaths.SplitOnNewLine().ToList();
                if (shouldIgnore.GetValueOrDefault())
                {
                    lines.Add(ShowGameModsId);
                }
                else
                {
                    lines.Remove(ShowGameModsId);
                }
                conflictResult.IgnoredPaths = string.Join(Environment.NewLine, lines).Trim(Environment.NewLine.ToCharArray());
                return !shouldIgnore;
            }
            return null;
        }

        /// <summary>
        /// Toggles self mod conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool? ToggleSelfModConflicts(IConflictResult conflictResult)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var shouldShow = ShouldShowSelfConflicts(conflictResult);
                var lines = ignoredPaths.SplitOnNewLine().ToList();
                if (!shouldShow.GetValueOrDefault())
                {
                    lines.Add(ShowSelfConflictsId);
                }
                else
                {
                    lines.Remove(ShowSelfConflictsId);
                }
                conflictResult.IgnoredPaths = string.Join(Environment.NewLine, lines).Trim(Environment.NewLine.ToCharArray());
                return !shouldShow;
            }
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
            bool existsInLastFile(IDefinition definition)
            {
                var result = true;
                var fileDefs = indexedDefinitions.GetByFile(definition.FileCI);
                var lastMod = fileDefs.GroupBy(p => p.ModName).Select(p => p.First()).OrderByDescending(p => modOrder.IndexOf(p.ModName)).FirstOrDefault();
                if (lastMod != null)
                {
                    result = fileDefs.Any(p => p.ModName.Equals(lastMod.ModName) && p.TypeAndId.Equals(definition.TypeAndId));
                }
                return result;
            }
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
                            var hasOverrides = allConflicts.Any(p => !p.IsCustomPatch && p.Dependencies != null && p.Dependencies.Any(p => p.Equals(conflict.ModName)));
                            if (hasOverrides && patchStateMode == PatchStateMode.Default)
                            {
                                var existing = allConflicts.Where(p => !p.IsCustomPatch && p.Dependencies != null && p.Dependencies.Any(p => p.Equals(conflict.ModName)));
                                if (existing.Any())
                                {
                                    var fileNames = conflict.AdditionalFileNames;
                                    foreach (var item in existing.Where(p => p != null))
                                    {
                                        foreach (var fileName in item.AdditionalFileNames)
                                        {
                                            fileNames.Add(fileName);
                                        }
                                    }
                                    conflict.AdditionalFileNames = fileNames;
                                }
                                continue;
                            }
                            validConflicts.Add(conflict);
                        }

                        var validConflictsGroup = validConflicts.GroupBy(p => p.DefinitionSHA);
                        if (validConflictsGroup.Count() > 1)
                        {
                            var filteredConflicts = validConflictsGroup.Select(p => EvalDefinitionPriority(p.OrderBy(m => modOrder.IndexOf(m.ModName))).Definition);
                            foreach (var item in filteredConflicts)
                            {
                                if (!conflicts.Contains(item) && IsValidDefinitionType(item))
                                {
                                    var shaMatches = validConflictsGroup.FirstOrDefault(p => p.Key == item.DefinitionSHA);
                                    if (shaMatches.Count() > 1)
                                    {
                                        var fileNames = item.AdditionalFileNames;
                                        foreach (var shaMatch in shaMatches.Where(p => p != item))
                                        {
                                            foreach (var fileName in shaMatch.AdditionalFileNames)
                                            {
                                                fileNames.Add(fileName);
                                            }
                                        }
                                        item.AdditionalFileNames = fileNames;
                                    }
                                    item.ExistsInLastFile = existsInLastFile(item);
                                    conflicts.Add(item);
                                }
                            }
                        }
                    }
                }
                else if (allConflicts.Count() == 1)
                {
                    if (allConflicts.FirstOrDefault().ValueType == ValueType.Binary)
                    {
                        fileConflictCache.TryAdd(def.FileCI, false);
                    }
                    else
                    {
                        if (fileConflictCache.TryGetValue(def.FileCI, out var result))
                        {
                            if (result)
                            {
                                if (!def.IsFromGame && !conflicts.Contains(def) && IsValidDefinitionType(def))
                                {
                                    def.ExistsInLastFile = existsInLastFile(def);
                                    if (!def.ExistsInLastFile)
                                    {
                                        conflicts.Add(def);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var fileDefs = indexedDefinitions.GetByFile(def.FileCI);
                            if (fileDefs.GroupBy(p => p.ModName).Count() > 1)
                            {
                                var hasOverrides = !def.IsCustomPatch && def.Dependencies != null && def.Dependencies.Any(p => fileDefs.Any(s => s.ModName.Equals(p)));
                                if (hasOverrides && patchStateMode == PatchStateMode.Default)
                                {
                                    fileConflictCache.TryAdd(def.FileCI, false);
                                }
                                else
                                {
                                    fileConflictCache.TryAdd(def.FileCI, true);
                                    if (!def.IsFromGame && !conflicts.Contains(def) && IsValidDefinitionType(def))
                                    {
                                        def.ExistsInLastFile = existsInLastFile(def);
                                        if (!def.ExistsInLastFile)
                                        {
                                            conflicts.Add(def);
                                        }
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
            var ignoreGameMods = true;
            var ignoreSelfConflicts = true;
            var alreadyIgnored = new HashSet<string>();
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
                        var ignoredModName = line.Replace(ModNameIgnoreId, string.Empty).Trim();
                        allowedMods.Remove(ignoredModName);
                    }
                    else if (parsed.Equals(ShowGameModsId))
                    {
                        ignoreGameMods = false;
                    }
                    else if (parsed.Equals(ShowSelfConflictsId))
                    {
                        ignoreSelfConflicts = false;
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
                        var invalid = topConflict.Children.Where(p => ignoreRules.Any(r => EvalWildcard(r, p.FileNames.ToArray()))).Where(p => !includeRules.Any(r => EvalWildcard(r, p.FileNames.ToArray())));
                        foreach (var item in invalid)
                        {
                            if (!alreadyIgnored.Contains(item.Key))
                            {
                                alreadyIgnored.Add(item.Key);
                                ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
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
            if (ignoreGameMods || ignoreSelfConflicts)
            {
                foreach (var topConflict in conflictResult.Conflicts.GetHierarchicalDefinitions())
                {
                    foreach (var item in topConflict.Children.Where(p => p.Mods.Count <= 1))
                    {
                        if (ignoreGameMods && item.NonGameDefinitions <= 1 && !alreadyIgnored.Contains(item.Key))
                        {
                            alreadyIgnored.Add(item.Key);
                            ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
                        }
                        if (ignoreSelfConflicts && item.NonGameDefinitions > 1 && !alreadyIgnored.Contains(item.Key))
                        {
                            alreadyIgnored.Add(item.Key);
                            ruleIgnoredDefinitions.AddToMap(conflictResult.Conflicts.GetByTypeAndId(item.Key).First());
                        }
                    }
                }
            }
            conflictResult.RuleIgnoredConflicts = ruleIgnoredDefinitions;
        }

        /// <summary>
        /// Evals the wildcard.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="values">The content.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool EvalWildcard(string pattern, params string[] values)
        {
            foreach (var item in values)
            {
                if (pattern.Contains("*") || pattern.Contains("?"))
                {
                    var regex = $"^{Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".")}$";
                    if (Regex.IsMatch(item, regex, RegexOptions.IgnoreCase))
                    {
                        return true;
                    }
                }
                else
                {
                    if (item.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                        RootDirectory = GetPatchModDirectory(game, patchName),
                    });
                    await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = mod,
                        RootDirectory = game.UserDirectory,
                        Path = mod.DescriptorFile,
                        LockDescriptor = CheckIfModShouldBeLocked(game, mod)
                    }, IsPatchMod(mod));
                    allMods.Add(mod);
                    Cache.Invalidate(new CacheInvalidateParameters() { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
                }
                else
                {
                    mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
                }
                var definitionMod = allMods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                if (definitionMod != null || IsPatchMod(definitionMod))
                {
                    var args = new ModPatchExporterParameters()
                    {
                        Game = game.Type,
                        RootPath = GetModDirectoryRootPath(game),
                        PatchPath = EvaluatePatchNamePath(game, patchName)
                    };
                    var exportPatches = new HashSet<IDefinition>();
                    switch (exportType)
                    {
                        case ExportType.Ignored:
                            conflictResult.IgnoredConflicts.AddToMap(definition);
                            break;

                        case ExportType.Custom:
                            conflictResult.CustomConflicts.AddToMap(definition);
                            exportPatches.Add(definition);
                            args.CustomConflicts = PopulateModPath(exportPatches, GetCollectionMods(allMods));
                            break;

                        default:
                            conflictResult.ResolvedConflicts.AddToMap(definition);
                            exportPatches.Add(definition);
                            args.Definitions = PopulateModPath(exportPatches, GetCollectionMods(allMods));
                            break;
                    }

                    await Task.Run(() =>
                    {
                        ModWriter.ApplyModsAsync(new ModWriterParameters()
                        {
                            AppendOnly = true,
                            TopPriorityMods = new List<IMod>() { mod },
                            RootDirectory = game.UserDirectory
                        }).ConfigureAwait(false);
                    }).ConfigureAwait(false);

                    bool exportResult = false;
                    if (exportPatches.Count > 0)
                    {
                        exportResult = await modPatchExporter.ExportDefinitionAsync(args);
                        if (exportResult)
                        {
                            var overwritten = conflictResult.OverwrittenConflicts.GetByTypeAndId(definition.TypeAndId);
                            if (overwritten.Any() && overwritten.FirstOrDefault().DiskFileCI != definition.DiskFileCI)
                            {
                                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                                {
                                    RootDirectory = GetPatchModDirectory(game, patchName),
                                    Path = overwritten.FirstOrDefault().DiskFile
                                });
                            }
                        }
                    }

                    var stateResult = await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                        Mode = MapPatchStateMode(conflictResult.Mode),
                        IgnoreConflictPaths = conflictResult.IgnoredPaths,
                        Definitions = exportPatches,
                        Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                        CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                        RootPath = GetModDirectoryRootPath(game),
                        PatchPath = EvaluatePatchNamePath(game, patchName),
                        HasGameDefinitions = conflictResult.AllConflicts.HasGameDefinitions()
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
                if (ignored.Any() && !IsCachedDefinitionDifferent(matchedConflicts, item))
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
            var cachedDiffs = cachedConflicts.Where(p => currentConflicts.Any(a => a.FileCI.Equals(p.FileCI) && a.DefinitionSHA.Equals(p.DefinitionSHA)));
            return cachedDiffs.Count() != cachedConflicts.Count();
        }

        /// <summary>
        /// Determines whether [is overwritten type] [the specified value type].
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns><c>true</c> if [is overwritten type] [the specified value type]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsOverwrittenType(ValueType valueType)
        {
            return valueType == ValueType.OverwrittenObject || valueType == ValueType.OverWrittenObjectWithPreserveFileName;
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
                if (resolved.Any())
                {
                    files.AddRange(resolved.Select(p => p.File));
                    files.AddRange(resolved.Where(p => !string.IsNullOrWhiteSpace(p.DiskFile)).ToList().Select(p => p.DiskFile));
                }
            }

            return files;
        }

        /// <summary>
        /// synchronize patch state as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="resolvedConflicts">The resolved conflicts.</param>
        /// <param name="item">The item.</param>
        /// <param name="files">The files.</param>
        /// <param name="matchedConflicts">The matched conflicts.</param>
        protected virtual async Task SyncPatchStateAsync(IGame game, string patchName, List<IDefinition> resolvedConflicts, IGrouping<string, IDefinition> item, IList<string> files, IEnumerable<IDefinition> matchedConflicts)
        {
            var synced = await SyncPatchStatesAsync(matchedConflicts, item, patchName, game, files.ToArray());
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
        /// <param name="game">The game.</param>
        /// <param name="files">The files.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> SyncPatchStatesAsync(IEnumerable<IDefinition> currentConflicts, IEnumerable<IDefinition> cachedConflicts, string patchName, IGame game, params string[] files)
        {
            if (IsCachedDefinitionDifferent(currentConflicts, cachedConflicts))
            {
                foreach (var file in files.Distinct())
                {
                    await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = GetPatchModDirectory(game, patchName),
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
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="exportType">Type of the export.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> UnResolveConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName, ExportType exportType)
        {
            var game = GameService.GetSelected();
            var patchName = GenerateCollectionPatchName(collectionName);
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
                if (result.Any())
                {
                    IEnumerable<IMod> collectionMods = null;
                    foreach (var item in result)
                    {
                        indexed.Remove(item);
                        if (purgeFiles)
                        {
                            var patchModDirectory = GetPatchModDirectory(game, patchName);
                            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                            {
                                RootDirectory = patchModDirectory,
                                Path = item.File
                            });
                            if (!string.IsNullOrWhiteSpace(item.DiskFile) && item.File != item.DiskFile)
                            {
                                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                                {
                                    RootDirectory = patchModDirectory,
                                    Path = item.DiskFile
                                });
                            }
                            if (IsOverwrittenType(item.ValueType))
                            {
                                if (collectionMods == null)
                                {
                                    collectionMods = GetCollectionMods();
                                }
                                var overwritten = conflictResult.OverwrittenConflicts.GetByTypeAndId(typeAndId);
                                if (overwritten.Any())
                                {
                                    await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                                    {
                                        Game = game.Type,
                                        OverwrittenConflicts = PopulateModPath(overwritten.Where(p => !conflictResult.ResolvedConflicts.GetByTypeAndId(p.TypeAndId).Any()), collectionMods),
                                        RootPath = GetModDirectoryRootPath(game),
                                        PatchPath = EvaluatePatchNamePath(game, patchName)
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
                            RootDirectory = GetPatchModDirectory(game, patchName)
                        });
                        await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                        {
                            Mod = mod,
                            RootDirectory = game.UserDirectory,
                            Path = mod.DescriptorFile,
                            LockDescriptor = CheckIfModShouldBeLocked(game, mod)
                        }, IsPatchMod(mod));
                        allMods.Add(mod);
                        Cache.Invalidate(new CacheInvalidateParameters() { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
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
                        LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                        Mode = MapPatchStateMode(conflictResult.Mode),
                        IgnoreConflictPaths = conflictResult.IgnoredPaths,
                        Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                        IgnoredConflicts = GetDefinitionOrDefault(conflictResult.IgnoredConflicts),
                        OverwrittenConflicts = GetDefinitionOrDefault(conflictResult.OverwrittenConflicts),
                        CustomConflicts = GetDefinitionOrDefault(conflictResult.CustomConflicts),
                        RootPath = GetModDirectoryRootPath(game),
                        PatchPath = EvaluatePatchNamePath(game, patchName),
                        HasGameDefinitions = conflictResult.AllConflicts.HasGameDefinitions()
                    });
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<IDefinition> Validate(ParserArgs args)
        {
            return validateParser.Validate(args);
        }

        public IBracketValidateResult GetBracketCount(string text)
        {
            return validateParser.GetBracketCount(text);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class EvalState.
        /// </summary>
        private class EvalState
        {
            #region Properties

            /// <summary>
            /// Gets or sets the content sha.
            /// </summary>
            /// <value>The content sha.</value>
            public string ContentSha { get; set; }

            /// <summary>
            /// Gets or sets the name of the fall back file.
            /// </summary>
            /// <value>The name of the fall back file.</value>
            public string FallBackFileName { get; set; }

            /// <summary>
            /// Gets or sets the name of the file.
            /// </summary>
            /// <value>The name of the file.</value>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the name of the mod.
            /// </summary>
            /// <value>The name of the mod.</value>
            public string ModName { get; set; }

            #endregion Properties
        }

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
