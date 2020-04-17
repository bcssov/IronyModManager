// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-16-2020
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
        /// The state name
        /// </summary>
        private const string StateName = "state" + Constants.JsonExtension;

        /// <summary>
        /// The service lock
        /// </summary>
        private static readonly AsyncLock serviceLock = new AsyncLock();

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

        #region Methods

        /// <summary>
        /// export definition as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentNullException">Game or definitions.</exception>
        public async Task<bool> ExportDefinitionAsync(ModPatchExporterParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Game) || parameters.Definitions == null || parameters.Definitions.Count() == 0)
            {
                throw new ArgumentNullException("Game or definitions.");
            }
            var definitionInfoProvider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(parameters.Game));
            if (definitionInfoProvider != null)
            {
                var results = new List<bool>
                {
                    await CopyBinariesAsync(parameters.Definitions.Where(p => p.ValueType == Parser.Common.ValueType.Binary), Path.Combine(parameters.RootPath, parameters.ModPath), GetPatchRootPath(parameters.RootPath, parameters.PatchName)),
                    await WriteMergedContentAsync(parameters.Definitions.Where(p => p.ValueType != Parser.Common.ValueType.Binary), GetPatchRootPath(parameters.RootPath, parameters.PatchName), parameters.Game)
                };
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
            using (await serviceLock.AcquireAsync())
            {
                return await GetPatchStateInternalAsync(parameters);
            }
        }

        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SaveStateAsync(ModPatchExporterParameters parameters)
        {
            using (await serviceLock.AcquireAsync())
            {
                var state = await GetPatchStateInternalAsync(parameters);
                if (state == null)
                {
                    state = DIResolver.Get<IPatchState>();
                }
                var statePath = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName), StateName);
                state.ResolvedConflicts = parameters.ResolvedConflicts != null ? parameters.ResolvedConflicts.ToList() : new List<IDefinition>();
                state.Conflicts = parameters.Conflicts != null ? parameters.Conflicts.ToList() : new List<IDefinition>();
                state.OrphanConflicts = parameters.OrphanConflicts != null ? parameters.OrphanConflicts.ToList() : new List<IDefinition>();
                state.IgnoredConflicts = parameters.IgnoredConflicts != null ? parameters.IgnoredConflicts.ToList() : new List<IDefinition>();
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
                state.ConflictHistory = history;
                return await WriteStateAsync(state, statePath);
            }
        }

        /// <summary>
        /// copy binaries as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="modRootPath">The mod root path.</param>
        /// <param name="patchRootPath">The patch root path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> CopyBinariesAsync(IEnumerable<IDefinition> definitions, string modRootPath, string patchRootPath)
        {
            foreach (var def in definitions)
            {
                using var stream = reader.GetStream(modRootPath, def.File);
                var outPath = Path.Combine(patchRootPath, def.File);
                if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                }
                using var fs = new FileStream(outPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                await stream.CopyToAsync(fs);
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
        /// get patch state internal as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IPatchState.</returns>
        private async Task<IPatchState> GetPatchStateInternalAsync(ModPatchExporterParameters parameters)
        {
            var statePath = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName), StateName);
            if (File.Exists(statePath))
            {
                if (File.Exists(statePath))
                {
                    var text = await File.ReadAllTextAsync(statePath);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var model = JsonDISerializer.Deserialize<IPatchState>(text);
                        if (model.ConflictHistory == null)
                        {
                            model.ConflictHistory = new List<IDefinition>();
                        }
                        if (model.Conflicts == null)
                        {
                            model.Conflicts = new List<IDefinition>();
                        }
                        if (model.IgnoredConflicts == null)
                        {
                            model.IgnoredConflicts = new List<IDefinition>();
                        }
                        if (model.OrphanConflicts == null)
                        {
                            model.OrphanConflicts = new List<IDefinition>();
                        }
                        if (model.ResolvedConflicts == null)
                        {
                            model.ResolvedConflicts = new List<IDefinition>();
                        }
                        return model;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// write merged content as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="patchRootPath">The patch root path.</param>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> WriteMergedContentAsync(IEnumerable<IDefinition> definitions, string patchRootPath, string game)
        {
            List<bool> results = new List<bool>();
            var validDefinitions = definitions.Where(p => p.ValueType != Parser.Common.ValueType.Namespace && p.ValueType != Parser.Common.ValueType.Variable);

            foreach (var item in validDefinitions)
            {
                var infoProvider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(game));
                if (infoProvider != null)
                {
                    var fileName = infoProvider.GetFileName(item);
                    var outPath = Path.Combine(patchRootPath, fileName);
                    if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                    }
                    // Update filename
                    item.File = fileName;
                    await File.WriteAllTextAsync(outPath, item.Code, infoProvider.GetEncoding(item));
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            return results.All(p => p);
        }

        /// <summary>
        /// write state as an asynchronous operation.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> WriteStateAsync(IPatchState model, string path)
        {
            await File.WriteAllTextAsync(path, JsonDISerializer.Serialize(model));
            return true;
        }

        #endregion Methods
    }
}
