// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 08-29-2021
// ***********************************************************************
// <copyright file="IModCollectionExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Models;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IModCollectionExporter
    /// </summary>
    public interface IModCollectionExporter
    {
        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Exports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportParadoxLauncherJsonAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<ICollectionImportResult> ImportAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Imports the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ImportModDirectoryAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Imports the paradox asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<ICollectionImportResult> ImportParadoxAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Imports the paradox launcher asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<ICollectionImportResult> ImportParadoxLauncherAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Imports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<ICollectionImportResult> ImportParadoxLauncherJsonAsync(ModCollectionExporterParams parameters);

        /// <summary>
        /// Imports the paradoxos asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<ICollectionImportResult> ImportParadoxosAsync(ModCollectionExporterParams parameters);

        #endregion Methods
    }
}
