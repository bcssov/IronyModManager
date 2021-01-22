// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 01-22-2021
// ***********************************************************************
// <copyright file="ModMergeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Mod;
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
    /// Class ModMergeService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModMergeService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModMergeService" />
    public class ModMergeService : ModBaseService, IModMergeService
    {
        #region Fields

        /// <summary>
        /// The maximum zips to merge
        /// </summary>
        private const int MaxZipsToMerge = 4;

        /// <summary>
        /// The zip lock
        /// </summary>
        private static readonly AsyncLock zipLock = new AsyncLock();

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The mod merge compress exporter
        /// </summary>
        private readonly IModMergeCompressExporter modMergeCompressExporter;

        /// <summary>
        /// The mod merge exporter
        /// </summary>
        private readonly IModMergeExporter modMergeExporter;

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
        /// Initializes a new instance of the <see cref="ModMergeService" /> class.
        /// </summary>
        /// <param name="modMergeCompressExporter">The mod merge compress exporter.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="modMergeExporter">The mod merge exporter.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModMergeService(IModMergeCompressExporter modMergeCompressExporter, IParserManager parserManager, ICache cache, IMessageBus messageBus, IModPatchExporter modPatchExporter,
            IModMergeExporter modMergeExporter, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.modMergeCompressExporter = modMergeCompressExporter;
            this.parserManager = parserManager;
            this.messageBus = messageBus;
            this.modMergeExporter = modMergeExporter;
            this.modPatchExporter = modPatchExporter;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// merge collection as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IMod&gt;.</returns>
        public virtual async Task<IMod> MergeCollectionByDefinitionsAsync(IConflictResult conflictResult, IList<string> modOrder, string collectionName)
        {
            static string cleanString(string text)
            {
                text ??= string.Empty;
                text = text.Replace(" ", string.Empty).Replace("\t", string.Empty).Trim();
                return text;
            }
            static string getNextVariableName(List<IDefinition> exportDefinitons, IDefinition definition)
            {
                var count = exportDefinitons.Where(p => p.Id.Equals(definition.Id, StringComparison.OrdinalIgnoreCase)).Count() + 1;
                var name = $"{definition.Id}_{count}";
                while (exportDefinitons.Any(p => p.Id.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    count++;
                    name = $"{definition.Id}_{count}";
                }
                return name;
            }
            void parseNameSpaces(List<IDefinition> exportDefinitions, IDefinition def)
            {
                var namespaces = def.Variables?.Where(p => p.ValueType == ValueType.Namespace);
                if (namespaces?.Count() > 0)
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
                if (variables?.Count() > 0)
                {
                    foreach (var variable in variables)
                    {
                        var copy = CopyDefinition(variable);
                        var oldId = copy.Id;
                        copy.Id = getNextVariableName(exportDefinitions, variable);
                        copy.Code = string.Join(" ", copy.Code.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                        copy.OriginalCode = string.Join(" ", copy.OriginalCode.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                        copy.CodeTag = def.CodeTag;
                        copy.CodeSeparator = def.CodeSeparator;
                        exportDefinitions.Add(copy);
                        def.Code = string.Join(" ", def.Code.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                        def.OriginalCode = string.Join(" ", def.OriginalCode.Split(" ", StringSplitOptions.None).Select(p => p.Contains(oldId) ? string.Join(Environment.NewLine, p.SplitOnNewLine(false).Select(s => s.Trim() == oldId ? s.Replace(oldId, copy.Id) : s)) : p));
                    }
                }
            }

            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var total = conflictResult.AllConflicts.GetAll().Count(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace);
            if (conflictResult.AllConflicts.GetAll().Any())
            {
                var allMods = GetInstalledModsInternal(game, false).ToList();
                var mergeCollectionPath = collectionName.GenerateValidFileName();
                var collectionMods = GetCollectionMods(allMods);

                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
                }, true);
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Shared.Constants.ModDirectory
                });
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
                });

                var mod = DIResolver.Get<IMod>();
                mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{mergeCollectionPath}{Shared.Constants.ModExtension}";
                mod.FileName = GetModDirectory(game, mergeCollectionPath).Replace("\\", "/");
                mod.Name = collectionName;
                mod.Source = ModSource.Local;
                mod.Version = allMods.OrderByDescending(p => p.VersionData).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.VersionData).FirstOrDefault().Version : string.Empty;
                await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                {
                    Mod = mod,
                    RootDirectory = game.UserDirectory,
                    Path = mod.DescriptorFile,
                    LockDescriptor = CheckIfModShouldBeLocked(game, mod)
                }, true);
                Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));

                var exportPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, mergeCollectionPath);

                var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
                var patchName = GenerateCollectionPatchName(collection.Name);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                }, true);

                var resolvedConflicts = state?.ResolvedConflicts ?? new List<IDefinition>();
                var ignoredConflicts = state?.IgnoredConflicts ?? new List<IDefinition>();
                var conflictHistory = state?.ConflictHistory ?? new List<IDefinition>();
                var customConflicts = state?.CustomConflicts ?? new List<IDefinition>();

                var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                resolvedIndex.InitMap(resolvedConflicts, true);
                conflictResult.ResolvedConflicts = resolvedIndex;
                var ignoredIndex = DIResolver.Get<IIndexedDefinitions>();
                ignoredIndex.InitMap(ignoredConflicts, true);
                conflictResult.IgnoredConflicts = ignoredIndex;
                var customConflictsIndex = DIResolver.Get<IIndexedDefinitions>();
                customConflictsIndex.InitMap(customConflicts);
                conflictResult.CustomConflicts = customConflictsIndex;
                var conflictHistoryIndex = DIResolver.Get<IIndexedDefinitions>();
                conflictHistoryIndex.InitMap(conflictHistory);
                if (customConflicts.Any())
                {
                    total += customConflicts.Count();
                }

                double lastPercentage = 0;
                int processed = 0;

                foreach (var file in conflictResult.CustomConflicts.GetAllFileKeys())
                {
                    var definition = CopyDefinition(conflictResult.CustomConflicts.GetByFile(file).FirstOrDefault());
                    definition.Code = conflictHistoryIndex.GetByTypeAndId(definition.TypeAndId).FirstOrDefault().Code;
                    await modMergeExporter.ExportDefinitionsAsync(new ModMergeDefinitionExporterParameters()
                    {
                        ExportPath = exportPath,
                        Definitions = definition != null ? PopulateModPath(new List<IDefinition>() { definition }, collectionMods) : null,
                        Game = game.Type
                    });
                    processed++;
                    var percentage = GetProgressPercentage(total, processed, 99.9);
                    if (lastPercentage != percentage)
                    {
                        await messageBus.PublishAsync(new ModDefinitionMergeProgressEvent(percentage));
                    }
                    lastPercentage = percentage;
                }

                var dumpedIds = new HashSet<string>();
                var fileCount = conflictResult.AllConflicts.GetAllFileKeys().Count();
                var counter = 0;
                foreach (var file in conflictResult.AllConflicts.GetAllFileKeys().OrderBy(p => p))
                {
                    counter++;
                    var definitions = conflictResult.AllConflicts.GetByFile(file).Where(p => p.ValueType != ValueType.EmptyFile);
                    if (definitions.Any())
                    {
                        var exportDefinitions = new List<IDefinition>();
                        foreach (var definitionGroup in definitions.GroupBy(p => p.TypeAndId).Where(p => p.FirstOrDefault() != null && p.FirstOrDefault().ValueType != ValueType.Namespace && p.FirstOrDefault().ValueType != ValueType.Variable))
                        {
                            // Orphans are placed under resolved items during analysis so no need to check them
                            var resolved = conflictResult.ResolvedConflicts.GetByTypeAndId(definitionGroup.FirstOrDefault().TypeAndId);
                            var overwritten = conflictResult.OverwrittenConflicts.GetByTypeAndId(definitionGroup.FirstOrDefault().TypeAndId);
                            if (resolved.Any() || overwritten.Any())
                            {
                                // Resolved takes priority, since if an item was resolved no need to use the overwritten code
                                // Also fetch the code from the patch state.json object to get the latest version of the code to dump
                                if (resolved.Any())
                                {
                                    foreach (var item in resolved)
                                    {
                                        if (!dumpedIds.Contains(item.TypeAndId))
                                        {
                                            var copy = CopyDefinition(item);
                                            if (copy.ValueType != ValueType.Binary)
                                            {
                                                copy.Code = conflictHistoryIndex.GetByTypeAndId(item.TypeAndId).FirstOrDefault().Code;
                                                var parsed = parserManager.Parse(new Parser.Common.Args.ParserManagerArgs()
                                                {
                                                    ContentSHA = copy.ContentSHA,
                                                    File = copy.File,
                                                    GameType = GameService.GetSelected().Type,
                                                    Lines = copy.Code.SplitOnNewLine(false),
                                                    ModDependencies = copy.Dependencies,
                                                    ModName = copy.ModName
                                                });
                                                var others = parsed.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace);
                                                foreach (var other in others)
                                                {
                                                    var variables = parsed.Where(p => p.ValueType == ValueType.Variable || p.ValueType == ValueType.Namespace);
                                                    other.Variables = variables;
                                                    parseNameSpaces(exportDefinitions, other);
                                                    parseVariables(exportDefinitions, other);
                                                    dumpedIds.Add(other.TypeAndId);
                                                    exportDefinitions.Add(CopyDefinition(other));
                                                }
                                            }
                                            else
                                            {
                                                dumpedIds.Add(copy.TypeAndId);
                                                exportDefinitions.Add(copy);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in overwritten)
                                    {
                                        if (!dumpedIds.Contains(item.TypeAndId))
                                        {
                                            var copy = CopyDefinition(item);
                                            if (copy.Variables?.Count() > 0)
                                            {
                                                parseNameSpaces(exportDefinitions, copy);
                                                parseVariables(exportDefinitions, copy);
                                            }
                                            dumpedIds.Add(copy.TypeAndId);
                                            exportDefinitions.Add(copy);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Check if this is a conflict so we can then perform evaluation of which definition would win based on current order
                                var conflicted = conflictResult.Conflicts.GetByTypeAndId(definitionGroup.FirstOrDefault().TypeAndId);
                                IDefinition priorityDef;
                                if (conflicted.Any())
                                {
                                    priorityDef = EvalDefinitionPriorityInternal(conflicted.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition;
                                }
                                else
                                {
                                    priorityDef = definitionGroup.FirstOrDefault();
                                }
                                if (priorityDef != null)
                                {
                                    IDefinition priorityDefCopy = null;
                                    if (!dumpedIds.Contains(priorityDef.TypeAndId))
                                    {
                                        priorityDefCopy = CopyDefinition(priorityDef);
                                        exportDefinitions.Add(priorityDefCopy);
                                        dumpedIds.Add(priorityDef.TypeAndId);
                                    }
                                    if (priorityDefCopy?.Variables?.Count() > 0)
                                    {
                                        parseNameSpaces(exportDefinitions, priorityDefCopy);
                                        parseVariables(exportDefinitions, priorityDefCopy);
                                    }
                                }
                            }
                            processed += definitionGroup.Count();
                            var percentage = GetProgressPercentage(total, processed, 99.9);
                            if (lastPercentage != percentage)
                            {
                                await messageBus.PublishAsync(new ModDefinitionMergeProgressEvent(percentage));
                            }
                            lastPercentage = percentage;
                        }

                        // Prevent exporting only namespaces or variables?
                        if (exportDefinitions.All(p => p.ValueType == ValueType.Namespace || p.ValueType == ValueType.Variable))
                        {
                            exportDefinitions.Clear();
                        }
                        // Something to export?
                        if (exportDefinitions.Count > 0)
                        {
                            var groupedMods = exportDefinitions.GroupBy(p => p.ModName);
                            if (groupedMods.Count() > 1)
                            {
                                var topGroup = groupedMods.OrderByDescending(p => p.Count()).FirstOrDefault();
                                foreach (var groupedMod in groupedMods.Where(p => p.Key != topGroup.Key))
                                {
                                    foreach (var item in groupedMod)
                                    {
                                        var allConflicts = conflictResult.AllConflicts.GetByTypeAndId(item.TypeAndId).Where(p => p.ModName.Equals(topGroup.Key));
                                        if (allConflicts.Count() > 1)
                                        {
                                            var match = allConflicts.FirstOrDefault(p => p.FileCI.Equals(item.FileCI));
                                            if (match != null)
                                            {
                                                item.Order = match.Order;
                                            }
                                            else
                                            {
                                                var infoProvider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
                                                if (infoProvider.DefinitionUsesFIOSRules(item))
                                                {
                                                    item.Order = allConflicts.OrderBy(p => p.File, StringComparer.Ordinal).FirstOrDefault().Order;
                                                }
                                                else
                                                {
                                                    item.Order = allConflicts.OrderByDescending(p => p.File, StringComparer.Ordinal).FirstOrDefault().Order;
                                                }
                                            }
                                        }
                                        else if (allConflicts.Count() == 1)
                                        {
                                            item.Order = allConflicts.FirstOrDefault().Order;
                                        }
                                    }
                                }
                            }
                            var variables = exportDefinitions.Where(p => p.ValueType == ValueType.Variable || p.ValueType == ValueType.Namespace).OrderBy(p => p.Id);
                            var other = exportDefinitions.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace).OrderBy(p => p.Order);
                            var merged = MergeDefinitions(variables.Concat(other));
                            // Preserve proper file casing
                            var conflicts = conflictResult.AllConflicts.GetByFile(file);
                            if (merged != null)
                            {
                                merged.File = conflicts.FirstOrDefault().File;
                                merged.DiskFile = conflicts.FirstOrDefault().DiskFile;
                                await modMergeExporter.ExportDefinitionsAsync(new ModMergeDefinitionExporterParameters()
                                {
                                    ExportPath = exportPath,
                                    Definitions = PopulateModPath(new List<IDefinition>() { merged }, collectionMods),
                                    Game = game.Type
                                });
                            }
                            if (counter >= fileCount)
                            {
                                await messageBus.PublishAsync(new ModDefinitionMergeProgressEvent(100));
                            }
                        }
                    }
                }

                return mod;
            }
            return null;
        }

        /// <summary>
        /// Merges the collection by files asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IMod&gt;.</returns>
        public virtual async Task<IMod> MergeCollectionByFilesAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return null;
            }

            var allMods = GetInstalledModsInternal(game, false).ToList();
            var collectionMods = GetCollectionMods(allMods).ToList();
            if (collectionMods.Count == 0)
            {
                return null;
            }

            var mergeCollectionPath = collectionName.GenerateValidFileName();
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
            }, true);
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Shared.Constants.ModDirectory
            });
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
            });

            var mod = DIResolver.Get<IMod>();
            mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{mergeCollectionPath}{Shared.Constants.ModExtension}";
            mod.FileName = GetModDirectory(game, mergeCollectionPath).Replace("\\", "/");
            mod.Name = collectionName;
            mod.Source = ModSource.Local;
            mod.Version = allMods.OrderByDescending(p => p.VersionData).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.VersionData).FirstOrDefault().Version : string.Empty;
            mod.FullPath = GetModDirectory(game, mergeCollectionPath);
            await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
            {
                Mod = mod,
                RootDirectory = game.UserDirectory,
                Path = mod.DescriptorFile,
                LockDescriptor = CheckIfModShouldBeLocked(game, mod)
            }, true);
            Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));

            var exportPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, mergeCollectionPath);
            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchName = GenerateCollectionPatchName(collection.Name);
            var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
            if (patchMod != null)
            {
                collectionMods.Add(patchMod);
            }

            await messageBus.PublishAsync(new ModFileMergeProgressEvent(1, 0));
            await PopulateModFilesInternalAsync(collectionMods);
            await messageBus.PublishAsync(new ModFileMergeProgressEvent(1, 100));

            var totalFiles = collectionMods.Where(p => p.Files != null).SelectMany(p => p.Files.Where(f => game.GameFolders.Any(s => f.StartsWith(s, StringComparison.OrdinalIgnoreCase)))).Count();
            double lastPercentage = 0;
            var processed = 0;
            foreach (var collectionMod in collectionMods.Where(p => p.Files != null))
            {
                foreach (var file in collectionMod.Files.Where(p => game.GameFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase))))
                {
                    processed++;
                    await modMergeExporter.ExportFilesAsync(new ModMergeFileExporterParameters()
                    {
                        RootModPath = collectionMod.FullPath,
                        ExportFile = file,
                        ExportPath = mod.FullPath
                    });
                    var percentage = GetProgressPercentage(totalFiles, processed, 100);
                    if (lastPercentage != percentage)
                    {
                        await messageBus.PublishAsync(new ModFileMergeProgressEvent(2, percentage));
                    }
                    lastPercentage = percentage;
                }
            }
            return mod;
        }

        /// <summary>
        /// merge compress collection as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="copiedNamePrefix">The copied name prefix.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        public virtual async Task<IEnumerable<IMod>> MergeCompressCollectionAsync(string collectionName, string copiedNamePrefix)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return null;
            }

            IMod cloneMod(IMod mod, string fileName, int order)
            {
                var newMod = DIResolver.Get<IMod>();
                newMod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{Path.GetFileNameWithoutExtension(fileName)}{Shared.Constants.ModExtension}";
                newMod.FileName = GetModDirectory(game, fileName).Replace("\\", "/");
                newMod.Name = !string.IsNullOrWhiteSpace(copiedNamePrefix) ? $"{copiedNamePrefix} {mod.Name}" : mod.Name;
                newMod.Source = ModSource.Local;
                newMod.Version = mod.Version;
                newMod.FullPath = GetModDirectory(game, fileName);
                newMod.RemoteId = mod.RemoteId;
                newMod.Picture = mod.Picture;
                newMod.ReplacePath = mod.ReplacePath;
                newMod.Tags = mod.Tags;
                newMod.UserDir = mod.UserDir;
                newMod.Order = order;
                var dependencies = mod.Dependencies;
                if (dependencies != null && dependencies.Any())
                {
                    var newDependencies = new List<string>();
                    foreach (var item in dependencies)
                    {
                        newDependencies.Add($"{copiedNamePrefix} {item}");
                    }
                    newMod.Dependencies = newDependencies;
                }
                return newMod;
            }

            var allMods = GetInstalledModsInternal(game, false).ToList();
            var collectionMods = GetCollectionMods(allMods).ToList();
            if (collectionMods.Count == 0)
            {
                return null;
            }
            var mergeCollectionPath = collectionName.GenerateValidFileName();
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
            }, true);
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Shared.Constants.ModDirectory
            });
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
            });

            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchName = GenerateCollectionPatchName(collection.Name);
            var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));

            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(1, 0));
            await PopulateModFilesInternalAsync(collectionMods);
            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(1, 100));

            var newPatchName = GenerateCollectionPatchName(collectionName);
            var renamePairs = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(patchName, newPatchName) };

            var totalFiles = (collectionMods.Where(p => p.Files != null).SelectMany(p => p.Files.Where(f => game.GameFolders.Any(s => f.StartsWith(s, StringComparison.OrdinalIgnoreCase)))).Count() * 2) + collectionMods.Count;
            double lastPercentage = 0;
            var processed = 0;
            async void modMergeCompressExporter_ProcessedFile(object sender, EventArgs e)
            {
                using var mutex = await zipLock.LockAsync();
                processed++;
                var percentage = GetProgressPercentage(totalFiles, processed, 99.98);
                if (lastPercentage != percentage)
                {
                    await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, percentage));
                }
                lastPercentage = percentage;
                mutex.Dispose();
            }
            modMergeCompressExporter.ProcessedFile += modMergeCompressExporter_ProcessedFile;

            var exportedMods = new List<IMod>();
            using var semaphore = new SemaphoreSlim(MaxZipsToMerge);
            var zipTasks = collectionMods.Where(p => p.Files != null).Select(async collectionMod =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var queueId = modMergeCompressExporter.Start();
                    var streams = new List<Stream>();
                    foreach (var file in collectionMod.Files.Where(p => game.GameFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase))))
                    {
                        // OSX seems to have a very low ulimit (according to #183) -- so don't punish other OS users by placing stuff in memory
                        var stream = Reader.GetStream(collectionMod.FullPath, file);
                        if (stream != null)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                var streamCopy = new MemoryStream();
                                if (stream.CanSeek)
                                {
                                    stream.Seek(0, SeekOrigin.Begin);
                                }
                                await stream.CopyToAsync(streamCopy);
                                stream.Close();
                                await stream.DisposeAsync();
                                modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters()
                                {
                                    FileName = file,
                                    QueueId = queueId,
                                    Stream = streamCopy
                                });
                                streams.Add(streamCopy);
                            }
                            else
                            {
                                modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters()
                                {
                                    FileName = file,
                                    QueueId = queueId,
                                    Stream = stream
                                });
                                streams.Add(stream);
                            }
                        }
                        using var innerProgressLock = await zipLock.LockAsync();
                        processed++;
                        var innerPercentage = GetProgressPercentage(totalFiles, processed, 99.98);
                        if (lastPercentage != innerPercentage)
                        {
                            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, innerPercentage));
                        }
                        lastPercentage = innerPercentage;
                        innerProgressLock.Dispose();
                    }

                    var newMod = cloneMod(collectionMod,
                        Path.Combine(mergeCollectionPath, !string.IsNullOrWhiteSpace(copiedNamePrefix) ?
                        $"{copiedNamePrefix} {collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName() :
                        $"{collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName()),
                        collectionMods.IndexOf(collectionMod));
                    var ms = new MemoryStream();
                    await ModWriter.WriteDescriptorToStreamAsync(new ModWriterParameters()
                    {
                        Mod = newMod
                    }, ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters()
                    {
                        FileName = Shared.Constants.DescriptorFile,
                        QueueId = queueId,
                        Stream = ms
                    });
                    streams.Add(ms);

                    using var outerProgressLock = await zipLock.LockAsync();
                    var outerPercentage = GetProgressPercentage(totalFiles, processed, 99.98);
                    if (lastPercentage != outerPercentage)
                    {
                        await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, outerPercentage));
                    }
                    lastPercentage = outerPercentage;
                    outerProgressLock.Dispose();

                    modMergeCompressExporter.Finalize(queueId, Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, mergeCollectionPath, !string.IsNullOrWhiteSpace(copiedNamePrefix) ? $"{copiedNamePrefix} {collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName() : $"{collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName()));
                    renamePairs.Add(new KeyValuePair<string, string>(collectionMod.Name, newMod.Name));
                    using var exportModLock = await zipLock.LockAsync();
                    exportedMods.Add(newMod);
                    exportModLock.Dispose();

                    var streamTasks = streams.Select(async p =>
                    {
                        p.Close();
                        await p.DisposeAsync();
                    });
                    await Task.WhenAll(streamTasks);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(zipTasks);

            modMergeCompressExporter.ProcessedFile -= modMergeCompressExporter_ProcessedFile;

            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, 99.99));
            await modPatchExporter.CopyPatchModAsync(new ModPatchExporterParameters()
            {
                RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                ModPath = patchName,
                PatchName = newPatchName,
                RenamePairs = renamePairs
            });
            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, 100));

            Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));
            var ordered = exportedMods.OrderBy(p => p.Order).ToList();
            return ordered;
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
        /// Merges the definitions.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition MergeDefinitions(IEnumerable<IDefinition> definitions)
        {
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
                if (Shared.Constants.CodeSeparators.ClosingSeparators.Map.ContainsKey(separator))
                {
                    var closingTag = Shared.Constants.CodeSeparators.ClosingSeparators.Map[separator];
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
                        bool varsInserted = false;
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
            if (copy.ValueType == ValueType.Namespace || copy.ValueType == ValueType.Variable)
            {
                copy.ValueType = ValueType.Object;
            }
            var groups = definitions.GroupBy(p => p.CodeTag, StringComparer.OrdinalIgnoreCase);
            foreach (var group in groups.OrderBy(p => p.FirstOrDefault().CodeTag, StringComparer.OrdinalIgnoreCase))
            {
                bool hasCodeTag = !string.IsNullOrWhiteSpace(group.FirstOrDefault().CodeTag);
                if (!hasCodeTag)
                {
                    var namespaces = group.Where(p => p.ValueType == ValueType.Namespace);
                    var variables = group.Where(p => p.ValueType == ValueType.Variable);
                    var other = group.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace);
                    var code = namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode)).Concat(other.Select(p => p.OriginalCode));
                    appendLine(sb, code);
                }
                else
                {
                    var namespaces = group.Where(p => p.ValueType == ValueType.Namespace);
                    var variables = definitions.Where(p => p.ValueType == ValueType.Variable && !string.IsNullOrWhiteSpace(p.CodeTag));
                    var other = group.Where(p => p.ValueType != ValueType.Variable && p.ValueType != ValueType.Namespace);
                    var vars = namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode));
                    var code = other.Select(p => p.OriginalCode);
                    mergeCode(sb, group.FirstOrDefault().CodeTag, group.FirstOrDefault().CodeSeparator, vars, code);
                }
            }

            copy.Code = sb.ToString();
            return copy;
        }

        #endregion Methods
    }
}
