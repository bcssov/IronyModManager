// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="IModPatchExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods.Models;
using static IronyModManager.IO.Common.Delegates;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IModPatchExporter
    /// </summary>
    public interface IModPatchExporter
    {
        #region Events

        /// <summary>
        /// Occurs when [mod definition analyze].
        /// </summary>
        event WriteOperationStateDelegate WriteOperationState;

        #endregion Events

        #region Methods

        /// <summary>
        /// Exports the definition asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportDefinitionAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Gets the patch state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IPatchState&gt;.</returns>
        Task<IPatchState> GetPatchStateAsync(ModPatchExporterParameters parameters);

        /// <summary>
        /// Saves the state asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SaveStateAsync(ModPatchExporterParameters parameters);

        #endregion Methods
    }
}
