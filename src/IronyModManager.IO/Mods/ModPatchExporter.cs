﻿// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 08-11-2020
// ***********************************************************************
// <copyright file="ModPatchExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using Nito.AsyncEx;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModPatchExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModPatchExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModPatchExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModPatchExporter : IModPatchExporter
    {
        #region Fields

        /// <summary>
        /// The cache external code key
        /// </summary>
        private const string CacheExternalCodeKey = "ExternallyLoadedCode";

        /// <summary>
        /// The cache state key
        /// </summary>
        private const string CacheStateKey = "PatchState";

        /// <summary>
        /// The cache state prefix
        /// </summary>
        private const string CacheStatePrefix = "ModPatchExporter";

        /// <summary>
        /// The state backup
        /// </summary>
        private const string StateBackup = StateName + ".bak";

        /// <summary>
        /// The state conflict history extension
        /// </summary>
        private const string StateConflictHistoryExtension = ".txt";

        /// <summary>
        /// The state history
        /// </summary>
        private const string StateHistory = "state_conflict_history";

        /// <summary>
        /// The state name
        /// </summary>
        private const string StateName = "state" + Shared.Constants.JsonExtension;

        /// <summary>
        /// The write lock
        /// </summary>
        private static readonly AsyncLock writeLock = new AsyncLock();

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache;

        /// <summary>
        /// The definition information providers
        /// </summary>
        private readonly IEnumerable<IDefinitionInfoProvider> definitionInfoProviders;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        /// <summary>
        /// The write counter
        /// </summary>
        private int writeCounter = 0;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModPatchExporter" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="messageBus">The message bus.</param>
        public ModPatchExporter(ICache cache, IReader reader, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IMessageBus messageBus)
        {
            this.cache = cache;
            this.definitionInfoProviders = definitionInfoProviders;
            this.reader = reader;
            this.messageBus = messageBus;
        }

        #endregion Constructors

        #region Enums

        /// <summary>
        /// Enum FileNameGeneration
        /// </summary>
        private enum FileNameGeneration
        {
            /// <summary>
            /// The generate file name
            /// </summary>
            GenerateFileName,

            /// <summary>
            /// The use existing file name
            /// </summary>
            UseExistingFileName,

            /// <summary>
            /// The use existing file name and write empty files
            /// </summary>
            UseExistingFileNameAndWriteEmptyFiles
        }

        #endregion Enums

        #region Methods

        /// <summary>
        /// Copies the patch mod asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CopyPatchModAsync(ModPatchExporterParameters parameters)
        {
            return CopyPathModInternalAsync(parameters);
        }

        /// <summary>
        /// export definition as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentNullException">Game.</exception>
        /// <exception cref="ArgumentNullException">Definitions.</exception>
        public async Task<bool> ExportDefinitionAsync(ModPatchExporterParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Game))
            {
                throw new ArgumentNullException("Game.");
            }
            var definitionsInvalid = (parameters.Definitions == null || parameters.Definitions.Count() == 0) &&
                (parameters.OrphanConflicts == null || parameters.OrphanConflicts.Count() == 0) &&
                (parameters.OverwrittenConflicts == null || parameters.OverwrittenConflicts.Count() == 0) &&
                (parameters.CustomConflicts == null || parameters.CustomConflicts.Count() == 0);
            if (definitionsInvalid)
            {
                throw new ArgumentNullException("Definitions.");
            }
            var definitionInfoProvider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(parameters.Game));
            if (definitionInfoProvider != null)
            {
                var results = new List<bool>();

                if (parameters.Definitions?.Count() > 0)
                {
                    results.Add(await CopyBinariesAsync(parameters.Definitions.Where(p => p.ValueType == Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), false));
                    results.Add(await WriteMergedContentAsync(parameters.Definitions.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game, false, FileNameGeneration.GenerateFileName));
                }

                if (parameters.OrphanConflicts?.Count() > 0)
                {
                    results.Add(await CopyBinariesAsync(parameters.OrphanConflicts.Where(p => p.ValueType == Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), true));
                    results.Add(await WriteMergedContentAsync(parameters.OrphanConflicts.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game, true, FileNameGeneration.GenerateFileName));
                }

                if (parameters.OverwrittenConflicts?.Count() > 0)
                {
                    results.Add(await CopyBinariesAsync(parameters.OverwrittenConflicts.Where(p => p.ValueType == Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), true));
                    results.Add(await WriteMergedContentAsync(parameters.OverwrittenConflicts.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game, true, FileNameGeneration.UseExistingFileNameAndWriteEmptyFiles));
                }

                if (parameters.CustomConflicts?.Count() > 0)
                {
                    results.Add(await WriteMergedContentAsync(parameters.CustomConflicts.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game, true, FileNameGeneration.UseExistingFileName));
                }
                return results.All(p => p);
            }
            return false;
        }

        /// <summary>
        /// Gets the patch files.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetPatchFiles(ModPatchExporterParameters parameters)
        {
            var path = GetPatchRootPath(parameters.RootPath, parameters.PatchName);
            var files = new List<string>();
            if (Directory.Exists(path))
            {
                foreach (var item in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    var relativePath = item.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                    if (relativePath.Contains(Path.DirectorySeparatorChar) && !relativePath.Contains(StateHistory, StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(relativePath);
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// get patch state as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="loadExternalCode">if set to <c>true</c> [load external code].</param>
        /// <returns>IPatchState.</returns>
        public async Task<IPatchState> GetPatchStateAsync(ModPatchExporterParameters parameters, bool loadExternalCode = true)
        {
            return await GetPatchStateInternalAsync(parameters, loadExternalCode);
        }

        /// <summary>
        /// rename patch mod as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> RenamePatchModAsync(ModPatchExporterParameters parameters)
        {
            var result = await CopyPathModInternalAsync(parameters);
            if (result)
            {
                var oldPath = Path.Combine(parameters.RootPath, parameters.ModPath);
                if (Directory.Exists(oldPath))
                {
                    Directory.Delete(oldPath, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Resets the cache.
        /// </summary>
        public void ResetCache()
        {
            cache.Invalidate(CacheStatePrefix, CacheStateKey);
        }

        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SaveStateAsync(ModPatchExporterParameters parameters)
        {
            var state = await GetPatchStateInternalAsync(parameters, true);
            if (state == null)
            {
                state = DIResolver.Get<IPatchState>();
            }
            var modifiedHistory = new List<IDefinition>();
            var path = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName));
            state.IgnoreConflictPaths = parameters.IgnoreConflictPaths;
            state.ResolvedConflicts = MapDefinitions(parameters.ResolvedConflicts, false);
            state.Conflicts = MapDefinitions(parameters.Conflicts, false);
            state.OrphanConflicts = MapDefinitions(parameters.OrphanConflicts, false);
            state.IgnoredConflicts = MapDefinitions(parameters.IgnoredConflicts, false);
            state.OverwrittenConflicts = MapDefinitions(parameters.OverwrittenConflicts, false);
            state.CustomConflicts = MapDefinitions(parameters.CustomConflicts, false);
            state.Mode = parameters.Mode;
            var history = state.ConflictHistory != null ? state.ConflictHistory.ToList() : new List<IDefinition>();
            var indexed = DIResolver.Get<IIndexedDefinitions>();
            indexed.InitMap(history);
            if (parameters.ResolvedConflicts != null)
            {
                foreach (var item in parameters.ResolvedConflicts.Where(s => !string.IsNullOrWhiteSpace(s.Code)))
                {
                    var existingHits = indexed.GetByTypeAndId(item.TypeAndId).ToList();
                    var existing = existingHits.FirstOrDefault(p => item.Code.Equals(p.Code));
                    if (existing == null)
                    {
                        history.RemoveAll(p => existingHits.Any(x => p.Equals(x)));
                        history.Add(item);
                        modifiedHistory.Add(item);
                    }
                    else
                    {
                        existingHits.Remove(existing);
                        history.RemoveAll(p => existingHits.Any(x => p.Equals(x)));
                    }
                }
            }
            if (parameters.Definitions != null)
            {
                foreach (var item in parameters.Definitions.Where(s => !string.IsNullOrWhiteSpace(s.Code) && !modifiedHistory.Any(p => p.TypeAndId.Equals(s.TypeAndId))))
                {
                    var existingHits = indexed.GetByTypeAndId(item.TypeAndId).ToList();
                    history.RemoveAll(p => existingHits.Any(x => p.Equals(x)));
                    history.Add(item);
                    modifiedHistory.Add(item);
                }
            }
            state.ConflictHistory = MapDefinitions(history, true);
            var externallyLoadedCode = cache.Get<HashSet<string>>(CacheStatePrefix, CacheExternalCodeKey);
            if (externallyLoadedCode == null)
            {
                externallyLoadedCode = new HashSet<string>();
                cache.Set(CacheStatePrefix, CacheExternalCodeKey, externallyLoadedCode);
            }
            return StoreState(state, modifiedHistory, externallyLoadedCode, path);
        }

        /// <summary>
        /// Standardizes the definition paths.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        protected virtual void StandardizeDefinitionPaths(IEnumerable<IDefinition> definitions)
        {
            static IList<string> standardizeArray(IList<string> paths)
            {
                if (paths?.Count() > 0)
                {
                    var newPaths = new List<string>();
                    foreach (var item in paths)
                    {
                        newPaths.Add(item.StandardizeDirectorySeparator());
                    }
                    return newPaths;
                }
                return paths;
            }

            if (definitions?.Count() > 0)
            {
                foreach (var item in definitions)
                {
                    item.AdditionalFileNames = standardizeArray(item.AdditionalFileNames);
                    item.File = item.File.StandardizeDirectorySeparator();
                    item.GeneratedFileNames = standardizeArray(item.GeneratedFileNames);
                    item.ModPath = item.ModPath.StandardizeDirectorySeparator();
                    item.OverwrittenFileNames = standardizeArray(item.OverwrittenFileNames);
                    item.Type = item.Type.StandardizeDirectorySeparator();
                }
            }
        }

        /// <summary>
        /// Copies the binaries asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="patchRootPath">The patch root path.</param>
        /// <param name="checkIfExists">The check if exists.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> CopyBinariesAsync(IEnumerable<IDefinition> definitions, string patchRootPath, bool checkIfExists)
        {
            var tasks = new List<Task>();
            var streams = new List<Stream>();

            var retry = new RetryStrategy();

            static async Task<bool> copyStream(Stream s, FileStream fs)
            {
                await s.CopyToAsync(fs);
                return true;
            }

            foreach (var def in definitions)
            {
                var outPath = Path.Combine(patchRootPath, def.File);
                if (checkIfExists && File.Exists(outPath))
                {
                    continue;
                }
                var stream = reader.GetStream(def.ModPath, def.File);
                // If image and no stream try switching extension
                if (Shared.Constants.ImageExtensions.Any(s => def.File.EndsWith(s, StringComparison.OrdinalIgnoreCase)) && stream == null)
                {
                    var segments = def.File.Split(".", StringSplitOptions.RemoveEmptyEntries);
                    var file = string.Join(".", segments.Take(segments.Count() - 1));
                    foreach (var item in Shared.Constants.ImageExtensions)
                    {
                        stream = reader.GetStream(def.ModPath, file + item);
                        if (stream != null)
                        {
                            break;
                        }
                    }
                }
                if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                }
                var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                tasks.Add(retry.RetryActionAsync(() =>
                {
                    return copyStream(stream, fs);
                }));
                streams.Add(stream);
                streams.Add(fs);
            }
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
                foreach (var fs in streams)
                {
                    fs.Close();
                    await fs.DisposeAsync();
                }
            }
            return true;
        }

        /// <summary>
        /// copy path mod internal as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> CopyPathModInternalAsync(ModPatchExporterParameters parameters)
        {
            var oldPath = Path.Combine(parameters.RootPath, parameters.ModPath);
            var newPath = Path.Combine(parameters.RootPath, parameters.PatchName);
            if (Directory.Exists(oldPath))
            {
                var files = Directory.EnumerateFiles(oldPath, "*", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    var info = new System.IO.FileInfo(item);
                    var destinationPath = Path.Combine(newPath, info.FullName.Replace(oldPath, string.Empty, StringComparison.OrdinalIgnoreCase).TrimStart(Path.DirectorySeparatorChar));
                    if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    }
                    info.CopyTo(destinationPath, true);
                }
                var backups = new List<string>() { Path.Combine(newPath, StateName), Path.Combine(newPath, StateBackup) };
                foreach (var item in backups)
                {
                    if (File.Exists(item))
                    {
                        var text = await File.ReadAllTextAsync(item);
                        text = text.Replace($"\"{parameters.ModPath}\"", $"\"{parameters.PatchName}\"");
                        await File.WriteAllTextAsync(item, text);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the patch root path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <returns>System.String.</returns>
        private string GetPatchRootPath(string path, string patchName)
        {
            return Path.Combine(path, patchName);
        }

        /// <summary>
        /// Gets the state of the patch.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IPatchState.</returns>
        private IPatchState GetPatchState(string path)
        {
            var cachedItem = cache.Get<CachedState>(CacheStatePrefix, CacheStateKey);
            if (cachedItem != null)
            {
                var lastPath = cachedItem.LastCachedPath ?? string.Empty;
                if (!lastPath.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    ResetCache();
                    return null;
                }
                return cachedItem.PatchState;
            }
            return null;
        }

        /// <summary>
        /// Gets the patch state internal asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="loadExternalCode">The load external code.</param>
        /// <returns>System.Threading.Tasks.Task&lt;IronyModManager.IO.Common.Mods.Models.IPatchState&gt;.</returns>
        private async Task<IPatchState> GetPatchStateInternalAsync(ModPatchExporterParameters parameters, bool loadExternalCode)
        {
            var statePath = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName), StateName);
            var cached = GetPatchState(statePath);
            if (File.Exists(statePath) && cached == null)
            {
                using var mutex = await writeLock.LockAsync();
                var text = await File.ReadAllTextAsync(statePath);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    cached = JsonDISerializer.Deserialize<IPatchState>(text);
                    if (string.IsNullOrEmpty(cached.IgnoreConflictPaths))
                    {
                        cached.IgnoreConflictPaths = string.Empty;
                    }
                    if (cached.ConflictHistory == null)
                    {
                        cached.ConflictHistory = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.ConflictHistory);
                    }
                    if (cached.Conflicts == null)
                    {
                        cached.Conflicts = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.Conflicts);
                    }
                    if (cached.IgnoredConflicts == null)
                    {
                        cached.IgnoredConflicts = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.IgnoredConflicts);
                    }
                    if (cached.OrphanConflicts == null)
                    {
                        cached.OrphanConflicts = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.OrphanConflicts);
                    }
                    if (cached.ResolvedConflicts == null)
                    {
                        cached.ResolvedConflicts = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.ResolvedConflicts);
                    }
                    if (cached.OverwrittenConflicts == null)
                    {
                        cached.OverwrittenConflicts = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.OverwrittenConflicts);
                    }
                    if (cached.CustomConflicts == null)
                    {
                        cached.CustomConflicts = new List<IDefinition>();
                    }
                    else
                    {
                        StandardizeDefinitionPaths(cached.CustomConflicts);
                    }
                    // If not allowing full load don't cache anything
                    if (loadExternalCode)
                    {
                        var externallyLoadedCode = new HashSet<string>();
                        async Task loadCode(IDefinition definition)
                        {
                            var historyPath = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName), StateHistory, definition.Type, definition.Id.GenerateValidFileName() + StateConflictHistoryExtension);
                            if (File.Exists(historyPath))
                            {
                                var code = await File.ReadAllTextAsync(historyPath);
                                definition.Code = string.Join(Environment.NewLine, code.SplitOnNewLine());
                                externallyLoadedCode.Add(definition.TypeAndId);
                            }
                        }
                        var tasks = new List<Task>();
                        foreach (var item in cached.ConflictHistory)
                        {
                            tasks.Add(loadCode(item));
                        }
                        var cachedItem = new CachedState()
                        {
                            LastCachedPath = statePath,
                            PatchState = cached
                        };
                        cache.Set(CacheStatePrefix, CacheStateKey, cachedItem);
                        cache.Set(CacheStatePrefix, CacheExternalCodeKey, externallyLoadedCode);
                        await Task.WhenAll(tasks);
                    }
                }
                mutex.Dispose();
            }
            if (cached != null)
            {
                var result = DIResolver.Get<IPatchState>();
                MapPatchState(cached, result, true);
                return result;
            }
            return null;
        }

        /// <summary>
        /// Maps the definition.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="includeCode">if set to <c>true</c> [include code].</param>
        /// <returns>IDefinition.</returns>
        private IDefinition MapDefinition(IDefinition original, bool includeCode)
        {
            var newInstance = DIResolver.Get<IDefinition>();
            if (includeCode)
            {
                // Most don't require except conflict history. Further reduces the json.
                newInstance.Code = original.Code;
            }
            newInstance.ContentSHA = original.ContentSHA;
            newInstance.DefinitionSHA = original.DefinitionSHA;
            newInstance.Dependencies = original.Dependencies;
            newInstance.File = original.File;
            newInstance.Id = original.Id;
            newInstance.ModName = original.ModName;
            newInstance.Type = original.Type;
            newInstance.UsedParser = original.UsedParser;
            newInstance.ValueType = original.ValueType;
            newInstance.ErrorColumn = original.ErrorColumn;
            newInstance.ErrorLine = original.ErrorLine;
            newInstance.ErrorMessage = original.ErrorMessage;
            newInstance.GeneratedFileNames = original.GeneratedFileNames;
            newInstance.AdditionalFileNames = original.AdditionalFileNames;
            newInstance.OverwrittenFileNames = original.OverwrittenFileNames;
            newInstance.OriginalCode = original.OriginalCode;
            newInstance.CodeSeparator = original.CodeSeparator;
            newInstance.CodeTag = original.CodeTag;
            newInstance.Order = original.Order;
            return newInstance;
        }

        /// <summary>
        /// Maps the definitions.
        /// </summary>
        /// <param name="originals">The originals.</param>
        /// <param name="includeCode">if set to <c>true</c> [include code].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        private IEnumerable<IDefinition> MapDefinitions(IEnumerable<IDefinition> originals, bool includeCode)
        {
            var col = new List<IDefinition>();
            if (originals != null)
            {
                foreach (var original in originals)
                {
                    col.Add(MapDefinition(original, includeCode));
                }
            }
            return col;
        }

        /// <summary>
        /// Maps the state of the patch.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="includeCode">if set to <c>true</c> [include code].</param>
        private void MapPatchState(IPatchState source, IPatchState destination, bool includeCode)
        {
            destination.ConflictHistory = MapDefinitions(source.ConflictHistory, includeCode);
            destination.Conflicts = MapDefinitions(source.Conflicts, includeCode);
            destination.IgnoreConflictPaths = source.IgnoreConflictPaths;
            destination.IgnoredConflicts = MapDefinitions(source.IgnoredConflicts, includeCode);
            destination.OrphanConflicts = MapDefinitions(source.OrphanConflicts, includeCode);
            destination.ResolvedConflicts = MapDefinitions(source.ResolvedConflicts, includeCode);
            destination.OverwrittenConflicts = MapDefinitions(source.OverwrittenConflicts, includeCode);
            destination.CustomConflicts = MapDefinitions(source.CustomConflicts, includeCode);
            destination.Mode = source.Mode;
        }

        /// <summary>
        /// Stores the state.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="modifiedHistory">The modified history.</param>
        /// <param name="externalCode">The external code.</param>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool StoreState(IPatchState model, IEnumerable<IDefinition> modifiedHistory, HashSet<string> externalCode, string path)
        {
            var statePath = Path.Combine(path, StateName);

            var cachedItem = cache.Get<CachedState>(CacheStatePrefix, CacheStateKey);
            if (cachedItem == null)
            {
                cachedItem = new CachedState();
            }
            cachedItem.LastCachedPath = statePath;
            cachedItem.PatchState = model;
            cache.Set(CacheStatePrefix, CacheStateKey, cachedItem);

            WriteStateInBackground(model, modifiedHistory, externalCode, path).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Writes the merged content asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="patchRootPath">The patch root path.</param>
        /// <param name="game">The game.</param>
        /// <param name="checkIfFileExists">The check if file exists.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> WriteMergedContentAsync(IEnumerable<IDefinition> definitions, string patchRootPath, string game, bool checkIfFileExists, FileNameGeneration mode)
        {
            var tasks = new List<Task>();
            List<bool> results = new List<bool>();
            var validDefinitions = definitions.Where(p => p.ValueType != Parser.Common.ValueType.Namespace && p.ValueType != Parser.Common.ValueType.Variable);
            var retry = new RetryStrategy();
            async Task evalZeroByteFiles(IDefinition definition, IDefinitionInfoProvider infoProvider, string fileName)
            {
                if (mode == FileNameGeneration.UseExistingFileNameAndWriteEmptyFiles)
                {
                    var emptyFileNames = definition.OverwrittenFileNames.Where(p => p != fileName);
                    foreach (var emptyFile in emptyFileNames)
                    {
                        var emptyPath = Path.Combine(patchRootPath, emptyFile);
                        await retry.RetryActionAsync(async () =>
                        {
                            await File.WriteAllTextAsync(emptyPath, string.Empty, infoProvider.GetEncoding(definition));
                            return true;
                        });
                    }
                }
            }

            foreach (var item in validDefinitions)
            {
                var infoProvider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(game));
                if (infoProvider != null)
                {
                    string fileName = string.Empty;
                    fileName = mode switch
                    {
                        FileNameGeneration.GenerateFileName => infoProvider.GetFileName(item),
                        _ => item.File,
                    };
                    var outPath = Path.Combine(patchRootPath, fileName);
                    if (checkIfFileExists && File.Exists(outPath))
                    {
                        // Zero byte files could still not be present
                        await evalZeroByteFiles(item, infoProvider, fileName);
                        continue;
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                    }
                    // Update filename
                    item.File = fileName;
                    tasks.Add(retry.RetryActionAsync(async () =>
                    {
                        await File.WriteAllTextAsync(outPath, item.Code, infoProvider.GetEncoding(item));
                        return true;
                    }));
                    await evalZeroByteFiles(item, infoProvider, fileName);
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            return results.All(p => p);
        }

        /// <summary>
        /// Writes the state in background.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="modifiedHistory">The modified history.</param>
        /// <param name="externalCode">The external code.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        private async Task WriteStateInBackground(IPatchState model, IEnumerable<IDefinition> modifiedHistory, HashSet<string> externalCode, string path)
        {
            writeCounter++;
            var mutex = await writeLock.LockAsync();
            await messageBus.PublishAsync(new WritingStateOperationEvent(writeCounter <= 0));
            var statePath = Path.Combine(path, StateName);
            var backupPath = Path.Combine(path, StateBackup);
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
            if (File.Exists(statePath))
            {
                File.Copy(statePath, backupPath);
            }
            await Task.Factory.StartNew(async () =>
            {
                var retry = new RetryStrategy();
                var patchState = DIResolver.Get<IPatchState>();
                MapPatchState(model, patchState, true);
                foreach (var item in patchState.ConflictHistory)
                {
                    if (externalCode != null && externalCode.Contains(item.TypeAndId))
                    {
                        item.Code = null;
                    }
                }

                var loadedCode = new HashSet<string>();
                foreach (var item in modifiedHistory)
                {
                    var historyPath = Path.Combine(path, StateHistory, item.Type, item.Id.GenerateValidFileName() + StateConflictHistoryExtension);
                    var historyDirectory = Path.GetDirectoryName(historyPath);
                    if (!Directory.Exists(historyDirectory))
                    {
                        Directory.CreateDirectory(historyDirectory);
                    }
                    if (externalCode != null && !externalCode.Contains(item.TypeAndId))
                    {
                        loadedCode.Add(item.TypeAndId);
                        var existing = patchState.ConflictHistory.FirstOrDefault(p => p.TypeAndId.Equals(item.TypeAndId));
                        if (existing != null)
                        {
                            existing.Code = null;
                        }
                    }
                    await retry.RetryActionAsync(async () =>
                    {
                        await File.WriteAllTextAsync(historyPath, item.Code);
                        return true;
                    });
                }

                var existingLoadedCode = cache.Get<HashSet<string>>(CacheStatePrefix, CacheExternalCodeKey);
                if (existingLoadedCode != null)
                {
                    foreach (var item in loadedCode)
                    {
                        existingLoadedCode.Add(item);
                    }
                    cache.Set(CacheStatePrefix, CacheExternalCodeKey, existingLoadedCode);
                }

                var dirPath = Path.GetDirectoryName(statePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var serialized = JsonDISerializer.Serialize(patchState);
                await retry.RetryActionAsync(async () =>
                {
                    await File.WriteAllTextAsync(statePath, serialized);
                    return true;
                });
                writeCounter--;
                await messageBus.PublishAsync(new WritingStateOperationEvent(writeCounter <= 0));
                mutex.Dispose();
            }).ConfigureAwait(false);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class CachedState.
        /// </summary>
        private class CachedState
        {
            #region Properties

            /// <summary>
            /// Gets or sets the last cached path.
            /// </summary>
            /// <value>The last cached path.</value>
            public string LastCachedPath { get; set; }

            /// <summary>
            /// Gets or sets the state of the patch.
            /// </summary>
            /// <value>The state of the patch.</value>
            public IPatchState PatchState { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}