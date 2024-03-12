// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="IModPatchExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods.Models;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IModPatchExporter
    /// </summary>
    public interface IModPatchExporter
    {
        #region Methods

        /// <summary>
        /// Copies the patch mod asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CopyPatchModAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Exports the definition asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportDefinitionAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Gets an allowed languages async.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task containing IReadOnlyCollection of strings.<see cref="Task{IReadOnlyCollection{string}}"/></returns>
        Task<IReadOnlyCollection<string>> GetAllowedLanguagesAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Gets the patch files.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetPatchFiles(ModPatchExporterParameters parameters);

        /// <summary>
        /// Gets the patch state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="loadExternalCode">if set to <c>true</c> [load external code].</param>
        /// <returns>Task&lt;IPatchState&gt;.</returns>
        Task<IPatchState> GetPatchStateAsync(ModPatchExporterParameters parameters, bool loadExternalCode = true);

        /// <summary>
        /// Gets the patch state mode asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Nullable&lt;PatchStateMode&gt;&gt;.</returns>
        Task<PatchStateMode?> GetPatchStateModeAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Loads the definition contents asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> LoadDefinitionContentsAsync(ModPatchExporterParameters parameters, string path);

        /// <summary>
        /// Renames the patch mod asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> RenamePatchModAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Resets the cache.
        /// </summary>
        void ResetCache();

        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SaveStateAsync(ModPatchExporterParameters parameters);

        #endregion Methods
    }
}
