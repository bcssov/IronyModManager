// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2020
// ***********************************************************************
// <copyright file="IModExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Interface IModExporter
    /// </summary>
    public interface IModExporter
    {
        #region Methods

        /// <summary>
        /// Applies the collection asynchronous.
        /// </summary>
        /// <param name="collectionMods">The collection mods.</param>
        /// <param name="rootDirectory">The root directory.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ApplyCollectionAsync(IReadOnlyCollection<IMod> collectionMods, string rootDirectory);

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exportPath">The export path.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="modDirectory">The mod directory.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportAsync<T>(string exportPath, T mod, string modDirectory) where T : IModCollection;

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The file.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ImportAsync<T>(string file, T mod) where T : IModCollection;

        #endregion Methods
    }
}
