// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-02-2020
// ***********************************************************************
// <copyright file="ModPatchExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models;
using IronyModManager.Shared;
using Newtonsoft.Json;

using System.Linq;

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
        private const string StateName = "state" + Shared.Constants.JsonExtension;

        #endregion Fields

        #region Methods

        private string GetPatchRootPath(string path, string patchName)
        {
            return Path.Combine(path, patchName);
        }

        /// <summary>
        /// Exports the definition asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ExportDefinitionAsync(ModPatchExporterParameters parameters)
        {
            return true;
        }

        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> SaveStateAsync(ModPatchExporterParameters parameters)
        {
            var state = new PatchState()
            {
                Conflicts = parameters.Conflicts.ToList(),
                OrphanConflicts = parameters.OrphanConflicts.ToList()
            };
            var statePath = Path.Combine(GetPatchRootPath(parameters.RootPath, parameters.PatchName), StateName);
            await WriteStateAsync(state, statePath);
            return true;
        }

        /// <summary>
        /// write PDX model as an asynchronous operation.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> WriteStateAsync(PatchState model, string path)
        {
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            return true;
        }

        #endregion Methods
    }
}
