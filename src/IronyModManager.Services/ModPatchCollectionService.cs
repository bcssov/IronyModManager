// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 02-09-2025
// ***********************************************************************
// <copyright file="ModPatchCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.Exceptions;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Configuration;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using Nito.AsyncEx;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Services
{
    /// <summary>
    /// The mod patch collection service.
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModPatchCollectionService" />
    public class ModPatchCollectionService(
        ICache cache,
        IMessageBus messageBus,
        IParserManager parserManager,
        IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
        IModPatchExporter modPatchExporter,
        IReader reader,
        IModWriter modWriter,
        IModParser modParser,
        IGameService gameService,
        IStorageProvider storageProvider,
        IMapper mapper,
        IValidateParser validateParser,
        IParametrizedParser parametrizedParser,
        IParserMerger parserMerger) : ModBaseService(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper), IModPatchCollectionService
    {
        #region Fields

        /// <summary>
        /// The mod exported key
        /// </summary>
        protected const string ModExportedKey = "ExportState";

        /// <summary>
        /// The mods exported region
        /// </summary>
        protected const string ModsExportedRegion = "ModExportedState";

        /// <summary>
        /// The cache region
        /// </summary>
        private const string CacheRegion = "CollectionPatchState";

        /// <summary>
        /// The ignore rules separator
        /// </summary>
        private const string IgnoreRulesSeparator = "--";

        /// <summary>
        /// The maximum allowed source
        /// </summary>
        private const double MaxAllowedSource = 1e+10;

        /// <summary>
        /// The maximum definitions to add
        /// </summary>
        private const int MaxDefinitionsToAdd = 8;

        /// <summary>
        /// The maximum mod conflicts to check
        /// </summary>
        private const int MaxModConflictsToCheck = 4;

        /// <summary>
        /// The maximum mods to process in parallel
        /// </summary>
        private const int MaxModsToProcessInParallel = 6;

        /// <summary>
        /// The mod name ignore counter identifier
        /// </summary>
        private const string ModNameIgnoreCounterId = "count:";

        /// <summary>
        /// The mod name ignore identifier
        /// </summary>
        private const string ModNameIgnoreId = "modName:";

        /// <summary>
        /// The ignore game mods identifier
        /// </summary>
        private const string ShowGameModsId = "--showGameMods";

        /// <summary>
        /// The show reset conflicts
        /// </summary>
        private const string ShowResetConflicts = "--showResetConflicts";

        /// <summary>
        /// The show self conflicts identifier
        /// </summary>
        private const string ShowSelfConflictsId = "--showSelfConflicts";

        /// <summary>
        /// The single file merged
        /// </summary>
        private const string SingleFileMerged = "irony_merged";

        /// <summary>
        /// The service lock
        /// </summary>
        private static readonly object serviceLock = new { };

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus = messageBus;

        /// <summary>
        /// The mod patch exporter
        /// </summary>
        private readonly IModPatchExporter modPatchExporter = modPatchExporter;

        /// <summary>
        /// The parametrized parser
        /// </summary>
        private readonly IParametrizedParser parametrizedParser = parametrizedParser;

        /// <summary>
        /// The parser manager
        /// </summary>
        private readonly IParserManager parserManager = parserManager;

        /// <summary>
        /// The parser merger
        /// </summary>
        private readonly IParserMerger parserMerger = parserMerger;

        /// <summary>
        /// The search initialize lock
        /// </summary>
        private readonly AsyncLock searchInitLock = new();

        /// <summary>
        /// The validate parser
        /// </summary>
        private readonly IValidateParser validateParser = validateParser;

        #endregion Fields

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
        public virtual void AddModsToIgnoreList(IConflictResult conflictResult, IEnumerable<IModIgnoreConfiguration> mods)
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
                        sb.AppendLine($"{ModNameIgnoreId}{item.ModName}{IgnoreRulesSeparator}{ModNameIgnoreCounterId}{(item.Count > 1 ? item.Count : 2)}");
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
                await DeleteDescriptorsInternalAsync([mod]);
            }

            return await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = GetPatchModDirectory(game, patchName) }, true);
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
            return modPatchExporter.CopyPatchModAsync(new ModPatchExporterParameters
            {
                RootPath = modDirRootPath,
                ModPath = EvaluatePatchNamePath(game, oldPatchName, modDirRootPath),
                PatchPath = EvaluatePatchNamePath(game, newPatchName, modDirRootPath),
                RenamePairs = [new KeyValuePair<string, string>(oldPatchName, newPatchName)]
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
                patch.UseSimpleValidation = false;
                patch.ModName = GenerateCollectionPatchName(collectionName);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patch.ModName) });
                if (state != null && state.IndexedConflictHistory.Any() && state.IndexedConflictHistory.TryGetValue(copy.TypeAndId, out var value))
                {
                    var history = value.FirstOrDefault();
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
        /// Evaluate the definition priority.
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
        /// <param name="allowedLanguages">The allowed languages.</param>
        /// <returns>IConflictResult.</returns>
        public virtual async Task<IConflictResult> FindConflictsAsync(IIndexedDefinitions indexedDefinitions, IList<string> modOrder, PatchStateMode patchStateMode, IReadOnlyCollection<IGameLanguage> allowedLanguages)
        {
            var actualMode = patchStateMode switch
            {
                PatchStateMode.ReadOnly => PatchStateMode.Advanced,
                PatchStateMode.ReadOnlyWithoutLocalization => PatchStateMode.AdvancedWithoutLocalization,
                _ => patchStateMode
            };

            // We need to filter out merge types
            var allIndexed = DIResolver.Get<IIndexedDefinitions>();
            var allDefs = (await indexedDefinitions.GetAllAsync()).ToList();
            await allIndexed.InitMapAsync(allDefs);
            indexedDefinitions.Dispose();
            indexedDefinitions = DIResolver.Get<IIndexedDefinitions>();
            await indexedDefinitions.InitMapAsync(allDefs.Where(p => p.MergeType == MergeType.None));

            var conflicts = new HashSet<IDefinition>();
            var fileKeys = await indexedDefinitions.GetAllFileKeysAsync();
            var typeAndIdKeys = await indexedDefinitions.GetAllTypeAndIdKeysAsync();
            var overwritten = (await indexedDefinitions.GetByValueTypeAsync(ValueType.OverwrittenObject)).Concat(await indexedDefinitions.GetByValueTypeAsync(ValueType.OverwrittenObjectSingleFile));
            var empty = await indexedDefinitions.GetByValueTypeAsync(ValueType.EmptyFile);
            var allCount = (await indexedDefinitions.GetAllAsync()).Count();
            var game = GameService.GetSelected();

            double total = (allCount * 2) + typeAndIdKeys.Count() + (overwritten.GroupBy(p => p.TypeAndId).Count() * 2) + empty.Count();
            double processed = 0;
            double previousProgress = 0;
            await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(0));

            // Cheers to the guys at paradox for doing a great job at implementing such a thing
            var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
            if (provider!.SupportsScriptMerge && allCount > 0)
            {
                var merges = provider.MergeTypes;
                foreach (var merge in merges)
                {
                    // Later put into a method... if we ever expand on merge types
                    var indexed = await allIndexed.GetByMergeTypeAsync(merge.Key);
                    var flatMerge = indexed.Where(p => p.MergeType == merge.Key).GroupBy(p => p.TypeAndId).ToList();
                    foreach (var item in flatMerge)
                    {
                        var types = await allIndexed.GetByTypeAndIdAsync(item.FirstOrDefault().TypeAndId);
                        var ordered = EvalDefinitionPriority(types.OrderBy(x => modOrder.IndexOf(x.ModName))).DefinitionOrder.ToList();
                        foreach (var definition in types.Where(p => p.MergeType == MergeType.None))
                        {
                            var id = ordered.IndexOf(definition);
                            var before = ordered.Take(id).Where(p => p.MergeType == merge.Key).Reverse();
                            var after = ordered.Skip(id + 1).Where(p => p.MergeType == merge.Key);
                            foreach (var val in merge.Value)
                            {
                                if (before.Any())
                                {
                                    foreach (var def in before)
                                    {
                                        definition.Code = definition.OriginalCode = parserMerger.MergeTopLevel(definition.Code.SplitOnNewLine(), definition.File, val, def.Code.SplitOnNewLine(), false);
                                    }
                                }

                                if (after.Any())
                                {
                                    foreach (var def in after)
                                    {
                                        definition.Code = definition.OriginalCode = parserMerger.MergeTopLevel(definition.Code.SplitOnNewLine(), definition.File, val, def.Code.SplitOnNewLine(), true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Yes, run a cleanup here
            allIndexed.Dispose();
            allIndexed = null;
            allDefs.Clear();
            allDefs = null;
            GCRunner.RunGC(GCCollectionMode.Optimized, false);

            // Handling inlines here now as game now uses these a lot -- Thanks pdx again
            allDefs = (await indexedDefinitions.GetAllAsync()).ToList();

            // Stellaris only (so far)
            total += provider!.SupportsInlineScripts ? allDefs.Count : 0;
            previousProgress = 0;
            List<IDefinition> prunedInlineDefinitions;
            if (provider.SupportsInlineScripts)
            {
                var tempIndex = DIResolver.Get<IIndexedDefinitions>();
                await tempIndex.InitMapAsync(allDefs);
                var scriptedVars = await tempIndex.GetByParentDirectoryAsync(provider.GlobalVariablesPath);
                prunedInlineDefinitions = [];
                var reportedInlineErrors = new HashSet<string>();
                foreach (var item in allDefs)
                {
                    var def = item;
                    var addDefault = true;
                    while ((def.Id.Equals(Parser.Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase) || def.ContainsInlineIdentifier) && !def.File.StartsWith(provider.InlineScriptsPath))
                    {
                        addDefault = false;
                        var path = Path.Combine(Parser.Common.Constants.Stellaris.InlineScripts, parametrizedParser.GetScriptPath(def.Code));
                        var pathCI = path.ToLowerInvariant();
                        var files = (await tempIndex.GetByParentDirectoryAsync(Path.GetDirectoryName(path))).Where(p => Path.Combine(Path.GetDirectoryName(p.FileCI)!, Path.GetFileNameWithoutExtension(p.FileCI)!).Equals(pathCI)).ToList();
                        if (files.Count != 0)
                        {
                            var priorityDefinition = EvalDefinitionPriority([.. files.OrderBy(p => modOrder.IndexOf(p.ModName))]);
                            if (priorityDefinition is { Definition: not null })
                            {
                                var vars = new List<IDefinition>();
                                if (item.Variables != null)
                                {
                                    vars.AddRange(item.Variables);
                                }

                                scriptedVars = scriptedVars?.GroupBy(p => p.Id).Select(p => EvalDefinitionPriority(p.OrderBy(f => modOrder.IndexOf(f.ModName))).Definition);

                                var parametrizedCode = parametrizedParser.Process(priorityDefinition.Definition.Code, ProcessInlineConstants(def.Code, vars, scriptedVars, out var ids));
                                if (!string.IsNullOrWhiteSpace(parametrizedCode))
                                {
                                    var validationType = ValidationType.Full;
                                    if (item.UseSimpleValidation.GetValueOrDefault() || item.UseSimpleValidation == null)
                                    {
                                        validationType = MapValidationType(item);
                                    }
                                    else if (priorityDefinition.Definition.UseSimpleValidation.GetValueOrDefault() || priorityDefinition.Definition.UseSimpleValidation == null)
                                    {
                                        validationType = MapValidationType(priorityDefinition.Definition);
                                    }

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
                                        ValidationType = ValidationType.SkipAll // First skip all validation types due to nth level
                                    });
                                    if (item.Variables != null && item.Variables.Any() && results != null)
                                    {
                                        MergeDefinitions(results.Concat(item.Variables));
                                    }

                                    if (results != null && results.Any())
                                    {
                                        var inline = results.FirstOrDefault(p => p.Id.Equals(Parser.Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase) || p.ContainsInlineIdentifier);

                                        var globalVars = new List<IDefinition>();
                                        if (def is { AppliedGlobalVariables: not null } && def.AppliedGlobalVariables.Any())
                                        {
                                            globalVars.AddRange(def.AppliedGlobalVariables);
                                        }

                                        if (ids != null && ids.Any())
                                        {
                                            globalVars.AddRange(ids);
                                        }

                                        globalVars = globalVars.GroupBy(p => p.TypeAndId).Select(p => p.FirstOrDefault()).ToList();

                                        if (inline == null)
                                        {
                                            results = parserManager.Parse(new ParserManagerArgs
                                            {
                                                ContentSHA = item.ContentSHA, // Want original file sha id
                                                File = item.File, // To trigger right parser
                                                GameType = game.Type,
                                                Lines = parametrizedCode.SplitOnNewLine(),
                                                FileLastModified = item.LastModified,
                                                ModDependencies = item.Dependencies,
                                                IsBinary = item.ValueType == ValueType.Binary,
                                                ModName = item.ModName,
                                                ValidationType =
                                                    validationType // This is kinda difficult but try to guess which validation type we want to inherit. This was previously done outside, but now we need to re-run and re-validate.
                                            });
                                            if (item.Variables != null && item.Variables.Any() && results != null)
                                            {
                                                MergeDefinitions(results.Concat(item.Variables));
                                            }

                                            if (results != null && results.Any())
                                            {
                                                foreach (var definition in results)
                                                {
                                                    definition.AppliedGlobalVariables = globalVars;
                                                }

                                                prunedInlineDefinitions.AddRange(results);
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            def = inline;
                                            def.AppliedGlobalVariables = globalVars;
                                        }
                                    }
                                    else if (!reportedInlineErrors.Contains($"{item.ModName} - {pathCI}"))
                                    {
                                        // Could happen, will need manually investigation though
                                        var copy = CopyDefinition(item);
                                        copy.ValueType = ValueType.Invalid;
                                        copy.ErrorMessage = $"Inline script {path} failed to be processed. Please report to the author of Irony.";
                                        prunedInlineDefinitions.Add(copy);
                                        reportedInlineErrors.Add($"{item.ModName} - {pathCI}");
                                    }
                                }
                            }
                        }
                        else if (!reportedInlineErrors.Contains($"{item.ModName} - {pathCI}"))
                        {
                            // Need to report missing inline script
                            var copy = CopyDefinition(item);
                            copy.ValueType = ValueType.Invalid;
                            copy.ErrorMessage = $"Inline script {path} is not found in any mods.{Environment.NewLine}{Environment.NewLine}It is possible that the file is missing or due to a syntax error Irony cannot find it.";
                            prunedInlineDefinitions.Add(copy);
                            reportedInlineErrors.Add($"{item.ModName} - {pathCI}");
                        }
                    }

                    if (addDefault)
                    {
                        prunedInlineDefinitions.Add(item);
                    }

                    processed++;
                    var perc = GetProgressPercentage(total, processed, 100);
                    if (perc.IsNotNearlyEqual(previousProgress))
                    {
                        await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                        previousProgress = perc;
                    }
                }

                tempIndex.Dispose();
            }
            else
            {
                prunedInlineDefinitions = [.. allDefs];
            }

            // Run another cleanup
            indexedDefinitions.Dispose();
            indexedDefinitions = null;
            allDefs.Clear();
            allDefs = null;

            indexedDefinitions = DIResolver.Get<IIndexedDefinitions>();
            await indexedDefinitions.InitMapAsync(prunedInlineDefinitions);

            prunedInlineDefinitions.Clear();
            prunedInlineDefinitions = null;
            GCRunner.RunGC(GCCollectionMode.Optimized, false);

            // Redeclare stuff
            fileKeys = await indexedDefinitions.GetAllFileKeysAsync();
            typeAndIdKeys = await indexedDefinitions.GetAllTypeAndIdKeysAsync();
            overwritten = (await indexedDefinitions.GetByValueTypeAsync(ValueType.OverwrittenObject)).Concat(await indexedDefinitions.GetByValueTypeAsync(ValueType.OverwrittenObjectSingleFile));
            empty = await indexedDefinitions.GetByValueTypeAsync(ValueType.EmptyFile);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // Create virtual empty objects from an empty file
            foreach (var item in empty)
            {
                var emptyConflicts = await indexedDefinitions.GetByFileAsync(item.File);
                if (emptyConflicts.Any())
                {
                    foreach (var emptyConflict in emptyConflicts.Where(p => p.ValueType != ValueType.Invalid && !p.ModName.Equals(item.ModName)))
                    {
                        var copy = (await indexedDefinitions.GetByTypeAndIdAsync(emptyConflict.TypeAndId)).FirstOrDefault(p => p.ModName.Equals(item.ModName));
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
                            copy.IsFromGame = item.IsFromGame;
                            copy.FileNameSuffix = item.FileNameSuffix;
                            await indexedDefinitions.AddToMapAsync(copy);
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
                var perc = GetProgressPercentage(total, processed, 99.99);
                if (perc.IsNotNearlyEqual(previousProgress))
                {
                    await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            Debug.WriteLine("FindConflictsAsync Empty Files Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            var opLock = new AsyncLock();
            var tasks = fileKeys.GroupBy(Path.GetDirectoryName).Select(dir =>
            {
                return Task.Run(async () =>
                {
                    var localConflicts = new HashSet<IDefinition>();
                    var localFileConflictCache = new Dictionary<string, bool>();
                    var modShaConflictCache = new Dictionary<string, List<string>>();
                    foreach (var item in dir)
                    {
                        var definitions = await indexedDefinitions.GetByFileAsync(item);
                        await EvalDefinitionsAsync(indexedDefinitions, localConflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, actualMode, localFileConflictCache, modShaConflictCache);
                        var progressMutex = await opLock.LockAsync();
                        processed += definitions.Count();
                        var perc = GetProgressPercentage(total, processed, 99.99);
                        if (perc.IsNotNearlyEqual(previousProgress))
                        {
                            await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                            previousProgress = perc;
                        }

                        progressMutex.Dispose();
                    }

                    var syncMutex = await opLock.LockAsync();
                    foreach (var item in localConflicts)
                    {
                        conflicts.Add(item);
                    }

                    syncMutex.Dispose();
                });
            });
            await Task.WhenAll(tasks);

            Debug.WriteLine("FindConflictsAsync All Files Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            tasks = typeAndIdKeys.GroupBy(Path.GetDirectoryName).Select(type =>
            {
                return Task.Run(async () =>
                {
                    var localConflicts = new HashSet<IDefinition>();
                    var localFileConflictCache = new Dictionary<string, bool>();
                    var modShaConflictCache = new Dictionary<string, List<string>>();
                    foreach (var item in type)
                    {
                        var definitions = await indexedDefinitions.GetByTypeAndIdAsync(item);
                        await EvalDefinitionsAsync(indexedDefinitions, localConflicts, definitions.OrderBy(p => modOrder.IndexOf(p.ModName)), modOrder, actualMode, localFileConflictCache, modShaConflictCache);
                        var progressMutex = await opLock.LockAsync();
                        processed += definitions.Count();
                        var perc = GetProgressPercentage(total, processed, 99.99);
                        if (perc.IsNotNearlyEqual(previousProgress))
                        {
                            await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                            previousProgress = perc;
                        }

                        progressMutex.Dispose();
                    }

                    var syncMutex = await opLock.LockAsync();
                    foreach (var item in localConflicts)
                    {
                        conflicts.Add(item);
                    }

                    syncMutex.Dispose();
                });
            });
            await Task.WhenAll(tasks);

            Debug.WriteLine("FindConflictsAsync All Definitions Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            var indexedConflicts = DIResolver.Get<IIndexedDefinitions>();
            await indexedConflicts.InitMapAsync(conflicts);

            tasks = typeAndIdKeys.GroupBy(Path.GetDirectoryName).Select(type =>
            {
                return Task.Run(async () =>
                {
                    var cache = new Dictionary<string, IDefinition>();
                    foreach (var typeId in type)
                    {
                        var typeLock = await opLock.LockAsync();
                        var items = await indexedConflicts.GetByTypeAndIdAsync(typeId);
                        typeLock.Dispose();
                        if (items.Any() && items.All(p => !p.ExistsInLastFile))
                        {
                            IDefinition lastMod;
                            if (cache.TryGetValue(items.FirstOrDefault()!.FileCI, out var value))
                            {
                                lastMod = value;
                            }
                            else
                            {
                                var fileLock = await opLock.LockAsync();
                                var fileDefs = await indexedDefinitions.GetByFileAsync(items.FirstOrDefault()!.FileCI);
                                fileLock.Dispose();
                                lastMod = fileDefs.GroupBy(p => p.ModName).Select(p => p.First()).MaxBy(p => modOrder.IndexOf(p.ModName));
                                cache[items.FirstOrDefault()!.FileCI] = lastMod;
                            }

                            var copy = CopyDefinition(items.FirstOrDefault());
                            copy.Dependencies = lastMod!.Dependencies;
                            copy.ModName = lastMod.ModName;
                            copy.Code = copy.OriginalCode = Comments.GetEmptyCommentType(copy.File);
                            copy.ContentSHA = lastMod.ContentSHA;
                            copy.UsedParser = lastMod.UsedParser;
                            copy.CodeSeparator = lastMod.CodeSeparator;
                            copy.CodeTag = lastMod.CodeTag;
                            copy.OriginalModName = lastMod.OriginalModName;
                            copy.OriginalFileName = lastMod.OriginalFileName;
                            copy.Variables = lastMod.Variables;
                            var fileNames = copy.AdditionalFileNames;
                            foreach (var fileName in lastMod.AdditionalFileNames)
                            {
                                fileNames.Add(fileName);
                            }

                            copy.AdditionalFileNames = fileNames;
                            copy.ExistsInLastFile = true;
                            copy.IsFromGame = lastMod.IsFromGame;
                            copy.LastModified = null;
                            var mutex = await opLock.LockAsync();
                            await indexedConflicts.AddToMapAsync(copy);
                            await indexedDefinitions.AddToMapAsync(copy);
                            conflicts.Add(copy);
                            processed++;
                            var perc = GetProgressPercentage(total, processed, 99.99);
                            if (perc.IsNotNearlyEqual(previousProgress))
                            {
                                await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                                previousProgress = perc;
                            }

                            mutex.Dispose();
                        }
                        else
                        {
                            var mutex = await opLock.LockAsync();
                            processed++;
                            var perc = GetProgressPercentage(total, processed, 99.99);
                            if (perc.IsNotNearlyEqual(previousProgress))
                            {
                                await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                                previousProgress = perc;
                            }

                            mutex.Dispose();
                        }
                    }
                });
            });
            await Task.WhenAll(tasks);

            Debug.WriteLine("FindConflictsAsync All Definitions Exist in Last File Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            var overwrittenDefs = new Dictionary<string, Tuple<IDefinition, IEnumerable<IDefinition>, IDefinition>>();
            var overwrittenSort = new Dictionary<string, IEnumerable<DefinitionOrderSort>>();
            var overwrittenSortExport = new Dictionary<string, List<IDefinition>>();

            foreach (var item in overwritten.GroupBy(p => p.TypeAndId))
            {
                if (!overwrittenSort.TryGetValue(item.FirstOrDefault()!.ParentDirectoryCI, out var value))
                {
                    var all = (await indexedDefinitions.GetByParentDirectoryAsync(item.FirstOrDefault()!.ParentDirectoryCI)).Where(IsValidDefinitionType);
                    var ordered = all.GroupBy(p => p.TypeAndId).Select(p =>
                    {
                        var partialCopy = new List<IDefinition>();
                        p.ToList().ForEach(x => partialCopy.Add(PartialDefinitionCopy(x, false)));
                        var priority = EvalDefinitionPriorityInternal(partialCopy.OrderBy(x => modOrder.IndexOf(x.ModName)), true);
                        return new DefinitionOrderSort { TypeAndId = priority.Definition.TypeAndId, Order = priority.Definition.Order, File = Path.GetFileNameWithoutExtension(priority.FileName) };
                    }).GroupBy(p => p.File).OrderBy(p => p.Key, StringComparer.Ordinal).SelectMany(p => p.OrderBy(x => x.Order)).ToList();
                    var fullyOrdered = ordered.Select(p => new DefinitionOrderSort { TypeAndId = p.TypeAndId, Order = ordered.IndexOf(p) }).ToList();
                    value = fullyOrdered;
                    overwrittenSort.Add(item.FirstOrDefault()!.ParentDirectoryCI, value);
                }

                var conflicted = await indexedConflicts.GetByTypeAndIdAsync(item.First().TypeAndId);
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
                        var hasOverrides = all.Any(p => !p.IsCustomPatch && p.Dependencies != null && p.Dependencies.Any(s => s.Equals(def.ModName)));
                        if (!hasOverrides || actualMode == PatchStateMode.Advanced)
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

                    // ReSharper disable once InlineTemporaryVariable
                    var ordered = value;
                    newDefinition.Order = ordered.FirstOrDefault(p => p.TypeAndId == newDefinition.TypeAndId)!.Order;
                    if (!overwrittenSortExport.TryGetValue(newDefinition.ParentDirectoryCI, out var valueInner))
                    {
                        overwrittenSortExport.Add(newDefinition.ParentDirectoryCI, [newDefinition]);
                    }
                    else
                    {
                        valueInner.Add(newDefinition);
                    }

                    overwrittenDefs.Add(definition.TypeAndId, Tuple.Create(newDefinition, definitions, definition));
                }

                processed++;
                var perc = GetProgressPercentage(total, processed, 99.99);
                if (perc.IsNotNearlyEqual(previousProgress))
                {
                    await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                    previousProgress = perc;
                }
            }

            Debug.WriteLine("FindConflictsAsync Overwritten Objects Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            tasks = overwrittenSortExport.Select(sortExport =>
            {
                return Task.Run(async () =>
                {
                    var fullySortedExport = sortExport.Value.OrderBy(p => p.Order).ToList();
                    foreach (var newDefinition in sortExport.Value)
                    {
                        var definitions = overwrittenDefs[newDefinition.TypeAndId].Item2;
                        var definition = overwrittenDefs[newDefinition.TypeAndId].Item3;
                        newDefinition.Order = fullySortedExport.IndexOf(newDefinition) + 1;
                        var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(GameService.GetSelected().Type) && p.IsFullyImplemented);
                        var oldFileName = newDefinition.File;
                        newDefinition.File = Path.Combine(definition.ParentDirectory, definition.Id.GenerateValidFileName() + Path.GetExtension(definition.File));
                        var overwrittenFileNames = definition.OverwrittenFileNames;
                        foreach (var file in definitions.SelectMany(p => p.OverwrittenFileNames))
                        {
                            overwrittenFileNames.Add(file);
                        }

                        newDefinition.OverwrittenFileNames = overwrittenFileNames.Distinct().ToList();
                        newDefinition.DiskFile = provider!.GetDiskFileName(newDefinition);
                        var preserveOverwrittenFileName = oldFileName == newDefinition.File;
                        newDefinition.File = provider.GetFileName(newDefinition);
                        if (preserveOverwrittenFileName)
                        {
                            // What are the chances that generated filename will match
                            var preservedOverwrittenFileName = newDefinition.OverwrittenFileNames;
                            preservedOverwrittenFileName.Add(oldFileName);
                            newDefinition.OverwrittenFileNames = preservedOverwrittenFileName;
                        }

                        var mutex = await opLock.LockAsync();
                        processed++;
                        var perc = GetProgressPercentage(total, processed, 99.99);
                        if (perc.IsNotNearlyEqual(previousProgress))
                        {
                            await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(perc));
                            previousProgress = perc;
                        }

                        mutex.Dispose();
                    }
                });
            });
            await Task.WhenAll(tasks);

            Debug.WriteLine("FindConflictsAsync Overwritten Objects Sort Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(99.99));
            var groupedConflicts = conflicts.GroupBy(p => p.TypeAndId);
            var filteredConflicts = new List<IDefinition>();
            foreach (var conflict in groupedConflicts.Where(p => p.Count() > 1))
            {
                if (conflict.Any(p => p.IsPlaceholder))
                {
                    // Ignore all placeholders
                    if (!conflict.All(p => p.IsPlaceholder))
                    {
                        var priority = EvalDefinitionPriority(conflict.OrderBy(m => modOrder.IndexOf(m.ModName)));

                        // If priority definition is placeholder then we need to show this as the mod author messed up or Irony messed up
                        if (priority.Definition.IsPlaceholder)
                        {
                            var nonPlaceholders = conflict.Where(p => !p.IsPlaceholder).ToList();
                            nonPlaceholders.Add(priority.Definition);
                            if (nonPlaceholders.Count > 1)
                            {
                                filteredConflicts.AddRange(nonPlaceholders);
                            }
                        }
                        else
                        {
                            var nonPlaceholders = conflict.Where(p => !p.IsPlaceholder);
                            if (nonPlaceholders.Count() > 1)
                            {
                                filteredConflicts.AddRange(nonPlaceholders);
                            }
                        }
                    }
                }
                else
                {
                    filteredConflicts.AddRange(conflict);
                }
            }

            Debug.WriteLine("FindConflictsAsync Placeholder Valid Parse: " + stopWatch.Elapsed.FormatElapsed());
            stopWatch.Restart();

            var result = GetModelInstance<IConflictResult>();
            result.AllowedLanguages = allowedLanguages != null ? allowedLanguages.Select(l => l.Type).ToList() : [];
            result.Mode = patchStateMode;
            var conflictsIndexed = DIResolver.Get<IIndexedDefinitions>();
            await conflictsIndexed.InitMapAsync(filteredConflicts, true);
            result.AllConflicts = indexedDefinitions;
            result.Conflicts = conflictsIndexed;
            var resolvedConflicts = DIResolver.Get<IIndexedDefinitions>();
            await resolvedConflicts.InitMapAsync(null, true);
            result.ResolvedConflicts = resolvedConflicts;
            var ignoredConflicts = DIResolver.Get<IIndexedDefinitions>();
            await ignoredConflicts.InitMapAsync(null, true);
            result.IgnoredConflicts = ignoredConflicts;
            var ruleIgnoredDefinitions = DIResolver.Get<IIndexedDefinitions>();
            await ruleIgnoredDefinitions.InitMapAsync(null, true);
            result.RuleIgnoredConflicts = ruleIgnoredDefinitions;
            var overwrittenDefinitions = DIResolver.Get<IIndexedDefinitions>();
            await overwrittenDefinitions.InitMapAsync(overwrittenDefs.Select(a => a.Value.Item1));
            result.OverwrittenConflicts = overwrittenDefinitions;
            var customConflicts = DIResolver.Get<IIndexedDefinitions>();
            await customConflicts.InitMapAsync(null, true);
            result.CustomConflicts = customConflicts;
            await messageBus.PublishAsync(new ModDefinitionAnalyzeEvent(100));

            stopWatch.Stop();
            Debug.WriteLine("FindConflictsAsync Init Result: " + stopWatch.Elapsed.FormatElapsed());

            return result;
        }

        /// <summary>
        /// Gets an allowed languages async.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A Task containing IReadOnlyCollection of strings.<see cref="T:System.Threading.Tasks.Task`1" /></returns>
        public async Task<IReadOnlyCollection<string>> GetAllowedLanguagesAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                var @params = new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) };
                var languages = await modPatchExporter.GetAllowedLanguagesAsync(@params);
                if (languages != null)
                {
                    return languages;
                }
                else
                {
                    var state = await modPatchExporter.GetPatchStateAsync(@params, false);
                    if (state != null)
                    {
                        return state.AllowedLanguages != null ? [.. state.AllowedLanguages] : [];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the bracket count.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns>IBracketValidateResult.</returns>
        public virtual IBracketValidateResult GetBracketCount(string file, string text)
        {
            return validateParser.GetBracketCount(file, text);
        }

        /// <summary>
        /// Gets the ignored mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        public virtual IReadOnlyList<IModIgnoreConfiguration> GetIgnoredMods(IConflictResult conflictResult)
        {
            var mods = new List<IModIgnoreConfiguration>();
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var lines = ignoredPaths.SplitOnNewLine().Where(p => !p.Trim().StartsWith('#'));
                foreach (var line in lines)
                {
                    var ignoreMod = ParseIgnoreModLine(line);
                    if (ignoreMod != null)
                    {
                        mods.Add(ignoreMod);
                    }
                }
            }

            return mods;
        }

        /// <summary>
        /// Get mod objects as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="allowedGameLanguages">The allowed game languages.</param>
        /// <returns>A Task&lt;IIndexedDefinitions&gt; representing the asynchronous operation.</returns>
        /// <exception cref="ModTooLargeException">Detected a mod which is potentially too large to parse.</exception>
        /// <exception cref="IronyModManager.Services.Common.Exceptions.ModTooLargeException">Detected a mod which is potentially too large to parse.</exception>
        public virtual async Task<IIndexedDefinitions> GetModObjectsAsync(IGame game, IEnumerable<IMod> mods, string collectionName, PatchStateMode mode, IReadOnlyCollection<IGameLanguage> allowedGameLanguages)
        {
            if (game == null || mods == null || !mods.Any())
            {
                return null;
            }

            var tooLargeMod = false;
            mods.AsParallel().WithDegreeOfParallelism(MaxModsToProcessInParallel).ForAll(m =>
            {
                if (tooLargeMod)
                {
                    return;
                }

                var size = Reader.GetTotalSize(m.FullPath, Shared.Constants.TextExtensions);
                if (size > MaxAllowedSource)
                {
                    tooLargeMod = true;
                }
            });

            if (tooLargeMod)
            {
                throw new ModTooLargeException("Detected a mod which is potentially too large to parse.");
            }

            var definitions = new ConcurrentBag<IDefinition>();

            double processed = 0;
            double total = mods.Count();
            double previousProgress = 0;

            // Don't need full implementation just BOM check
            var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
            await messageBus.PublishAsync(new ModDefinitionLoadEvent(0));

            var allowedLanguages = allowedGameLanguages;
            var gameFolders = game.GameFolders.ToList();
            if (mode is PatchStateMode.DefaultWithoutLocalization or PatchStateMode.AdvancedWithoutLocalization or PatchStateMode.ReadOnlyWithoutLocalization)
            {
                gameFolders = gameFolders.Where(p => !p.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase)).ToList();

                // Mode override
                allowedGameLanguages = null;
            }

            mods.AsParallel().WithDegreeOfParallelism(MaxModsToProcessInParallel).WithExecutionMode(ParallelExecutionMode.ForceParallelism).ForAll(m =>
            {
                var result = ParseModFiles(game, Reader.Read(m.FullPath, gameFolders), m, provider, allowedLanguages);
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
                    if (perc.IsNotNearlyEqual(previousProgress))
                    {
                        messageBus.Publish(new ModDefinitionLoadEvent(perc));
                        previousProgress = perc;
                    }

                    GCRunner.RunGC(GCCollectionMode.Optimized);
                }
            });

            await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(0));
            processed = 0;
            total = definitions.Count;
            previousProgress = 0;

            List<IDefinition> prunedDefinitions;
            List<IDefinition> definitionsCopy = [.. definitions];
            var patchName = GenerateCollectionPatchName(collectionName);
            var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) });

            definitions.Clear();
            definitions = null;
            if (state != null && state.CustomConflicts.Any())
            {
                prunedDefinitions = [];
                var customIndexed = DIResolver.Get<IIndexedDefinitions>();
                await customIndexed.InitMapAsync(state.CustomConflicts);
                foreach (var item in definitionsCopy)
                {
                    var addDefault = true;
                    if (item.ValueType == ValueType.Invalid)
                    {
                        var fileCodes = await customIndexed.GetByFileAsync(item.File);
                        if (fileCodes.Any())
                        {
                            var fileDefs = new List<IDefinition>();
                            foreach (var fileCode in fileCodes)
                            {
                                if (state.IndexedConflictHistory != null && state.IndexedConflictHistory.Any() && state.IndexedConflictHistory.TryGetValue(fileCode.TypeAndId, out var value))
                                {
                                    var history = value.FirstOrDefault();
                                    if (history != null && !string.IsNullOrWhiteSpace(history.Code))
                                    {
                                        fileDefs.AddRange(parserManager.Parse(new ParserManagerArgs
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
                            }

                            if (fileDefs.Count != 0)
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
                    if (perc.IsNotNearlyEqual(previousProgress))
                    {
                        await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(perc));
                        previousProgress = perc;
                    }
                }
            }
            else
            {
                prunedDefinitions = [.. definitionsCopy];
            }

            definitionsCopy.Clear();
            definitionsCopy = null;

            await messageBus.PublishAsync(new ModDefinitionInvalidReplaceEvent(99.9));
            var indexed = DIResolver.Get<IIndexedDefinitions>();
            await indexed.InitMapAsync(prunedDefinitions);

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
                var @params = new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) };
                var mode = await modPatchExporter.GetPatchStateModeAsync(@params);
                if (mode.HasValue)
                {
                    return MapPatchStateMode(mode.GetValueOrDefault());
                }
                else
                {
                    var state = await modPatchExporter.GetPatchStateAsync(@params, false);
                    if (state != null)
                    {
                        return MapPatchStateMode(state.Mode);
                    }
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
            var allowCleanup = conflictResult != null && conflictResult.Mode != PatchStateMode.ReadOnly && conflictResult.Mode != PatchStateMode.ReadOnlyWithoutLocalization;

            async Task cleanSingleMergeFiles(string directory, string patchName)
            {
                if (!allowCleanup)
                {
                    return;
                }

                var patchModDir = GetPatchModDirectory(game, patchName);
                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = patchModDir, Path = directory });
            }

            async Task<int> syncPatchFiles(IConflictResult conflicts, IEnumerable<string> patchFiles, string patchName, int total, int processed, int maxProgress)
            {
                var patchModDir = GetPatchModDirectory(game, patchName);
                foreach (var file in patchFiles.Distinct())
                {
                    if (allowCleanup)
                    {
                        var cleaned = false;

                        // Skip single file overwrites as they are cleaned at the beginning of the process
                        var folderConflicts = await conflicts.AllConflicts.GetByParentDirectoryAsync(Path.GetDirectoryName(file));
                        if (folderConflicts.Any() && folderConflicts.Any(p => p.ValueType == ValueType.OverwrittenObjectSingleFile))
                        {
                            continue;
                        }

                        if (!await conflicts.CustomConflicts.ExistsByFileAsync(file) &&
                            !await conflicts.OverwrittenConflicts.ExistsByFileAsync(file) &&
                            !await conflicts.ResolvedConflicts.ExistsByFileAsync(file))
                        {
                            cleaned = await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = patchModDir, Path = file });
                        }

                        if (!cleaned)
                        {
                            var resolved = await conflicts.ResolvedConflicts.GetByDiskFileAsync(file);
                            if (resolved.Any())
                            {
                                var overwritten = await conflicts.OverwrittenConflicts.GetByTypeAndIdAsync(resolved.FirstOrDefault()!.TypeAndId);
                                if (overwritten.Any() && overwritten.FirstOrDefault()!.DiskFileCI != resolved.FirstOrDefault()!.DiskFileCI)
                                {
                                    await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = patchModDir, Path = overwritten.FirstOrDefault()!.DiskFile });
                                }
                            }
                        }
                    }

                    processed++;
                    var perc = GetProgressPercentage(total, processed, maxProgress);
                    if (previousProgress.IsNotNearlyEqual(perc))
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                        previousProgress = perc;
                    }
                }

                return processed;
            }

            async Task<(IIndexedDefinitions, int)> initAllIndexedDefinitions(IConflictResult conflictResult, int total, int processed, int maxProgress)
            {
                async void processedSearchItemHandler(object sender, ProcessedArgs args)
                {
                    using var mutex = await searchInitLock.LockAsync();
                    processed++;
                    var perc = GetProgressPercentage(total, processed, maxProgress);
                    if (previousProgress.IsNotNearlyEqual(perc))
                    {
                        await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                        previousProgress = perc;
                    }

                    // ReSharper disable once DisposeOnUsingVariable
                    mutex.Dispose();
                }

                var copy = DIResolver.Get<IIndexedDefinitions>();
                var options = DIResolver.Get<IDomainConfiguration>().GetOptions();
                var diskSearchPath = string.Empty;
                if (options.ConflictSolver.UseDiskSearch)
                {
                    diskSearchPath = StorageProvider.GetRootStoragePath();
                }

                copy.UseSearch(diskSearchPath, nameof(conflictResult.AllConflicts));
                if (options.ConflictSolver.UseHybridMemory)
                {
                    copy.UseDiskStore(StorageProvider.GetRootStoragePath());
                }

                copy.SetAllowedType(AddToMapAllowedType.InvalidAndSpecial);
                var semaphore = new AsyncSemaphore(MaxDefinitionsToAdd);
                var searchDefinitions = new List<IDefinition>();
                var tasks = (await conflictResult.AllConflicts.GetAllAsync()).Select(async item =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var defCopy = item;
                        if (item.ValueType == ValueType.Invalid || item.IsSpecialFolder)
                        {
                            defCopy = PartialDefinitionCopy(item);
                        }

                        await copy.AddToMapAsync(defCopy);
                        if (defCopy.Tags != null && defCopy.Tags.Any() && !defCopy.IsFromGame)
                        {
                            searchDefinitions.Add(defCopy);
                        }

                        processed++;
                        var perc = GetProgressPercentage(total, processed, maxProgress);
                        if (previousProgress.IsNotNearlyEqual(perc))
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                await Task.WhenAll(tasks);
                copy.ProcessedSearchItem += processedSearchItemHandler;
                await copy.InitializeSearchAsync(searchDefinitions);
                copy.ProcessedSearchItem -= processedSearchItemHandler;
                return (copy, processed);
            }

            if (game != null && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                double perc;
                var patchName = GenerateCollectionPatchName(collectionName);
                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(0));
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) });
                var patchFiles = modPatchExporter.GetPatchFiles(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) });
                foreach (var item in (await conflictResult.OverwrittenConflicts.GetAllAsync()).GroupBy(p => p.ParentDirectory))
                {
                    await cleanSingleMergeFiles(item.First().ParentDirectory, patchName);
                }

                var all = await conflictResult.AllConflicts.GetAllAsync();
                var total = patchFiles.Count() + all.Count() + all.Count(p => p.Tags != null && p.Tags.Any() && !p.IsFromGame); // Other all is the trie init counter
                if (state != null)
                {
                    var resolvedConflicts = new List<IDefinition>(state.ResolvedConflicts);
                    var ignoredConflicts = new List<IDefinition>();
                    total += state.Conflicts.Count() + (state.OverwrittenConflicts.Count() * 2) + 1;
                    var processed = 0;
                    foreach (var item in state.Conflicts.GroupBy(p => p.TypeAndId))
                    {
                        var files = ProcessPatchStateFiles(state, item, ref processed);
                        var matchedConflicts = await FindPatchStateMatchedConflictsAsync(conflictResult.Conflicts, state, ignoredConflicts, item);
                        await SyncPatchStateAsync(game, patchName, resolvedConflicts, item, files, matchedConflicts, !allowCleanup);
                        perc = GetProgressPercentage(total, processed);
                        if (previousProgress.IsNotNearlyEqual(perc))
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

                        var matchedConflicts = await conflictResult.OverwrittenConflicts.GetByTypeAndIdAsync(item.First().TypeAndId);
                        await SyncPatchStatesAsync(matchedConflicts, item, patchName, game, !allowCleanup, [.. files]);
                        perc = GetProgressPercentage(total, processed);
                        if (previousProgress.IsNotNearlyEqual(perc))
                        {
                            await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                            previousProgress = perc;
                        }
                    }

                    var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                    await resolvedIndex.InitMapAsync(resolvedConflicts, true);

                    if ((await conflictResult.OverwrittenConflicts.GetAllAsync()).Any())
                    {
                        var alreadyMergedTypes = new HashSet<string>();
                        var overwrittenConflicts = PopulateModPath(await conflictResult.OverwrittenConflicts.GetAllAsync(), GetCollectionMods());
                        foreach (var item in overwrittenConflicts)
                        {
                            var definition = item;
                            var resolved = await resolvedIndex.GetByTypeAndIdAsync(definition.TypeAndId);
                            if (resolved.Any())
                            {
                                definition = resolved.FirstOrDefault();
                                definition!.Order = item.Order;
                                definition.DiskFile = item.DiskFile;
                                definition.File = item.File;
                                definition.OverwrittenFileNames = item.OverwrittenFileNames;
                                if (state.IndexedConflictHistory != null && state.IndexedConflictHistory.Any() && state.IndexedConflictHistory.TryGetValue(definition.TypeAndId, out var value))
                                {
                                    var history = value.FirstOrDefault();
                                    if (history != null)
                                    {
                                        definition.Code = history.Code;
                                    }
                                }
                            }

                            var canExport = true;
                            if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                            {
                                if (!alreadyMergedTypes.Contains(definition.Type))
                                {
                                    var merged = await ProcessOverwrittenSingleFileDefinitionsAsync(conflictResult, patchName, definition.Type, Tuple.Create(state, resolvedIndex));
                                    if (merged != null)
                                    {
                                        definition = PopulateModPath(merged, GetCollectionMods()).FirstOrDefault();
                                        alreadyMergedTypes.Add(definition!.Type);
                                    }
                                }
                                else
                                {
                                    canExport = false;
                                }
                            }

                            if (canExport && allowCleanup)
                            {
                                await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters
                                {
                                    Game = game.Type, OverwrittenConflicts = [definition], RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName)
                                });
                            }

                            processed++;
                            perc = GetProgressPercentage(total, processed);
                            if (previousProgress.IsNotNearlyEqual(perc))
                            {
                                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                                previousProgress = perc;
                            }
                        }
                    }

                    var conflicts = GetModelInstance<IConflictResult>();

                    var conflictsIndex = DIResolver.Get<IIndexedDefinitions>();
                    await conflictsIndex.InitMapAsync(await conflictResult.Conflicts.GetAllAsync(), true);
                    conflicts.Conflicts = conflictsIndex;
                    conflicts.ResolvedConflicts = resolvedIndex;
                    var ignoredIndex = DIResolver.Get<IIndexedDefinitions>();
                    await ignoredIndex.InitMapAsync(ignoredConflicts, true);
                    conflicts.IgnoredConflicts = ignoredIndex;
                    conflicts.IgnoredPaths = state.IgnoreConflictPaths ?? string.Empty;
                    conflicts.OverwrittenConflicts = conflictResult.OverwrittenConflicts;
                    var customConflicts = DIResolver.Get<IIndexedDefinitions>();
                    await customConflicts.InitMapAsync(state.CustomConflicts, true);
                    conflicts.CustomConflicts = customConflicts;
                    conflicts.Mode = conflictResult.Mode;
                    conflicts.AllowedLanguages = conflictResult.AllowedLanguages;

                    var indexResult = await initAllIndexedDefinitions(conflictResult, total, processed, 100);
                    conflictResult.AllConflicts.Dispose();
                    conflicts.AllConflicts = indexResult.Item1;
                    processed = indexResult.Item2;

                    await EvalModIgnoreDefinitionsAsync(conflicts);
                    processed = await syncPatchFiles(conflicts, patchFiles, patchName, total, processed, 100);

                    if (allowCleanup)
                    {
                        await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters
                        {
                            LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                            Mode = MapPatchStateMode(conflicts.Mode),
                            IgnoreConflictPaths = conflicts.IgnoredPaths,
                            Conflicts = await GetDefinitionOrDefaultAsync(conflicts.Conflicts),
                            ResolvedConflicts = await GetDefinitionOrDefaultAsync(conflicts.ResolvedConflicts),
                            IgnoredConflicts = await GetDefinitionOrDefaultAsync(conflicts.IgnoredConflicts),
                            OverwrittenConflicts = await GetDefinitionOrDefaultAsync(conflicts.OverwrittenConflicts),
                            CustomConflicts = await GetDefinitionOrDefaultAsync(conflicts.CustomConflicts),
                            RootPath = GetModDirectoryRootPath(game),
                            PatchPath = EvaluatePatchNamePath(game, patchName),
                            HasGameDefinitions = await conflicts.AllConflicts.HasGameDefinitionsAsync(),
                            AllowedLanguages = conflicts.AllowedLanguages
                        });
                    }

                    await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(100));

                    return conflicts;
                }
                else
                {
                    var processed = 0;
                    processed = await syncPatchFiles(conflictResult, patchFiles, patchName, total, processed, 100);

                    var exportedConflicts = false;
                    if ((await conflictResult.OverwrittenConflicts.GetAllAsync()).Any())
                    {
                        var alreadyMergedTypes = new HashSet<string>();
                        var overwrittenConflicts = PopulateModPath(await conflictResult.OverwrittenConflicts.GetAllAsync(), GetCollectionMods());
                        foreach (var item in overwrittenConflicts)
                        {
                            var definition = item;
                            var resolved = await conflictResult.ResolvedConflicts.GetByTypeAndIdAsync(definition.TypeAndId);
                            if (resolved.Any())
                            {
                                definition = resolved.FirstOrDefault();
                                definition!.Order = item.Order;
                                definition.DiskFile = item.DiskFile;
                                definition.File = item.File;
                                definition.OverwrittenFileNames = item.OverwrittenFileNames;
                            }

                            var canExport = true;
                            if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                            {
                                if (!alreadyMergedTypes.Contains(definition.Type))
                                {
                                    var merged = await ProcessOverwrittenSingleFileDefinitionsAsync(conflictResult, patchName, definition.Type);
                                    if (merged != null)
                                    {
                                        definition = PopulateModPath(merged, GetCollectionMods()).FirstOrDefault();
                                        alreadyMergedTypes.Add(definition!.Type);
                                    }
                                }
                                else
                                {
                                    canExport = false;
                                }
                            }

                            if (canExport && allowCleanup)
                            {
                                if (await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters
                                    {
                                        Game = game.Type, OverwrittenConflicts = [definition], RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName)
                                    }))
                                {
                                    exportedConflicts = true;
                                }
                            }

                            processed++;
                            perc = GetProgressPercentage(total, processed);
                            if (previousProgress.IsNotNearlyEqual(perc))
                            {
                                await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(perc));
                                previousProgress = perc;
                            }
                        }
                    }

                    await EvalModIgnoreDefinitionsAsync(conflictResult);

                    if (exportedConflicts && allowCleanup)
                    {
                        await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters
                        {
                            LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                            Mode = MapPatchStateMode(conflictResult.Mode),
                            IgnoreConflictPaths = conflictResult.IgnoredPaths,
                            Conflicts = await GetDefinitionOrDefaultAsync(conflictResult.Conflicts),
                            ResolvedConflicts = await GetDefinitionOrDefaultAsync(conflictResult.ResolvedConflicts),
                            IgnoredConflicts = await GetDefinitionOrDefaultAsync(conflictResult.IgnoredConflicts),
                            OverwrittenConflicts = await GetDefinitionOrDefaultAsync(conflictResult.OverwrittenConflicts),
                            CustomConflicts = await GetDefinitionOrDefaultAsync(conflictResult.CustomConflicts),
                            RootPath = GetModDirectoryRootPath(game),
                            PatchPath = EvaluatePatchNamePath(game, patchName),
                            HasGameDefinitions = await conflictResult.AllConflicts.HasGameDefinitionsAsync(),
                            AllowedLanguages = conflictResult.AllowedLanguages
                        });
                    }

                    var indexResult = await initAllIndexedDefinitions(conflictResult, total, processed, 100);
                    conflictResult.AllConflicts.Dispose();
                    conflictResult.AllConflicts = indexResult.Item1;
                    processed = indexResult.Item2;

                    await messageBus.PublishAsync(new ModDefinitionPatchLoadEvent(100));

                    return conflictResult;
                }
            }

            return null;
        }

        /// <summary>
        /// Invalidates the state of the patch mod.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns><c>true</c> if invalid, <c>false</c> otherwise.</returns>
        public virtual bool InvalidatePatchModState(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null)
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                Cache.Invalidate(new CacheInvalidateParameters { Region = CacheRegion, Prefix = game.Type, Keys = [patchName] });
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
            return modPatchExporter.LoadDefinitionContentsAsync(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) }, definition.File);
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
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) }, false);
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
            loadOrder ??= [];

            List<EvalState> mapEvalState(IEnumerable<IDefinition> definitions)
            {
                var result = new List<EvalState>();
                if ((definitions?.Any()).GetValueOrDefault())
                {
                    result.AddRange(definitions!.Where(p => !p.IsFromGame).Select(m => new EvalState { ContentSha = m.ContentSHA, FileName = m.OriginalFileName, FallBackFileName = m.File, ModName = m.ModName }));
                }

                return result;
            }

            async Task<bool> evalState(IGame game, string patchName)
            {
                Cache.Set(new CacheAddParameters<PatchCollectionState> { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState { CheckInProgress = true } });
                var allMods = GetInstalledModsInternal(game, false);
                var mods = allMods.Where(p => loadOrder.Any(x => x.Equals(p.DescriptorFile))).ToList();
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters { RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) }, false);
                if (state == null)
                {
                    Cache.Set(new CacheAddParameters<PatchCollectionState> { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState { NeedsUpdate = false, CheckInProgress = false } });
                    return false;
                }

                // Check load order first
                if (!state.LoadOrder.SequenceEqual(loadOrder))
                {
                    Cache.Set(new CacheAddParameters<PatchCollectionState> { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState { NeedsUpdate = true, CheckInProgress = false } });
                    return true;
                }

                var conflicts = new List<EvalState>();
                conflicts.AddRange(mapEvalState(state.Conflicts));
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
                                var mod = mods.FirstOrDefault(p => p.Name.Equals(definition!.ModName));
                                if (mod == null)
                                {
                                    // Mod no longer in collection, needs refresh break further checks...
                                    Cache.Set(new CacheAddParameters<PatchCollectionState> { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState { NeedsUpdate = true, CheckInProgress = false } });
                                    return true;
                                }
                                else
                                {
                                    var info = Reader.GetFileInfo(mod.FullPath, definition!.FileName);
                                    info ??= Reader.GetFileInfo(mod.FullPath, definition.FallBackFileName);
                                    if (info == null || !info.ContentSHA.Equals(definition.ContentSha))
                                    {
                                        // File no longer in collection or content does not match, break further checks
                                        Cache.Set(new CacheAddParameters<PatchCollectionState> { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState { NeedsUpdate = true, CheckInProgress = false } });
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
                while (tasks.Count != 0)
                {
                    var task = await Task.WhenAny(tasks);
                    tasks.Remove(task);
                    var result = await task;
                    if (result)
                    {
                        await cancellationToken.CancelAsync();
                        return true;
                    }
                }

                Cache.Set(new CacheAddParameters<PatchCollectionState> { Region = CacheRegion, Prefix = game.Type, Key = patchName, Value = new PatchCollectionState { NeedsUpdate = false, CheckInProgress = false } });
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
                var result = Cache.Get<PatchCollectionState>(new CacheGetParameters { Region = CacheRegion, Prefix = game.Type, Key = patchName });
                if (result != null)
                {
                    while (result.CheckInProgress)
                    {
                        // Since another check is queued, wait and periodically check if the task is done...
                        await Task.Delay(10);
                        result = Cache.Get<PatchCollectionState>(new CacheGetParameters { Region = CacheRegion, Prefix = game.Type, Key = patchName });
                        if (result == null)
                        {
                            await evalState(game, patchName);
                            result = Cache.Get<PatchCollectionState>(new CacheGetParameters { Region = CacheRegion, Prefix = game.Type, Key = patchName });
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
            return modPatchExporter.RenamePatchModAsync(new ModPatchExporterParameters
            {
                RootPath = GetModDirectoryRootPath(game),
                ModPath = EvaluatePatchNamePath(game, oldPatchName),
                PatchPath = EvaluatePatchNamePath(game, newPatchName),
                RenamePairs = [new KeyValuePair<string, string>(oldPatchName, newPatchName)]
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
        /// <returns><c>true</c> if reset, <c>false</c> otherwise.</returns>
        public virtual bool ResetPatchStateCache()
        {
            Cache.Invalidate(new CacheInvalidateParameters { Region = ModsExportedRegion, Keys = [ModExportedKey] });
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
            var game = GameService.GetSelected();
            if (definition == null || game == null)
            {
                return string.Empty;
            }

            if (definition.IsFromGame)
            {
                return Path.Combine(PathResolver.GetPath(game), definition.File);
            }
            else
            {
                var mods = GetCollectionMods();
                if (mods != null && mods.Any())
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
            }

            return string.Empty;
        }

        /// <summary>
        /// Saves the ignored paths asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> SaveIgnoredPathsAsync(IConflictResult conflictResult, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }

            await EvalModIgnoreDefinitionsAsync(conflictResult);
            if (conflictResult.Mode != PatchStateMode.ReadOnly && conflictResult.Mode != PatchStateMode.ReadOnlyWithoutLocalization)
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                return await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters
                {
                    LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                    Mode = MapPatchStateMode(conflictResult.Mode),
                    IgnoreConflictPaths = conflictResult.IgnoredPaths,
                    Conflicts = await GetDefinitionOrDefaultAsync(conflictResult.Conflicts),
                    ResolvedConflicts = await GetDefinitionOrDefaultAsync(conflictResult.ResolvedConflicts),
                    IgnoredConflicts = await GetDefinitionOrDefaultAsync(conflictResult.IgnoredConflicts),
                    OverwrittenConflicts = await GetDefinitionOrDefaultAsync(conflictResult.OverwrittenConflicts),
                    CustomConflicts = await GetDefinitionOrDefaultAsync(conflictResult.CustomConflicts),
                    RootPath = GetModDirectoryRootPath(game),
                    PatchPath = EvaluatePatchNamePath(game, patchName),
                    HasGameDefinitions = await conflictResult.AllConflicts.HasGameDefinitionsAsync(),
                    AllowedLanguages = conflictResult.AllowedLanguages
                });
            }

            return true;
        }

        /// <summary>
        /// Checks whether it should ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it should ignore, <c>false</c> otherwise.</returns>
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
        /// Checks whether it should show the reset conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it should show, <c>false</c> otherwise.</returns>
        public virtual bool? ShouldShowResetConflicts(IConflictResult conflictResult)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var lines = ignoredPaths.SplitOnNewLine();
                return lines.Any(p => p.Equals(ShowResetConflicts));
            }

            return null;
        }

        /// <summary>
        /// Checks whether if it should show self conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it should show, <c>false</c> otherwise.</returns>
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
        /// <returns><c>true</c> if toggled, <c>false</c> otherwise.</returns>
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
        /// <returns><c>true</c> if toggled, <c>false</c> otherwise.</returns>
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
        /// Toggles the show reset conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if toggled, <c>false</c> otherwise.</returns>
        public virtual bool? ToggleShowResetConflicts(IConflictResult conflictResult)
        {
            if (conflictResult != null)
            {
                var ignoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
                var shouldReset = ShouldShowResetConflicts(conflictResult);
                var lines = ignoredPaths.SplitOnNewLine().ToList();
                if (!shouldReset.GetValueOrDefault())
                {
                    lines.Add(ShowResetConflicts);
                }
                else
                {
                    lines.Remove(ShowResetConflicts);
                }

                conflictResult.IgnoredPaths = string.Join(Environment.NewLine, lines).Trim(Environment.NewLine.ToCharArray());
                return !shouldReset;
            }

            return null;
        }

        /// <summary>
        /// Validates the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public virtual IValidateResult Validate(IDefinition definition)
        {
            var result = GetModelInstance<IValidateResult>();
            result.IsValid = true;
            if (definition != null)
            {
                var lines = definition.Code.SplitOnNewLine();
                var args = new ParserArgs { Lines = lines, File = definition.File, ValidationType = MapValidationType(definition) };
                var validation = definition.ValueType != ValueType.Binary ? validateParser.Validate(args) : null;
                if (validation != null && validation.Any())
                {
                    result.ErrorMessage = validation.FirstOrDefault()!.ErrorMessage;
                    result.ErrorLine = validation.FirstOrDefault()!.ErrorLine;
                    result.ErrorColumn = validation.FirstOrDefault()!.ErrorColumn;
                    result.IsValid = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluates the definitions.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="conflicts">The conflicts.</param>
        /// <param name="definitions">The definitions.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="patchStateMode">The patch state mode.</param>
        /// <param name="fileConflictCache">The file conflict cache.</param>
        /// <param name="modShaConflictCache">The mod sha conflict cache.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task EvalDefinitionsAsync(IIndexedDefinitions indexedDefinitions, HashSet<IDefinition> conflicts, IEnumerable<IDefinition> definitions, IList<string> modOrder, PatchStateMode patchStateMode,
            Dictionary<string, bool> fileConflictCache, Dictionary<string, List<string>> modShaConflictCache)
        {
            async Task<bool> existsInLastFile(IDefinition definition)
            {
                var result = true;
                var fileDefs = await indexedDefinitions.GetByFileAsync(definition.FileCI);
                var lastMod = fileDefs.GroupBy(p => p.ModName).Select(p => p.First()).MaxBy(p => modOrder.IndexOf(p.ModName));
                if (lastMod != null)
                {
                    result = fileDefs.Any(p => p.ModName.Equals(lastMod.ModName) && p.TypeAndId.Equals(definition.TypeAndId));
                }

                return result;
            }

            var anyWholeTextFile = definitions.Any(p => p.ValueType == ValueType.WholeTextFile);
            var validDefinitions = new HashSet<IDefinition>();
            foreach (var item in definitions.Where(p => IsValidDefinitionType(p) || (anyWholeTextFile && p.ValueType == ValueType.EmptyFile)))
            {
                validDefinitions.Add(item);
            }

            var processed = new HashSet<IDefinition>();
            foreach (var def in validDefinitions)
            {
                if (processed.Contains(def) || conflicts.Contains(def) || def.AllowDuplicate)
                {
                    continue;
                }

                var allConflicts = (await indexedDefinitions.GetByTypeAndIdAsync(def.Type, def.Id)).Where(p => IsValidDefinitionType(p) || (anyWholeTextFile && p.ValueType == ValueType.EmptyFile));
                foreach (var conflict in allConflicts)
                {
                    processed.Add(conflict);
                }

                switch (allConflicts.Count())
                {
                    case > 1:
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
                                if (hasOverrides && patchStateMode is PatchStateMode.Default or PatchStateMode.DefaultWithoutLocalization)
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
                                    if (!conflicts.Contains(item) && (IsValidDefinitionType(item) || (anyWholeTextFile && item.ValueType == ValueType.EmptyFile)))
                                    {
                                        if (!item.IsFromGame)
                                        {
                                            if (modShaConflictCache.TryGetValue(item.TypeAndId, out var value) && value.Contains(item.DefinitionSHA))
                                            {
                                                continue;
                                            }
                                        }

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
                                            if (item.IsPlaceholder && shaMatches.Any(p => !p.IsPlaceholder))
                                            {
                                                // Uncheck placeholder mark as there's a duplicate which is not marked as placeholder
                                                item.IsPlaceholder = false;
                                            }
                                        }

                                        item.ExistsInLastFile = await existsInLastFile(item);
                                        conflicts.Add(item);
                                        if (!item.IsFromGame)
                                        {
                                            if (modShaConflictCache.TryGetValue(item.TypeAndId, out var value))
                                            {
                                                value.Add(item.DefinitionSHA);
                                            }
                                            else
                                            {
                                                var sha = new List<string> { item.DefinitionSHA };
                                                modShaConflictCache[item.TypeAndId] = sha;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                    case 1 when allConflicts.FirstOrDefault().ValueType == ValueType.Binary:
                        fileConflictCache.TryAdd(def.FileCI, false);
                        break;
                    case 1 when fileConflictCache.TryGetValue(def.FileCI, out var result):
                    {
                        if (result)
                        {
                            if (!conflicts.Contains(def) && (IsValidDefinitionType(def) || (anyWholeTextFile && def.ValueType == ValueType.EmptyFile)))
                            {
                                def.ExistsInLastFile = await existsInLastFile(def);
                                if (!def.ExistsInLastFile)
                                {
                                    conflicts.Add(def);
                                    if (!def.IsFromGame)
                                    {
                                        if (modShaConflictCache.TryGetValue(def.TypeAndId, out var value))
                                        {
                                            value.Add(def.DefinitionSHA);
                                        }
                                        else
                                        {
                                            var sha = new List<string> { def.DefinitionSHA };
                                            modShaConflictCache[def.TypeAndId] = sha;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                    case 1:
                    {
                        var fileDefs = await indexedDefinitions.GetByFileAsync(def.FileCI);
                        if (fileDefs.GroupBy(p => p.ModName).Count() > 1)
                        {
                            var hasOverrides = !def.IsCustomPatch && def.Dependencies != null && def.Dependencies.Any(p => fileDefs.Any(s => s.ModName.Equals(p)));
                            if (hasOverrides && patchStateMode is PatchStateMode.Default or PatchStateMode.DefaultWithoutLocalization)
                            {
                                fileConflictCache.TryAdd(def.FileCI, false);
                            }
                            else
                            {
                                fileConflictCache.TryAdd(def.FileCI, true);
                                if (!conflicts.Contains(def) && (IsValidDefinitionType(def) || (anyWholeTextFile && def.ValueType == ValueType.EmptyFile)))
                                {
                                    def.ExistsInLastFile = await existsInLastFile(def);
                                    if (!def.ExistsInLastFile)
                                    {
                                        conflicts.Add(def);
                                        if (!def.IsFromGame)
                                        {
                                            if (modShaConflictCache.TryGetValue(def.TypeAndId, out var value))
                                            {
                                                value.Add(def.DefinitionSHA);
                                            }
                                            else
                                            {
                                                var sha = new List<string> { def.DefinitionSHA };
                                                modShaConflictCache[def.TypeAndId] = sha;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            fileConflictCache.TryAdd(def.FileCI, false);
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates the mod ignore definitions.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task EvalModIgnoreDefinitionsAsync(IConflictResult conflictResult)
        {
            bool canAllowForbiddenMod(IHierarchicalDefinitions hierarchicalDefinition, IReadOnlyCollection<IModIgnoreConfiguration> ignoreConfigurations)
            {
                if (ignoreConfigurations.Any(i => hierarchicalDefinition.Mods.Any(m => i.ModName.Equals(m))))
                {
                    var item = ignoreConfigurations.FirstOrDefault(i => hierarchicalDefinition.Mods.Any(m => i.ModName.Equals(m)));
                    if (item != null)
                    {
                        return hierarchicalDefinition.Mods.Count > item.Count;
                    }
                }

                return true;
            }

            var ruleIgnoredDefinitions = DIResolver.Get<IIndexedDefinitions>();
            await ruleIgnoredDefinitions.InitMapAsync(null, true);
            var showResetConflicts = false;
            var ignoreGameMods = true;
            var ignoreSelfConflicts = true;
            var alreadyIgnored = new HashSet<string>();
            if (!string.IsNullOrEmpty(conflictResult.IgnoredPaths))
            {
                var allowedMods = GetCollectionMods().Select(p => p.Name).ToList();
                var forbiddenMods = new List<IModIgnoreConfiguration>();
                var ignoreRules = new List<string>();
                var includeRules = new List<string>();
                var lines = conflictResult.IgnoredPaths.SplitOnNewLine().Where(p => !p.Trim().StartsWith('#'));
                foreach (var line in lines)
                {
                    var parsed = line.StandardizeDirectorySeparator().Trim().TrimStart(Path.DirectorySeparatorChar);
                    var ignoreMod = ParseIgnoreModLine(line);
                    if (ignoreMod != null)
                    {
                        allowedMods.Remove(ignoreMod.ModName);
                        forbiddenMods.Add(ignoreMod);
                    }
                    else if (parsed.Equals(ShowGameModsId))
                    {
                        ignoreGameMods = false;
                    }
                    else if (parsed.Equals(ShowSelfConflictsId))
                    {
                        ignoreSelfConflicts = false;
                    }
                    else if (parsed.Equals(ShowResetConflicts))
                    {
                        // Only filter if there's actually something to filter
                        if (await conflictResult.Conflicts.HasResetDefinitionsAsync())
                        {
                            showResetConflicts = true;
                        }
                    }
                    else
                    {
                        if (!parsed.StartsWith('!'))
                        {
                            ignoreRules.Add(parsed);
                        }
                        else
                        {
                            includeRules.Add(parsed.TrimStart('!'));
                        }
                    }
                }

                if (!showResetConflicts)
                {
                    foreach (var topConflict in conflictResult.Conflicts.GetHierarchicalDefinitions())
                    {
                        if (topConflict.Mods.Any(allowedMods.Contains))
                        {
                            foreach (var item in topConflict.Children)
                            {
                                if (!item.Mods.Any(allowedMods.Contains) || !canAllowForbiddenMod(item, forbiddenMods))
                                {
                                    if (alreadyIgnored.Add(item.Key))
                                    {
                                        await ruleIgnoredDefinitions.AddToMapAsync((await conflictResult.Conflicts.GetByTypeAndIdAsync(item.Key)).First());
                                    }
                                }
                            }

                            var name = topConflict.Name;
                            if (!name.EndsWith(Path.DirectorySeparatorChar))
                            {
                                name = $"{name}{Path.DirectorySeparatorChar}";
                            }

                            var invalid = topConflict.Children.Where(p => ignoreRules.Any(r => EvalWildcard(r, [.. p.FileNames]))).Where(p => !includeRules.Any(r => EvalWildcard(r, [.. p.FileNames])));
                            foreach (var item in invalid)
                            {
                                if (alreadyIgnored.Add(item.Key))
                                {
                                    await ruleIgnoredDefinitions.AddToMapAsync((await conflictResult.Conflicts.GetByTypeAndIdAsync(item.Key)).First());
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in topConflict.Children)
                            {
                                if (alreadyIgnored.Add(item.Key))
                                {
                                    await ruleIgnoredDefinitions.AddToMapAsync((await conflictResult.Conflicts.GetByTypeAndIdAsync(item.Key)).First());
                                }
                            }
                        }
                    }
                }
            }

            if (showResetConflicts)
            {
                foreach (var topConflict in conflictResult.Conflicts.GetHierarchicalDefinitions())
                {
                    foreach (var item in topConflict.Children)
                    {
                        if (item.ResetType == ResetType.None && alreadyIgnored.Add(item.Key))
                        {
                            await ruleIgnoredDefinitions.AddToMapAsync((await conflictResult.Conflicts.GetByTypeAndIdAsync(item.Key)).First());
                        }
                    }
                }
            }
            else
            {
                if (ignoreGameMods || ignoreSelfConflicts)
                {
                    foreach (var topConflict in conflictResult.Conflicts.GetHierarchicalDefinitions())
                    {
                        foreach (var item in topConflict.Children.Where(p => p.Mods.Count <= 1))
                        {
                            if (ignoreGameMods && item.NonGameDefinitions <= 1 && alreadyIgnored.Add(item.Key))
                            {
                                await ruleIgnoredDefinitions.AddToMapAsync((await conflictResult.Conflicts.GetByTypeAndIdAsync(item.Key)).First());
                            }

                            if (ignoreSelfConflicts && item.NonGameDefinitions > 1 && alreadyIgnored.Add(item.Key))
                            {
                                await ruleIgnoredDefinitions.AddToMapAsync((await conflictResult.Conflicts.GetByTypeAndIdAsync(item.Key)).First());
                            }
                        }
                    }
                }
            }

            conflictResult.RuleIgnoredConflicts = ruleIgnoredDefinitions;
        }

        /// <summary>
        /// Evaluates the wildcard.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="values">The content.</param>
        /// <returns><c>true</c> if evaluated the wildcard, <c>false</c> otherwise.</returns>
        protected virtual bool EvalWildcard(string pattern, params string[] values)
        {
            foreach (var item in values)
            {
                if (pattern.Contains('*') || pattern.Contains('?'))
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
        /// <returns><c>true</c> if exported, <c>false</c> otherwise.</returns>
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
                    await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory });
                    await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = GetPatchModDirectory(game, patchName) });
                    if (game.ModDescriptorType == ModDescriptorType.JsonMetadata)
                    {
                        await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory });
                    }

                    await ModWriter.WriteDescriptorAsync(new ModWriterParameters
                    {
                        Mod = mod,
                        RootDirectory = game.UserDirectory,
                        Path = mod.DescriptorFile,
                        LockDescriptor = CheckIfModShouldBeLocked(game, mod),
                        DescriptorType = MapDescriptorType(game.ModDescriptorType)
                    }, IsPatchMod(mod));
                    allMods.Add(mod);
                    Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [GetModsCacheKey(true), GetModsCacheKey(false)] });
                }
                else
                {
                    mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
                }

                var definitionMod = allMods.FirstOrDefault(p => p.Name.Equals(definition.ModName));
                if (definitionMod != null || definition.IsFromGame)
                {
                    var args = new ModPatchExporterParameters { Game = game.Type, RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName) };
                    var exportPatches = new HashSet<IDefinition>();
                    switch (exportType)
                    {
                        case ExportType.Ignored:
                            await conflictResult.IgnoredConflicts.AddToMapAsync(definition);
                            break;

                        case ExportType.Custom:
                            await conflictResult.CustomConflicts.AddToMapAsync(definition);
                            exportPatches.Add(definition);
                            args.CustomConflicts = PopulateModPath(exportPatches, GetCollectionMods(allMods));
                            break;

                        default:
                            await conflictResult.ResolvedConflicts.AddToMapAsync(definition);
                            exportPatches.Add(definition);
                            args.Definitions = PopulateModPath(exportPatches, GetCollectionMods(allMods));
                            break;
                    }

                    var state = Cache.Get<ModsExportedState>(new CacheGetParameters { Region = ModsExportedRegion, Key = ModExportedKey });
                    if (state == null || state.Exported.GetValueOrDefault() == false)
                    {
                        await Task.Run(() =>
                        {
                            ModWriter.ApplyModsAsync(new ModWriterParameters { AppendOnly = true, TopPriorityMods = [mod], RootDirectory = game.UserDirectory, DescriptorType = MapDescriptorType(game.ModDescriptorType) })
                                .ConfigureAwait(false);
                        }).ConfigureAwait(false);
                        Cache.Set(new CacheAddParameters<ModsExportedState> { Region = ModsExportedRegion, Key = ModExportedKey, Value = new ModsExportedState { Exported = true } });
                    }

                    // Reset type flag since it was resolved now
                    definition.ResetType = ResetType.None;
                    await conflictResult.ResolvedConflicts.ChangeHierarchicalResetStateAsync(definition);

                    var exportResult = false;
                    if (exportPatches.Count != 0)
                    {
                        if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                        {
                            var merged = await ProcessOverwrittenSingleFileDefinitionsAsync(conflictResult, patchName, definition.Type);
                            if (merged != null)
                            {
                                args.OverwrittenConflicts = PopulateModPath(merged, GetCollectionMods());
                                args.Definitions = null;
                            }
                        }
                        else if (definition.ValueType == ValueType.OverwrittenObject)
                        {
                            var overwritten = await conflictResult.OverwrittenConflicts.GetByTypeAndIdAsync(definition.TypeAndId);
                            if (overwritten.Any())
                            {
                                definition.Order = overwritten.FirstOrDefault()!.Order;
                            }
                        }

                        exportResult = await modPatchExporter.ExportDefinitionAsync(args);
                        if (exportResult)
                        {
                            var overwritten = await conflictResult.OverwrittenConflicts.GetByTypeAndIdAsync(definition.TypeAndId);
                            if (overwritten.Any() && overwritten.FirstOrDefault()!.DiskFileCI != definition.DiskFileCI)
                            {
                                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = GetPatchModDirectory(game, patchName), Path = overwritten.FirstOrDefault()!.DiskFile });
                            }
                        }
                    }

                    var stateResult = await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters
                    {
                        LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                        Mode = MapPatchStateMode(conflictResult.Mode),
                        IgnoreConflictPaths = conflictResult.IgnoredPaths,
                        Definitions = exportPatches,
                        Conflicts = await GetDefinitionOrDefaultAsync(conflictResult.Conflicts),
                        ResolvedConflicts = await GetDefinitionOrDefaultAsync(conflictResult.ResolvedConflicts),
                        IgnoredConflicts = await GetDefinitionOrDefaultAsync(conflictResult.IgnoredConflicts),
                        OverwrittenConflicts = await GetDefinitionOrDefaultAsync(conflictResult.OverwrittenConflicts),
                        CustomConflicts = await GetDefinitionOrDefaultAsync(conflictResult.CustomConflicts),
                        RootPath = GetModDirectoryRootPath(game),
                        PatchPath = EvaluatePatchNamePath(game, patchName),
                        HasGameDefinitions = await conflictResult.AllConflicts.HasGameDefinitionsAsync(),
                        AllowedLanguages = conflictResult.AllowedLanguages
                    });
                    return exportPatches.Count != 0 ? exportResult && stateResult : stateResult;
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
        protected virtual async Task<IEnumerable<IDefinition>> FindPatchStateMatchedConflictsAsync(IIndexedDefinitions indexedDefinitions, IPatchState state, List<IDefinition> ignoredConflicts, IGrouping<string, IDefinition> item)
        {
            var matchedConflicts = await indexedDefinitions.GetByTypeAndIdAsync(item.First().TypeAndId);
            if (state.IgnoredConflicts != null)
            {
                var ignored = state.IgnoredConflicts.Where(p => p.TypeAndId.Equals(item.First().TypeAndId));
                if (ignored.Any())
                {
                    if (!IsCachedDefinitionDifferent(matchedConflicts, item))
                    {
                        ignoredConflicts.AddRange(ignored);
                    }
                    else
                    {
                        matchedConflicts.ToList().ForEach(p => p.ResetType = ResetType.Ignored);
                    }
                }
            }

            return matchedConflicts;
        }

        /// <summary>
        /// Get definition or default as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>A Task&lt;IList`1&gt; representing the asynchronous operation.</returns>
        protected virtual async Task<IList<IDefinition>> GetDefinitionOrDefaultAsync(IIndexedDefinitions definitions)
        {
            if (definitions != null)
            {
                var defs = await definitions.GetAllAsync();
                return defs.ToList();
            }

            return [];
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
            return valueType == ValueType.OverwrittenObject || valueType == ValueType.OverwrittenObjectSingleFile;
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
                IO.Common.PatchStateMode.AdvancedWithoutLocalization => PatchStateMode.AdvancedWithoutLocalization,
                IO.Common.PatchStateMode.DefaultWithoutLocalization => PatchStateMode.DefaultWithoutLocalization,
                _ => PatchStateMode.None
            };
        }

        /// <summary>
        /// Maps the patch state mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>IO.Common.PatchStateMode.</returns>
        /// <exception cref="ArgumentException">Invalid readonly mode</exception>
        /// <exception cref="System.ArgumentException">Invalid readonly mode</exception>
        protected virtual IO.Common.PatchStateMode MapPatchStateMode(PatchStateMode mode)
        {
            if (mode == PatchStateMode.ReadOnly || mode == PatchStateMode.ReadOnlyWithoutLocalization)
            {
                throw new ArgumentException("Invalid readonly mode");
            }

            return mode switch
            {
                PatchStateMode.Default => IO.Common.PatchStateMode.Default,
                PatchStateMode.Advanced => IO.Common.PatchStateMode.Advanced,
                PatchStateMode.AdvancedWithoutLocalization => IO.Common.PatchStateMode.AdvancedWithoutLocalization,
                PatchStateMode.DefaultWithoutLocalization => IO.Common.PatchStateMode.DefaultWithoutLocalization,
                _ => IO.Common.PatchStateMode.None
            };
        }

        /// <summary>
        /// Maps the type of the validation.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>ValidationType.</returns>
        protected virtual ValidationType MapValidationType(IDefinition definition)
        {
            if (definition.UseSimpleValidation.GetValueOrDefault())
            {
                return ValidationType.SimpleOnly;
            }
            else if (definition.UseSimpleValidation == null)
            {
                return ValidationType.SkipAll;
            }

            return ValidationType.Full;
        }

        /// <summary>
        /// Merges the single file definitions.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition MergeSingleFileDefinitions(IEnumerable<IDefinition> definitions)
        {
            bool hasCode(string file, string text)
            {
                var result = validateParser.HasCode(file, text);
                return result;
            }

            static void appendLine(StringBuilder sb, IEnumerable<string> lines, int indent = 0)
            {
                foreach (var item in lines)
                {
                    if (indent == 0)
                    {
                        sb.AppendLine(item);
                    }
                    else
                    {
                        sb.AppendLine($"{new string(' ', indent)}{item}");
                    }
                }
            }

            static void mergeCode(StringBuilder sb, string codeTag, string separator, IEnumerable<string> variables, IEnumerable<string> lines)
            {
                if (Shared.Constants.CodeSeparators.ClosingSeparators.Map.TryGetValue(separator, out var value))
                {
                    var closingTag = value;
                    sb.AppendLine($"{codeTag} = {separator}");
                    if (!lines.Any())
                    {
                        foreach (var item in variables)
                        {
                            var splitLines = item.SplitOnNewLine();
                            appendLine(sb, splitLines, 4);
                        }
                    }
                    else
                    {
                        var varsInserted = false;
                        foreach (var item in lines)
                        {
                            var splitLines = item.SplitOnNewLine();
                            foreach (var split in splitLines)
                            {
                                sb.AppendLine($"{new string(' ', 4)}{split}");
                                if (!varsInserted && split.Contains(Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket))
                                {
                                    varsInserted = true;
                                    foreach (var var in variables)
                                    {
                                        var splitVars = var.SplitOnNewLine();
                                        appendLine(sb, splitVars, 8);
                                    }
                                }
                            }
                        }
                    }

                    sb.AppendLine(closingTag);
                }
                else
                {
                    sb.AppendLine($"{codeTag}{separator}");
                    foreach (var item in variables)
                    {
                        var splitLines = item.SplitOnNewLine();
                        appendLine(sb, splitLines, 4);
                    }

                    foreach (var item in lines)
                    {
                        var splitLines = item.SplitOnNewLine();
                        appendLine(sb, splitLines, 4);
                    }
                }
            }

            var sb = new StringBuilder();
            var copy = CopyDefinition(definitions.FirstOrDefault());
            copy.ValueType = ValueType.OverwrittenObjectSingleFile;
            copy.IsFromGame = false;
            var groups = definitions.GroupBy(p => p.CodeTag, StringComparer.OrdinalIgnoreCase);
            foreach (var group in groups.OrderBy(p => p.FirstOrDefault()!.CodeTag, StringComparer.OrdinalIgnoreCase))
            {
                var hasCodeTag = !string.IsNullOrWhiteSpace(group.FirstOrDefault()!.CodeTag);
                if (!hasCodeTag)
                {
                    var namespaces = group.Where(p => hasCode(p.File, p.Code) && p.ValueType == ValueType.Namespace);
                    var variables = group.Where(p => hasCode(p.File, p.Code) && p.ValueType == ValueType.Variable);
                    var other = group.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace && hasCode(p.File, p.Code));
                    var code = namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode)).Concat(other.Select(p => p.OriginalCode));
                    appendLine(sb, code);
                }
                else
                {
                    var namespaces = group.Where(p => hasCode(p.File, p.Code) && p.ValueType == ValueType.Namespace);
                    var variables = definitions.Where(p => p.ValueType == ValueType.Variable && !string.IsNullOrWhiteSpace(p.CodeTag) && hasCode(p.File, p.Code));
                    var other = group.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace && hasCode(p.File, p.Code));
                    var vars = namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode));
                    var code = other.Select(p => p.OriginalCode);
                    mergeCode(sb, group.FirstOrDefault()!.CodeTag, group.FirstOrDefault()!.CodeSeparator, vars, code);
                }
            }

            copy.Code = sb.ToString();
            return copy;
        }

        /// <summary>
        /// Parses the ignore mod line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>IModIgnoreConfiguration.</returns>
        protected virtual IModIgnoreConfiguration ParseIgnoreModLine(string line)
        {
            var parsed = line.StandardizeDirectorySeparator().Trim().TrimStart(Path.DirectorySeparatorChar);
            if (parsed.StartsWith(ModNameIgnoreId))
            {
                var statements = line.Split(IgnoreRulesSeparator);
                var ignoredModName = statements[0].Replace(ModNameIgnoreId, string.Empty).Trim();
                var counter = 2;
                if (statements.Length > 1)
                {
                    if (int.TryParse(statements[1].Replace(ModNameIgnoreCounterId, string.Empty).Trim(), out var val))
                    {
                        counter = val;
                    }
                }

                var model = GetModelInstance<IModIgnoreConfiguration>();
                model.ModName = ignoredModName;
                model.Count = counter;
                return model;
            }

            return null;
        }

        /// <summary>
        /// Parses the mod files.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="fileInfos">The file infos.</param>
        /// <param name="modObject">The mod object.</param>
        /// <param name="definitionInfoProvider">The definition information provider.</param>
        /// <param name="allowedGameLanguages">The allowed game languages.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseModFiles(IGame game, IEnumerable<IFileInfo> fileInfos, IModObject modObject, IDefinitionInfoProvider definitionInfoProvider, IReadOnlyCollection<IGameLanguage> allowedGameLanguages)
        {
            if (fileInfos == null)
            {
                return null;
            }

            var definitions = new List<IDefinition>();
            foreach (var fileInfo in fileInfos)
            {
                // See if we should skip maybe
                var onlyFilename = Path.GetFileNameWithoutExtension(fileInfo.FileName);
                if (allowedGameLanguages != null && fileInfo.FileName!.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    if (!allowedGameLanguages.Any(p => onlyFilename!.EndsWith(p.Type, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                }

                var validationType = ValidationType.Full;
                if (definitionInfoProvider.SupportsInlineScripts)
                {
                    if (fileInfo.FileName != null && fileInfo.FileName.StartsWith(definitionInfoProvider.InlineScriptsPath))
                    {
                        // Skip inline validation
                        validationType = ValidationType.SkipAll;
                    }
                }

                var fileDefs = parserManager.Parse(new ParserManagerArgs
                {
                    ContentSHA = fileInfo.ContentSHA,
                    File = fileInfo.FileName,
                    GameType = game.Type,
                    Lines = fileInfo.Content,
                    ModDependencies = modObject.Dependencies,
                    ModName = modObject.Name,
                    FileLastModified = fileInfo.LastModified,
                    IsBinary = fileInfo.IsBinary,
                    ValidationType = validationType
                });
                if (fileDefs.Any())
                {
                    // Validate and see whether we need to check encoding
                    if (fileDefs.All(p => p.ValueType != ValueType.Invalid && p.ValueType != ValueType.Binary) && definitionInfoProvider != null &&
                        !definitionInfoProvider.IsValidEncoding(Path.GetDirectoryName(fileInfo.FileName), fileInfo.Encoding))
                    {
                        var definition = DIResolver.Get<IDefinition>();
                        definition.ErrorMessage = "File has invalid encoding, please use UTF-8-BOM Encoding.";
                        definition.Id = Path.GetFileName(fileInfo.FileName)!.ToLowerInvariant();
                        definition.ValueType = ValueType.Invalid;
                        definition.OriginalCode = definition.Code = string.Join(Environment.NewLine, fileInfo.Content ?? []);
                        definition.ContentSHA = fileInfo.ContentSHA;
                        definition.Dependencies = modObject.Dependencies;
                        definition.ModName = modObject.Name;
                        definition.OriginalModName = modObject.Name;
                        definition.OriginalFileName = fileInfo.FileName;
                        definition.File = fileInfo.FileName;
                        definition.Type = fileInfo.FileName.FormatDefinitionType();
                        definitions.Add(definition);
                        continue;
                    }

                    MergeDefinitions(fileDefs);
                    definitions.AddRange(fileDefs);
                }
            }

            return definitions;
        }

        /// <summary>
        /// Partials the definition copy.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="copyAdditionalFilenames">The copy additional filenames.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition PartialDefinitionCopy(IDefinition definition, bool copyAdditionalFilenames = true)
        {
            if (definition.ValueType == ValueType.Invalid || definition.AllowDuplicate)
            {
                return CopyDefinition(definition);
            }

            var clone = DIResolver.Get<IObjectClone>();
            return clone.PartialCloneDefinition(definition, copyAdditionalFilenames);
        }

        /// <summary>
        /// Processes the overwritten single file definitions.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="type">The type.</param>
        /// <param name="stateProvider">The state provider.</param>
        /// <returns>IDefinition.</returns>
        protected virtual async Task<IDefinition> ProcessOverwrittenSingleFileDefinitionsAsync(IConflictResult conflictResult, string patchName, string type, Tuple<IPatchState, IIndexedDefinitions> stateProvider = null)
        {
            static string cleanString(string text)
            {
                text ??= string.Empty;
                text = text.Replace(" ", string.Empty).Replace("\t", string.Empty).Trim();
                return text;
            }

            static string getNextVariableName(List<IDefinition> exportDefinitions, IDefinition definition)
            {
                var count = exportDefinitions.Count(p => p.Id.Equals(definition.Id, StringComparison.OrdinalIgnoreCase)) + 1;
                var name = $"{definition.Id}_{count}";
                while (exportDefinitions.Any(p => p.Id.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    count++;
                    name = $"{definition.Id}_{count}";
                }

                return name;
            }

            void parseNameSpaces(List<IDefinition> exportDefinitions, IDefinition def)
            {
                var namespaces = def.Variables?.Where(p => p.ValueType == ValueType.Namespace);
                if (namespaces != null && namespaces.Any())
                {
                    foreach (var name in namespaces)
                    {
                        if (!exportDefinitions.Any(p => p.ValueType == ValueType.Namespace && cleanString(p.Code).Equals(cleanString(name.Code))))
                        {
                            var copy = CopyDefinition(name);
                            copy.CodeTag = def.CodeTag;
                            copy.CodeSeparator = def.CodeSeparator;
                            exportDefinitions.Add(copy);
                        }
                    }
                }
            }

            void parseVariables(List<IDefinition> exportDefinitions, IDefinition def)
            {
                var variables = def.Variables?.Where(p => p.ValueType == ValueType.Variable);
                if (variables != null && variables.Any())
                {
                    foreach (var variable in variables)
                    {
                        var copy = CopyDefinition(variable);
                        var oldId = copy.Id;
                        copy.Id = getNextVariableName(exportDefinitions, variable);
                        copy.Code = string.Join(" ",
                            copy.Code.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                        copy.OriginalCode = string.Join(" ",
                            copy.OriginalCode.Split(" ", StringSplitOptions.None)
                                .Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                        copy.CodeTag = def.CodeTag;
                        copy.CodeSeparator = def.CodeSeparator;
                        exportDefinitions.Add(copy);
                        def.Code = string.Join(" ",
                            def.Code.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                        def.OriginalCode = string.Join(" ",
                            def.OriginalCode.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                    }
                }
            }

            var definitions = (await conflictResult.OverwrittenConflicts.GetByValueTypeAsync(ValueType.OverwrittenObjectSingleFile)).Where(p => p.Type.Equals(type));
            if (definitions.Any())
            {
                var modOrder = GetCollectionMods().Select(p => p.Name).ToList();
                var game = GameService.GetSelected();
                var export = new List<IDefinition>();
                var all = (await conflictResult.AllConflicts.GetByParentDirectoryAsync(definitions.FirstOrDefault()!.ParentDirectoryCI)).Where(IsValidDefinitionType);
                var ordered = all.GroupBy(p => p.TypeAndId).Select(p =>
                {
                    if (p.Any(v => v.AllowDuplicate))
                    {
                        return p.GroupBy(p => p.File).Select(v =>
                        {
                            var definition = v.FirstOrDefault();
                            return new DefinitionOrderSort { TypeAndId = definition!.TypeAndId, Order = definition.Order, File = Path.GetFileNameWithoutExtension(definition.File) };
                        });
                    }
                    else
                    {
                        var partialCopy = new List<IDefinition>();
                        p.ToList().ForEach(x => partialCopy.Add(PartialDefinitionCopy(x, false)));
                        var priority = EvalDefinitionPriorityInternal(partialCopy.OrderBy(x => modOrder.IndexOf(x.ModName)), true);
                        return [new DefinitionOrderSort { TypeAndId = priority.Definition.TypeAndId, Order = priority.Definition.Order, File = Path.GetFileNameWithoutExtension(priority.FileName) }];
                    }
                }).SelectMany(p => p).GroupBy(p => p.File).OrderBy(p => p.Key, StringComparer.Ordinal).SelectMany(p => p.OrderBy(x => x.Order)).ToList();
                var fullyOrdered = ordered.Select(p => new DefinitionOrderSort { TypeAndId = p.TypeAndId, Order = ordered.IndexOf(p), File = p.File }).ToList();
                var sortExport = new List<IDefinition>();
                var infoProvider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type) && p.IsFullyImplemented);
                var overwrittenFileNames = new HashSet<string>();

                async Task handleDefinition(IDefinition item)
                {
                    var definition = item;
                    IEnumerable<IDefinition> resolved;
                    if (stateProvider != null)
                    {
                        resolved = await stateProvider.Item2.GetByTypeAndIdAsync(item.TypeAndId);
                    }
                    else
                    {
                        resolved = await conflictResult.ResolvedConflicts.GetByTypeAndIdAsync(item.TypeAndId);
                    }

                    // Only need to check for resolution, since overwritten objects already have sorted out priority
                    if (resolved.Any())
                    {
                        definition = resolved.FirstOrDefault();
                        if (stateProvider != null)
                        {
                            definition!.Order = item.Order;
                            definition.DiskFile = item.DiskFile;
                            definition.File = item.File;
                            definition.OverwrittenFileNames = item.OverwrittenFileNames;

                            // If state is provided assume we need to load from conflict history
                            if (stateProvider.Item1 is { IndexedConflictHistory: not null } && stateProvider.Item1.IndexedConflictHistory.Any() && stateProvider.Item1.IndexedConflictHistory.TryGetValue(definition.TypeAndId, out var value))
                            {
                                var history = value.FirstOrDefault();
                                if (history != null)
                                {
                                    definition.Code = history.Code;
                                }
                            }
                        }
                    }

                    var copy = CopyDefinition(definition);
                    var parsed = parserManager.Parse(new ParserManagerArgs
                    {
                        ContentSHA = copy.ContentSHA,
                        File = copy.File,
                        GameType = game.Type,
                        Lines = copy.Code.SplitOnNewLine(false),
                        ModDependencies = copy.Dependencies,
                        ModName = copy.ModName
                    });
                    var others = parsed.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace);
                    foreach (var other in others)
                    {
                        var variables = parsed.Where(p => p.ValueType is ValueType.Variable or ValueType.Namespace);
                        other.Variables = variables;
                        var exportCopy = CopyDefinition(other);
                        var allType = (await conflictResult.AllConflicts.GetByTypeAndIdAsync(definition!.TypeAndId)).ToList();
                        allType.ForEach(p => overwrittenFileNames.Add(p.OriginalFileName));
                        if (other.AllowDuplicate)
                        {
                            var match = fullyOrdered.FirstOrDefault(p => p.TypeAndId == definition.TypeAndId && p.File == Path.GetFileNameWithoutExtension(other.File));
                            match ??= fullyOrdered.FirstOrDefault(p => p.TypeAndId == definition.TypeAndId);
                            exportCopy.Order = match!.Order;
                        }
                        else
                        {
                            exportCopy.Order = fullyOrdered.FirstOrDefault(p => p.TypeAndId == definition.TypeAndId)!.Order;
                        }

                        export.Add(exportCopy);
                        sortExport.Add(exportCopy);
                    }
                }

                var handledDuplicates = new HashSet<string>();
                foreach (var item in definitions)
                {
                    if (!item.AllowDuplicate)
                    {
                        await handleDefinition(item);
                    }
                    else if (handledDuplicates.Add(item.TypeAndId))
                    {
                        var duplicates = (await conflictResult.AllConflicts.GetByTypeAndIdAsync(item.TypeAndId)).GroupBy(p => p.File);
                        foreach (var duplicate in duplicates)
                        {
                            await handleDefinition(EvalDefinitionPriority(duplicate.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition);
                        }
                    }
                }

                var fullySortedExport = sortExport.OrderBy(p => p.Order).ToList();
                sortExport.ForEach(p => p.Order = fullySortedExport.IndexOf(p) + 1);
                foreach (var item in sortExport.OrderBy(p => p.Order).Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace))
                {
                    parseNameSpaces(export, item);
                    parseVariables(export, item);
                }

                if (export.All(p => p.ValueType == ValueType.Namespace || p.ValueType == ValueType.Variable))
                {
                    export.Clear();
                }

                if (export.Count != 0)
                {
                    var namespaces = export.Where(p => p.ValueType == ValueType.Namespace).OrderBy(p => p.Id);
                    var variables = export.Where(p => p.ValueType == ValueType.Variable).OrderBy(p => p.Id);
                    var other = export.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace).OrderBy(p => p.Order);
                    var merged = MergeSingleFileDefinitions(namespaces.Concat(variables.Concat(other)));
                    if (merged != null)
                    {
                        merged.Id = SingleFileMerged;
                        merged.File = overwrittenFileNames.FirstOrDefault();
                        merged.GeneratedFileNames = overwrittenFileNames.Distinct().ToList();
                        merged.File = infoProvider!.GetFileName(merged);
                        merged.DiskFile = infoProvider.GetDiskFileName(merged);
                        merged.ValueType = ValueType.OverwrittenObjectSingleFile;
                        merged.ModName = patchName;
                        merged.OverwrittenFileNames = overwrittenFileNames.Distinct().ToList();
                        return merged;
                    }
                }
            }

            return null;
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
        /// <param name="readOnlyMode">if set to <c>true</c> [read only mode].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task SyncPatchStateAsync(IGame game, string patchName, List<IDefinition> resolvedConflicts, IGrouping<string, IDefinition> item, IList<string> files, IEnumerable<IDefinition> matchedConflicts,
            bool readOnlyMode)
        {
            var synced = await SyncPatchStatesAsync(matchedConflicts, item, patchName, game, readOnlyMode, [.. files]);
            if (synced)
            {
                matchedConflicts.ToList().ForEach(p =>
                {
                    if (p.ResetType == ResetType.None)
                    {
                        p.ResetType = ResetType.Resolved;
                    }
                });
                item.ToList().ForEach(diff =>
                {
                    var existingConflict = resolvedConflicts.FirstOrDefault(p => p.TypeAndId.Equals(diff.TypeAndId));
                    if (existingConflict != null)
                    {
                        resolvedConflicts.Remove(existingConflict);
                    }
                });
            }
        }

        /// <summary>
        /// synchronize patch states as an asynchronous operation.
        /// </summary>
        /// <param name="currentConflicts">The current conflicts.</param>
        /// <param name="cachedConflicts">The cached conflicts.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="game">The game.</param>
        /// <param name="readOnlyMode">if set to <c>true</c> [read only mode].</param>
        /// <param name="files">The files.</param>
        /// <returns><c>true</c> if synced, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> SyncPatchStatesAsync(IEnumerable<IDefinition> currentConflicts, IEnumerable<IDefinition> cachedConflicts, string patchName, IGame game, bool readOnlyMode, params string[] files)
        {
            if (IsCachedDefinitionDifferent(currentConflicts, cachedConflicts))
            {
                if (!readOnlyMode)
                {
                    foreach (var file in files.Distinct())
                    {
                        await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = GetPatchModDirectory(game, patchName), Path = file });
                    }
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
        /// <returns><c>true</c> if unresolved, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> UnResolveConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName, ExportType exportType)
        {
            var game = GameService.GetSelected();
            var patchName = GenerateCollectionPatchName(collectionName);
            if (conflictResult != null && game != null)
            {
                var purgeFiles = true;
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

                var result = await indexed.GetByTypeAndIdAsync(typeAndId);
                if (result.Any())
                {
                    IEnumerable<IMod> collectionMods = null;
                    foreach (var item in result)
                    {
                        await indexed.RemoveAsync(item);
                        if (purgeFiles)
                        {
                            var patchModDirectory = GetPatchModDirectory(game, patchName);
                            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = patchModDirectory, Path = item.File });
                            if (!string.IsNullOrWhiteSpace(item.DiskFile) && item.File != item.DiskFile)
                            {
                                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = patchModDirectory, Path = item.DiskFile });
                            }

                            if (IsOverwrittenType(item.ValueType))
                            {
                                collectionMods ??= GetCollectionMods();
                                var overwritten = await conflictResult.OverwrittenConflicts.GetByTypeAndIdAsync(typeAndId);
                                if (overwritten.Any())
                                {
                                    if (item.ValueType == ValueType.OverwrittenObjectSingleFile)
                                    {
                                        var merged = await ProcessOverwrittenSingleFileDefinitionsAsync(conflictResult, patchName, item.Type);
                                        if (merged != null)
                                        {
                                            await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters
                                            {
                                                Game = game.Type, OverwrittenConflicts = PopulateModPath(merged, collectionMods), RootPath = GetModDirectoryRootPath(game), PatchPath = EvaluatePatchNamePath(game, patchName)
                                            });
                                        }
                                    }
                                    else
                                    {
                                        await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters
                                        {
                                            Game = game.Type,
                                            OverwrittenConflicts =
                                                PopulateModPath(await overwritten.ToAsyncEnumerable().WhereAwait(async p => !(await conflictResult.ResolvedConflicts.GetByTypeAndIdAsync(p.TypeAndId)).Any()).ToListAsync(),
                                                    collectionMods),
                                            RootPath = GetModDirectoryRootPath(game),
                                            PatchPath = EvaluatePatchNamePath(game, patchName)
                                        });
                                    }
                                }
                            }
                        }
                    }

                    var allMods = GetInstalledModsInternal(game, false).ToList();
                    IMod mod;
                    if (!allMods.Any(p => p.Name.Equals(patchName)))
                    {
                        mod = GeneratePatchModDescriptor(allMods, game, patchName);
                        await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory });
                        await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = GetPatchModDirectory(game, patchName) });
                        if (game.ModDescriptorType == ModDescriptorType.JsonMetadata)
                        {
                            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory });
                        }

                        await ModWriter.WriteDescriptorAsync(new ModWriterParameters
                        {
                            Mod = mod,
                            RootDirectory = game.UserDirectory,
                            Path = mod.DescriptorFile,
                            LockDescriptor = CheckIfModShouldBeLocked(game, mod),
                            DescriptorType = MapDescriptorType(game.ModDescriptorType)
                        }, IsPatchMod(mod));
                        allMods.Add(mod);
                        Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [GetModsCacheKey(true), GetModsCacheKey(false)] });
                    }
                    else
                    {
                        mod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
                    }

                    var state = Cache.Get<ModsExportedState>(new CacheGetParameters { Region = ModsExportedRegion, Key = ModExportedKey });
                    if (state == null || state.Exported.GetValueOrDefault() == false)
                    {
                        await Task.Run(() =>
                        {
                            ModWriter.ApplyModsAsync(new ModWriterParameters { AppendOnly = true, TopPriorityMods = [mod], RootDirectory = game.UserDirectory, DescriptorType = MapDescriptorType(game.ModDescriptorType) })
                                .ConfigureAwait(false);
                        }).ConfigureAwait(false);
                        Cache.Set(new CacheAddParameters<ModsExportedState> { Region = ModsExportedRegion, Key = ModExportedKey, Value = new ModsExportedState { Exported = true } });
                    }

                    await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters
                    {
                        LoadOrder = GetCollectionMods(collectionName: collectionName).Select(p => p.DescriptorFile),
                        Mode = MapPatchStateMode(conflictResult.Mode),
                        IgnoreConflictPaths = conflictResult.IgnoredPaths,
                        Conflicts = await GetDefinitionOrDefaultAsync(conflictResult.Conflicts),
                        ResolvedConflicts = await GetDefinitionOrDefaultAsync(conflictResult.ResolvedConflicts),
                        IgnoredConflicts = await GetDefinitionOrDefaultAsync(conflictResult.IgnoredConflicts),
                        OverwrittenConflicts = await GetDefinitionOrDefaultAsync(conflictResult.OverwrittenConflicts),
                        CustomConflicts = await GetDefinitionOrDefaultAsync(conflictResult.CustomConflicts),
                        RootPath = GetModDirectoryRootPath(game),
                        PatchPath = EvaluatePatchNamePath(game, patchName),
                        HasGameDefinitions = await conflictResult.AllConflicts.HasGameDefinitionsAsync(),
                        AllowedLanguages = conflictResult.AllowedLanguages
                    });
                    return true;
                }
            }

            return false;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DefinitionOrderSort.
        /// </summary>
        private class DefinitionOrderSort
        {
            #region Properties

            /// <summary>
            /// Gets or sets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File { get; init; }

            /// <summary>
            /// Gets or sets the order.
            /// </summary>
            /// <value>The order.</value>
            public int Order { get; init; }

            /// <summary>
            /// Gets or sets the type and identifier.
            /// </summary>
            /// <value>The type and identifier.</value>
            public string TypeAndId { get; init; }

            #endregion Properties
        }

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
            public string ContentSha { get; init; }

            /// <summary>
            /// Gets or sets the name of the fallback file.
            /// </summary>
            /// <value>The name of the fallback file.</value>
            public string FallBackFileName { get; init; }

            /// <summary>
            /// Gets or sets the name of the file.
            /// </summary>
            /// <value>The name of the file.</value>
            public string FileName { get; init; }

            /// <summary>
            /// Gets or sets the name of the mod.
            /// </summary>
            /// <value>The name of the mod.</value>
            public string ModName { get; init; }

            #endregion Properties
        }

        /// <summary>
        /// Class ModsExportedState.
        /// </summary>
        private class ModsExportedState
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ModsExportedState" /> is exported.
            /// </summary>
            /// <value><c>null</c> if [exported] contains no value, <c>true</c> if [exported]; otherwise, <c>false</c>.</value>
            public bool? Exported { get; init; }

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
            public bool CheckInProgress { get; init; }

            /// <summary>
            /// Gets or sets a value indicating whether [needs update].
            /// </summary>
            /// <value><c>true</c> if [needs update]; otherwise, <c>false</c>.</value>
            public bool NeedsUpdate { get; init; }

            #endregion Properties
        }

        #endregion Classes
    }
}
