// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-12-2020
// ***********************************************************************
// <copyright file="ModService.cs" company="Mario">
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
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModService" />
    public class ModService : ModBaseService, IModService
    {
        #region Fields

        /// <summary>
        /// The service lock
        /// </summary>
        private static readonly object serviceLock = new { };

        /// <summary>
        /// The mod parser
        /// </summary>
        private readonly IModParser modParser;

        /// <summary>
        /// The mod patch exporter
        /// </summary>
        private readonly IModPatchExporter modPatchExporter;

        /// <summary>
        /// The mod writer
        /// </summary>
        private readonly IModWriter modWriter;

        /// <summary>
        /// The parser manager
        /// </summary>
        private readonly IParserManager parserManager;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModService" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModService(IReader reader, IParserManager parserManager,
            IModParser modParser, IModWriter modWriter, IModPatchExporter modPatchExporter, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(gameService, storageProvider, mapper)
        {
            this.reader = reader;
            this.parserManager = parserManager;
            this.modParser = modParser;
            this.modWriter = modWriter;
            this.modPatchExporter = modPatchExporter;
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
        public virtual async Task<bool> ApplyModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName)
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
                    await modWriter.WriteDescriptorAsync(new ModWriterParameters()
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
                    conflictResult.ResolvedConflicts.AddToMap(definition);
                    var patches = GetDefinitionsToWrite(conflictResult, definition);
                    await modWriter.CreateModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = game.UserDirectory,
                        Path = Path.Combine(Constants.ModDirectory, patchName)
                    });
                    await modPatchExporter.SaveStateAsync(new ModPatchExporterParameters()
                    {
                        Conflicts = GetDefinitionOrDefault(conflictResult.Conflicts),
                        OrphanConflicts = GetDefinitionOrDefault(conflictResult.OrphanConflicts),
                        ResolvedConflicts = GetDefinitionOrDefault(conflictResult.ResolvedConflicts),
                        RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                        PatchName = patchName
                    });
                    await modWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = Path.Combine(game.UserDirectory, Constants.ModDirectory, patchName),
                        Path = definition.ParentDirectory
                    });
                    var allPatches = new HashSet<IDefinition>(patches);
                    foreach (var item in conflictResult.ResolvedConflicts.GetByParentDirectory(definition.ParentDirectory))
                    {
                        if (!allPatches.Contains(item))
                        {
                            allPatches.Add(item);
                        }
                    }
                    foreach (var item in conflictResult.OrphanConflicts.GetAll())
                    {
                        if (!allPatches.Contains(item))
                        {
                            allPatches.Add(item);
                        }
                    }
                    await modWriter.ApplyModsAsync(new ModWriterParameters()
                    {
                        AppendOnly = true,
                        HiddenMods = new List<IMod>() { mod },
                        RootDirectory = game.UserDirectory
                    });
                    return await modPatchExporter.ExportDefinitionAsync(new ModPatchExporterParameters()
                    {
                        Game = game.Type,
                        Definitions = allPatches,
                        RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                        ModPath = definitionMod.FileName,
                        PatchName = patchName
                    });
                }
            }
            return false;
        }

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        public virtual string BuildModUrl(IMod mod)
        {
            if (!mod.RemoteId.HasValue)
            {
                return string.Empty;
            }
            return mod.Source switch
            {
                ModSource.Steam => string.Format(Constants.Steam_Url, mod.RemoteId),
                ModSource.Paradox => string.Format(Constants.Paradox_Url, mod.RemoteId),
                _ => string.Empty,
            };
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
        /// delete descriptors as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> DeleteDescriptorsAsync(IEnumerable<IMod> mods)
        {
            var game = GameService.GetSelected();
            if (game != null && mods?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var item in mods)
                {
                    var task = modWriter.DeleteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = item,
                        RootDirectory = game.UserDirectory
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                return true;
            }
            return false; ;
        }

        /// <summary>
        /// Exports the mods asynchronous.
        /// </summary>
        /// <param name="enabledMods">The mods.</param>
        /// <param name="regularMods">The regular mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ExportModsAsync(IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null || enabledMods == null || regularMods == null)
            {
                return false;
            }
            var allMods = GetInstalledModsInternal(game, false);
            var mod = GeneratePatchModDescriptor(allMods, game, GenerateCollectionPatchName(collectionName));
            var applyModParams = new ModWriterParameters()
            {
                OtherMods = regularMods.Where(p => !enabledMods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile))).ToList(),
                EnabledMods = enabledMods,
                RootDirectory = game.UserDirectory
            };
            if (await modWriter.DescriptorExistsAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Mod = mod
            }))
            {
                applyModParams.HiddenMods = new List<IMod>() { mod };
            }
            return await modWriter.ApplyModsAsync(applyModParams);
        }

        /// <summary>
        /// Finds the conflicts.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public virtual IConflictResult FindConflicts(IIndexedDefinitions indexedDefinitions)
        {
            var conflicts = new HashSet<IDefinition>();
            var fileKeys = indexedDefinitions.GetAllFileKeys();
            var typeAndIdKeys = indexedDefinitions.GetAllTypeAndIdKeys();

            double total = fileKeys.Count() + typeAndIdKeys.Count();
            double processed = 0;

            foreach (var item in fileKeys)
            {
                var definitions = indexedDefinitions.GetByFile(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions, false);
                processed++;
                ModDefinitionAnalyze?.Invoke(Convert.ToInt32(processed / total * 100));
            }

            foreach (var item in typeAndIdKeys)
            {
                var definitions = indexedDefinitions.GetByTypeAndId(item);
                EvalDefinitions(indexedDefinitions, conflicts, definitions, true);
                processed++;
                ModDefinitionAnalyze?.Invoke(Convert.ToInt32(processed / total * 100));
            }

            var groupedConflicts = conflicts.GroupBy(p => p.TypeAndId);
            var result = GetModelInstance<IConflictResult>();
            var conflictsIndexed = DIResolver.Get<IIndexedDefinitions>();
            conflictsIndexed.InitMap(groupedConflicts.Where(p => p.Count() > 1).SelectMany(p => p), true);
            var orphanedConflictsIndexed = DIResolver.Get<IIndexedDefinitions>();
            orphanedConflictsIndexed.InitMap(groupedConflicts.Where(p => p.Count() == 1).SelectMany(p => p), true);
            result.AllConflicts = indexedDefinitions;
            result.Conflicts = conflictsIndexed;
            result.OrphanConflicts = orphanedConflictsIndexed;
            var resolvedConflicts = DIResolver.Get<IIndexedDefinitions>();
            resolvedConflicts.InitMap(null, true);
            result.ResolvedConflicts = resolvedConflicts;

            return result;
        }

        /// <summary>
        /// Gets the definitions to write.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public virtual IEnumerable<IDefinition> GetDefinitionsToWrite(IConflictResult conflictResult, IDefinition definition)
        {
            if (definition.ValueType != Parser.Common.ValueType.Object && definition.ValueType != Parser.Common.ValueType.Variable)
            {
                return new List<IDefinition>() { definition };
            }
            var definitions = new List<IDefinition>() { definition };
            var resolvedConflicts = FilterValidWriteDefinitions(conflictResult.ResolvedConflicts, definition);
            var conflicts = FilterValidWriteDefinitions(conflictResult.Conflicts, definition).Where(p => !resolvedConflicts.Any(c => c.Id.Equals(p.Id)));
            List<IDefinition> allConflicts = new List<IDefinition>();
            if (definition.ValueType == Parser.Common.ValueType.Variable)
            {
                foreach (var item in conflictResult.Conflicts.GetByTypeAndId(definition.TypeAndId))
                {
                    allConflicts.AddRange(FilterValidWriteDefinitions(conflictResult.AllConflicts, item).Where(p => !conflicts.Any(c => c.Id.Equals(p.Id)) && !allConflicts.Any(c => c.Id.Equals(p.Id))));
                }
            }
            else
            {
                allConflicts = FilterValidWriteDefinitions(conflictResult.AllConflicts, definition).Where(p => !conflicts.Any(c => c.Id.Equals(p.Id))).ToList();
            }
            definitions.AddRange(allConflicts.GroupBy(p => p.Id).Select(p => p.First()));
            var orphanConflicts = FilterValidWriteDefinitions(conflictResult.OrphanConflicts, definition).Where(p => !allConflicts.Any(c => c.Id.Equals(p.Id)));
            definitions.AddRange(orphanConflicts.GroupBy(p => p.Id).Select(p => p.First()));
            return definitions;
        }

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IModObject&gt;.</returns>
        /// <exception cref="ArgumentNullException">game</exception>
        public virtual IEnumerable<IMod> GetInstalledMods(IGame game)
        {
            return GetInstalledModsInternal(game, true);
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

            mods.AsParallel().ForAll((m) =>
            {
                IEnumerable<IDefinition> result = null;
                if (Path.IsPathFullyQualified(m.FileName))
                {
                    result = ParseModFiles(game, reader.Read(m.FileName), m);
                }
                else
                {
                    // Check user directory and workshop directory.
                    var userDirectoryMod = Path.Combine(game.UserDirectory, m.FileName);
                    var workshopDirectoryMod = Path.Combine(game.WorkshopDirectory, m.FileName);
                    if (File.Exists(userDirectoryMod) || Directory.Exists(userDirectoryMod))
                    {
                        result = ParseModFiles(game, reader.Read(userDirectoryMod), m);
                    }
                    else if (File.Exists(workshopDirectoryMod) || Directory.Exists(workshopDirectoryMod))
                    {
                        result = ParseModFiles(game, reader.Read(workshopDirectoryMod), m);
                    }
                }
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
                    var perc = Convert.ToInt32((processed / total * 100) - 2);
                    if (perc < 0)
                    {
                        perc = 1;
                    }
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
        /// install mods as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> InstallModsAsync()
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            var mods = GetInstalledModsInternal(game, false);
            var descriptors = new List<IMod>();
            var userDirectoryMods = GetAllModDescriptors(Path.Combine(game.UserDirectory, Constants.ModDirectory), ModSource.Local);
            if (userDirectoryMods?.Count() > 0)
            {
                descriptors.AddRange(userDirectoryMods);
            }
            var workshopDirectoryMods = GetAllModDescriptors(game.WorkshopDirectory, ModSource.Steam);
            if (workshopDirectoryMods?.Count() > 0)
            {
                descriptors.AddRange(workshopDirectoryMods);
            }
            var diffs = descriptors.Where(p => !mods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile, StringComparison.OrdinalIgnoreCase))).ToList();
            if (diffs.Count > 0)
            {
                var tasks = new List<Task>();
                foreach (var diff in diffs)
                {
                    tasks.Add(modWriter.WriteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = diff,
                        RootDirectory = game.UserDirectory,
                        Path = diff.DescriptorFile
                    }));
                }
                await Task.WhenAll(tasks);
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
            if (mod != null)
            {
                return IsPatchMod(mod.Name);
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod name].
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod name]; otherwise, <c>false</c>.</returns>
        public virtual bool IsPatchMod(string modName)
        {
            if (!string.IsNullOrWhiteSpace(modName))
            {
                return modName.StartsWith(PatchCollectionName);
            }
            return false;
        }

        /// <summary>
        /// Loads the patch state asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        public virtual async Task<IConflictResult> LoadPatchStateAsync(IConflictResult conflictResult, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game != null && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName))
            {
                var patchName = GenerateCollectionPatchName(collectionName);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Constants.ModDirectory),
                    PatchName = patchName
                });
                if (state != null)
                {
                    var resolvedConflicts = new List<IDefinition>(state.ResolvedConflicts);
                    var total = state.Conflicts.Count() + state.OrphanConflicts.Count();
                    int processed = 0;
                    foreach (var item in state.OrphanConflicts.GroupBy(p => p.TypeAndId))
                    {
                        processed += item.Count();
                        var matchedConflicts = conflictResult.OrphanConflicts.GetByTypeAndId(item.First().TypeAndId);
                        await SyncPatchStatesAsync(matchedConflicts, item, patchName, game.UserDirectory);
                        ModDefinitionPatchLoad?.Invoke(Convert.ToInt32(processed / total * 100));
                    }
                    foreach (var item in state.Conflicts.GroupBy(p => p.TypeAndId))
                    {
                        processed += item.Count();
                        var matchedConflicts = conflictResult.Conflicts.GetByTypeAndId(item.First().TypeAndId);
                        var synced = await SyncPatchStatesAsync(matchedConflicts, item, patchName, game.UserDirectory);
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
                        ModDefinitionPatchLoad?.Invoke(Convert.ToInt32(processed / total * 100));
                    }
                    var conflicts = GetModelInstance<IConflictResult>();
                    conflicts.AllConflicts = conflictResult.AllConflicts;
                    conflicts.Conflicts = conflictResult.Conflicts;
                    conflicts.OrphanConflicts = conflictResult.OrphanConflicts;
                    var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                    resolvedIndex.InitMap(resolvedConflicts, true);
                    conflicts.ResolvedConflicts = resolvedIndex;
                    return conflicts;
                }
            };
            return null;
        }

        /// <summary>
        /// lock descriptors as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> LockDescriptorsAsync(IEnumerable<IMod> mods, bool isLocked)
        {
            var game = GameService.GetSelected();
            if (game != null && mods?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var item in mods)
                {
                    var task = modWriter.SetDescriptorLockAsync(new ModWriterParameters()
                    {
                        Mod = item,
                        RootDirectory = game.UserDirectory
                    }, isLocked);
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evals the definitions.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="conflicts">The conflicts.</param>
        /// <param name="definitions">The definitions.</param>
        /// <param name="evalShouldSkipVariables">if set to <c>true</c> [skip variables].</param>
        protected virtual void EvalDefinitions(IIndexedDefinitions indexedDefinitions, HashSet<IDefinition> conflicts, IEnumerable<IDefinition> definitions, bool evalShouldSkipVariables = false)
        {
            if (definitions.GroupBy(p => p.ModName.ToLowerInvariant()).Count() > 1)
            {
                var validDefinitions = new HashSet<IDefinition>();
                if (evalShouldSkipVariables && definitions.All(p => p.ValueType == Parser.Common.ValueType.Variable))
                {
                    // Must have at least one definition match per file
                    foreach (var def in definitions)
                    {
                        var fileDefs = indexedDefinitions.GetByFile(def.File);
                        foreach (var fileDef in fileDefs.Where(p => p.ValueType == Parser.Common.ValueType.Object).ToList())
                        {
                            var fileConflicts = indexedDefinitions.GetByTypeAndId(fileDef.TypeAndId);
                            if (fileConflicts.GroupBy(p => p.ModName.ToLowerInvariant()).Count() > 1)
                            {
                                var validDefs = definitions.Where(p => fileConflicts.Any(d => d.File.Equals(p.File)));
                                if (validDefs.Count() > 1)
                                {
                                    foreach (var item in validDefs)
                                    {
                                        if (!validDefinitions.Contains(item))
                                        {
                                            validDefinitions.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in definitions)
                    {
                        validDefinitions.Add(item);
                    }
                }
                var processed = new HashSet<IDefinition>();
                foreach (var def in validDefinitions)
                {
                    if (processed.Contains(def) || conflicts.Contains(def))
                    {
                        continue;
                    }
                    var allConflicts = indexedDefinitions.GetByTypeAndId(def.Type, def.Id);
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
                                var hasOverrides = allConflicts.Any(p => (p.Dependencies?.Any(p => p.Contains(conflict.ModName))).GetValueOrDefault());
                                if (hasOverrides)
                                {
                                    continue;
                                }
                                validConflicts.Add(conflict);
                            }

                            var validConflictsGroup = validConflicts.GroupBy(p => p.DefinitionSHA);
                            if (validConflictsGroup.Count() > 1)
                            {
                                var filteredConflicts = validConflictsGroup.Select(p => p.OrderBy(p => p.ModName).First());
                                foreach (var item in filteredConflicts)
                                {
                                    if (!conflicts.Contains(item) && item.ValueType != Parser.Common.ValueType.Namespace)
                                    {
                                        conflicts.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!conflicts.Contains(def) && def.ValueType != Parser.Common.ValueType.Namespace)
                        {
                            conflicts.Add(def);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Filters the valid write definitions.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> FilterValidWriteDefinitions(IIndexedDefinitions indexedDefinitions, IDefinition definition)
        {
            var conflicts = indexedDefinitions.GetByFile(definition.File);
            if (conflicts != null)
            {
                if (definition.ValueType == Parser.Common.ValueType.Variable)
                {
                    return conflicts.Where(p => !p.Id.Equals(definition.Id) && (p.ValueType == Parser.Common.ValueType.Object || p.ValueType == Parser.Common.ValueType.Variable || p.ValueType == Parser.Common.ValueType.Namespace));
                }
                return conflicts.Where(p => !p.Id.Equals(definition.Id) && (p.ValueType == Parser.Common.ValueType.Variable || p.ValueType == Parser.Common.ValueType.Namespace));
            }
            return new List<IDefinition>();
        }

        /// <summary>
        /// Generates the patch mod descriptor.
        /// </summary>
        /// <param name="allMods">All mods.</param>
        /// <param name="game">The game.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <returns>IMod.</returns>
        protected virtual IMod GeneratePatchModDescriptor(IEnumerable<IMod> allMods, IGame game, string patchName)
        {
            var mod = DIResolver.Get<IMod>();
            mod.Dependencies = allMods.Select(p => p.Name).ToList();
            mod.DescriptorFile = $"{Constants.ModDirectory}/{patchName}{Constants.ModExtension}";
            mod.FileName = Path.Combine(game.UserDirectory, Constants.ModDirectory, patchName).Replace("\\", "/");
            mod.Name = patchName;
            mod.Source = ModSource.Local;
            mod.Version = allMods.OrderByDescending(p => p.Version).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.Version).FirstOrDefault().Version : string.Empty;
            return mod;
        }

        /// <summary>
        /// Gets all mod descriptors.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="modSource">The mod source.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        protected virtual IEnumerable<IMod> GetAllModDescriptors(string path, ModSource modSource)
        {
            var files = Directory.Exists(path) ? Directory.EnumerateFiles(path, $"*{Shared.Constants.ZipExtension}") : new string[] { };
            var directories = Directory.Exists(path) ? Directory.EnumerateDirectories(path) : new string[] { };
            var mods = new List<IMod>();

            void parseModFiles(string path, ModSource source, bool isDirectory)
            {
                var fileInfo = reader.GetFileInfo(path, Constants.DescriptorFile);
                if (fileInfo == null)
                {
                    return;
                }
                var mod = Mapper.Map<IMod>(modParser.Parse(fileInfo.Content));
                mod.FileName = path.Replace("\\", "/");
                mod.Source = source;
                var cleanedPath = path;
                if (!isDirectory)
                {
                    cleanedPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                }

                switch (mod.Source)
                {
                    case ModSource.Local:
                        mod.DescriptorFile = $"{Constants.ModDirectory}/{cleanedPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()}{Constants.ModExtension}";
                        break;

                    case ModSource.Steam:
                        mod.DescriptorFile = $"{Constants.ModDirectory}/{Constants.Steam_mod_id}{mod.RemoteId}{Constants.ModExtension}";
                        break;

                    case ModSource.Paradox:
                        if (!isDirectory)
                        {
                            var modParentDirectory = Path.GetDirectoryName(path);
                            mod.RemoteId = GetPdxModId(modParentDirectory, isDirectory);
                        }
                        else
                        {
                            mod.RemoteId = GetPdxModId(path, isDirectory);
                        }
                        mod.DescriptorFile = $"{Constants.ModDirectory}/{Constants.Paradox_mod_id}{mod.RemoteId}{Constants.ModExtension}";
                        break;

                    default:
                        break;
                }
                mods.Add(mod);
            }
            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    parseModFiles(file, modSource, false);
                }
            }
            if (directories.Count() > 0)
            {
                foreach (var directory in directories)
                {
                    var modSourceOverride = directory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).
                            LastOrDefault().Contains(Constants.Paradox_mod_id, StringComparison.OrdinalIgnoreCase) ? ModSource.Paradox : modSource;
                    var zipFiles = Directory.EnumerateFiles(directory, $"*{Shared.Constants.ZipExtension}");
                    if (zipFiles.Count() > 0)
                    {
                        foreach (var zip in zipFiles)
                        {
                            parseModFiles(zip, modSourceOverride, false);
                        }
                    }
                    else
                    {
                        parseModFiles(directory, modSourceOverride, true);
                    }
                }
            }
            return mods;
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
        /// Gets the installed mods internal.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="ignorePatchMods">if set to <c>true</c> [ignore patch mods].</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        /// <exception cref="ArgumentNullException">game</exception>
        protected virtual IEnumerable<IMod> GetInstalledModsInternal(IGame game, bool ignorePatchMods)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            var result = new List<IMod>();
            var installedMods = reader.Read(Path.Combine(game.UserDirectory, Constants.ModDirectory));
            if (installedMods?.Count() > 0)
            {
                foreach (var installedMod in installedMods.Where(p => p.Content.Count() > 0))
                {
                    var mod = Mapper.Map<IMod>(modParser.Parse(installedMod.Content));
                    if (ignorePatchMods && IsPatchMod(mod))
                    {
                        continue;
                    }
                    mod.DescriptorFile = $"{Constants.ModDirectory}/{installedMod.FileName}";
                    mod.Source = GetModSource(installedMod);
                    if (mod.Source == ModSource.Paradox)
                    {
                        mod.RemoteId = GetPdxModId(installedMod.FileName);
                    }
                    result.Add(mod);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the mod source.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>ModSource.</returns>
        protected virtual ModSource GetModSource(IFileInfo fileInfo)
        {
            if (fileInfo.FileName.Contains(Constants.Paradox_mod_id))
            {
                return ModSource.Paradox;
            }
            else if (fileInfo.FileName.Contains(Constants.Steam_mod_id))
            {
                return ModSource.Steam;
            }
            return ModSource.Local;
        }

        /// <summary>
        /// Gets the PDX mod identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
        /// <returns>System.Int32.</returns>
        protected virtual int GetPdxModId(string path, bool isDirectory = false)
        {
            var name = !isDirectory ? Path.GetFileNameWithoutExtension(path) : path;
            int.TryParse(name.Replace(Constants.Paradox_mod_id, string.Empty), out var id);
            return id;
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
            var definitions = new List<IDefinition>();
            foreach (var fileInfo in fileInfos)
            {
                definitions.AddRange(parserManager.Parse(new ParserManagerArgs()
                {
                    ContentSHA = fileInfo.ContentSHA,
                    File = fileInfo.FileName,
                    GameType = game.Type,
                    Lines = fileInfo.Content,
                    ModDependencies = modObject.Dependencies,
                    ModName = modObject.Name
                }));
            }
            return definitions;
        }

        /// <summary>
        /// synchronize patch states as an asynchronous operation.
        /// </summary>
        /// <param name="currentConflicts">The current conflicts.</param>
        /// <param name="cachedConflicts">The cached conflicts.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="userDirectory">The user directory.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> SyncPatchStatesAsync(IEnumerable<IDefinition> currentConflicts, IEnumerable<IDefinition> cachedConflicts, string patchName, string userDirectory)
        {
            var cachedDiffs = cachedConflicts.Where(p => currentConflicts.Any(a => a.ModName.Equals(p.ModName) && a.File.Equals(p.File) && a.DefinitionSHA.Equals(p.DefinitionSHA)));
            if (cachedDiffs.Count() != cachedConflicts.Count())
            {
                foreach (var diff in cachedConflicts)
                {
                    await modWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                    {
                        RootDirectory = Path.Combine(userDirectory, Constants.ModDirectory, patchName),
                        Path = diff.File
                    });
                }
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}
