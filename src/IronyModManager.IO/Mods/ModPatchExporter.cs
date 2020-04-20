// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
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
using Flettu.Lock;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;

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
        /// The state backup
        /// </summary>
        private const string StateBackup = StateName + ".bak";

        /// <summary>
        /// The state name
        /// </summary>
        private const string StateName = "state" + Shared.Constants.JsonExtension;

        /// <summary>
        /// The write lock
        /// </summary>
        private static readonly AsyncLock writeLock = new AsyncLock();

        /// <summary>
        /// The cached state
        /// </summary>
        private static IPatchState cachedState;

        /// <summary>
        /// The last cached state path
        /// </summary>
        private static string lastCachedStatePath;

        /// <summary>
        /// The definition information providers
        /// </summary>
        private readonly IEnumerable<IDefinitionInfoProvider> definitionInfoProviders;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModPatchExporter" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        public ModPatchExporter(IReader reader, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders)
        {
            this.definitionInfoProviders = definitionInfoProviders;
            this.reader = reader;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [mod definition analyze].
        /// </summary>
        public event Delegates.WriteOperationStateDelegate WriteOperationState;

        #endregion Events

        #region Methods

        /// <summary>
        /// export definition as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentNullException">Game</exception>
        /// <exception cref="ArgumentNullException">Definitions.</exception>
        public async Task<bool> ExportDefinitionAsync(ModPatchExporterParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Game))
            {
                throw new ArgumentNullException("Game");
            }
            var definitionsInvalid = (parameters.Definitions == null || parameters.Definitions.Count() == 0) && (parameters.OrphanConflicts == null || parameters.OrphanConflicts.Count() == 0);
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
                        Path.Combine(parameters.RootPath, parameters.ModPath), GetPatchRootPath(parameters.RootPath, parameters.PatchName), false));
                    results.Add(await WriteMergedContentAsync(parameters.Definitions.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game, false));
                }

                if (parameters.OrphanConflicts?.Count() > 0)
                {
                    results.Add(await CopyBinariesAsync(parameters.OrphanConflicts.Where(p => p.ValueType == Parser.Common.ValueType.Binary),
                        Path.Combine(parameters.RootPath, parameters.ModPath), GetPatchRootPath(parameters.RootPath, parameters.PatchName), true));
                    results.Add(await WriteMergedContentAsync(parameters.OrphanConflicts.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                        GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game, true));
                }
                return results.All(p => p);
            }
            return false;
        }

        /// <summary>
        /// Gets the state of the patch.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IPatchState&gt;.</returns>
        public async Task<IPatchState> GetPatchStateAsync(ModPatchExporterParameters parameters)
        {
            return await GetPatchStateInternalAsync(parameters);
        }

        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SaveStateAsync(ModPatchExporterParameters parameters)
        {
            var state = await GetPatchStateInternalAsync(parameters);
            if (state == null)
            {
                state = DIResolver.Get<IPatchState>();
            }
            var path = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName));
            state.ResolvedConflicts = MapDefinitions(parameters.ResolvedConflicts, false);
            state.Conflicts = MapDefinitions(parameters.Conflicts, false);
            state.OrphanConflicts = MapDefinitions(parameters.OrphanConflicts, false);
            state.IgnoredConflicts = MapDefinitions(parameters.IgnoredConflicts, false);
            var history = state.ConflictHistory != null ? state.ConflictHistory.ToList() : new List<IDefinition>();
            foreach (var item in state.ResolvedConflicts)
            {
                var existing = history.FirstOrDefault(p => p.TypeAndId.Equals(item.TypeAndId));
                if (existing == null)
                {
                    history.Remove(existing);
                }
                history.Add(item);
            }
            state.ConflictHistory = MapDefinitions(history, true);
            return StoreState(state, path);
        }

        /// <summary>
        /// copy binaries as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="modRootPath">The mod root path.</param>
        /// <param name="patchRootPath">The patch root path.</param>
        /// <param name="checkIfExists">if set to <c>true</c> [check if exists].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> CopyBinariesAsync(IEnumerable<IDefinition> definitions, string modRootPath, string patchRootPath, bool checkIfExists)
        {
            var tasks = new List<Task>();
            foreach (var def in definitions)
            {
                var outPath = Path.Combine(patchRootPath, def.File);
                if (checkIfExists && File.Exists(outPath))
                {
                    continue;
                }
                using var stream = reader.GetStream(modRootPath, def.File);
                if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                }
                using var fs = new FileStream(outPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                tasks.Add(stream.CopyToAsync(fs));
            }
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
            return true;
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
            var lastPath = lastCachedStatePath ?? string.Empty;
            if (!lastPath.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                cachedState = null;
            }
            return cachedState;
        }

        /// <summary>
        /// get patch state internal as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IPatchState.</returns>
        private async Task<IPatchState> GetPatchStateInternalAsync(ModPatchExporterParameters parameters)
        {
            var statePath = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName), StateName);
            var cached = GetPatchState(statePath);
            if (cached == null && writeLock.TaskId.HasValue)
            {
                while (writeLock.TaskId.HasValue)
                {
                    // wait until lock is released
                }
            }
            if (File.Exists(statePath) && cached == null)
            {
                var text = await File.ReadAllTextAsync(statePath);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    cached = JsonDISerializer.Deserialize<IPatchState>(text);
                    if (cached.ConflictHistory == null)
                    {
                        cached.ConflictHistory = new List<IDefinition>();
                    }
                    if (cached.Conflicts == null)
                    {
                        cached.Conflicts = new List<IDefinition>();
                    }
                    if (cached.IgnoredConflicts == null)
                    {
                        cached.IgnoredConflicts = new List<IDefinition>();
                    }
                    if (cached.OrphanConflicts == null)
                    {
                        cached.OrphanConflicts = new List<IDefinition>();
                    }
                    if (cached.ResolvedConflicts == null)
                    {
                        cached.ResolvedConflicts = new List<IDefinition>();
                    }
                    cachedState = cached;
                    lastCachedStatePath = statePath;
                    return cached;
                }
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
        /// write state as an asynchronous operation.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool StoreState(IPatchState model, string path)
        {
            var statePath = Path.Combine(path, StateName);
            WriteStateInBackground(model, path).ConfigureAwait(false);
            cachedState = model;
            lastCachedStatePath = statePath;
            return true;
        }

        /// <summary>
        /// write merged content as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="patchRootPath">The patch root path.</param>
        /// <param name="game">The game.</param>
        /// <param name="checkIfExists">if set to <c>true</c> [check if exists].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> WriteMergedContentAsync(IEnumerable<IDefinition> definitions, string patchRootPath, string game, bool checkIfExists)
        {
            var tasks = new List<Task>();
            List<bool> results = new List<bool>();
            var validDefinitions = definitions.Where(p => p.ValueType != Parser.Common.ValueType.Namespace && p.ValueType != Parser.Common.ValueType.Variable);

            foreach (var item in validDefinitions)
            {
                var infoProvider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(game));
                if (infoProvider != null)
                {
                    var fileName = infoProvider.GetFileName(item);
                    var outPath = Path.Combine(patchRootPath, fileName);
                    if (checkIfExists && File.Exists(outPath))
                    {
                        continue;
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                    }
                    // Update filename
                    item.File = fileName;
                    tasks.Add(File.WriteAllTextAsync(outPath, item.Code, infoProvider.GetEncoding(item)));
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
        /// <param name="path">The path.</param>
        private async Task WriteStateInBackground(IPatchState model, string path)
        {
            using (await writeLock.AcquireAsync())
            {
                WriteOperationState?.Invoke(true);
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
                    var state = JsonDISerializer.Serialize(model);
                    await File.WriteAllTextAsync(statePath, state);
                    WriteOperationState?.Invoke(false);
                }).ConfigureAwait(false);
            }
        }

        #endregion Methods
    }
}
