﻿// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 08-25-2021
// ***********************************************************************
// <copyright file="IModCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IModCollectionService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IModCollectionService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>IModCollection.</returns>
        IModCollection Create();

        /// <summary>
        /// Deletes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Delete(string name);

        /// <summary>
        /// Existses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Exists(string name);

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <param name="exportOrderOnly">if set to <c>true</c> [export order only].</param>
        /// <param name="exportMods">if set to <c>true</c> [export mods].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportAsync(string file, IModCollection modCollection, bool exportOrderOnly = false, bool exportMods = false);

        /// <summary>
        /// Exports the hash report asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportHashReportAsync(IEnumerable<IMod> mods, string path);

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IModCollection.</returns>
        IModCollection Get(string name);

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, System.Boolean&gt;.</returns>
        IEnumerable<IModCollection> GetAll();

        /// <summary>
        /// Gets the imported collection details asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        Task<IModCollection> GetImportedCollectionDetailsAsync(string file);

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        Task<IModCollection> ImportAsync(string file);

        /// <summary>
        /// Imports the hash report asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>Task&lt;IEnumerable&lt;IHashReport&gt;&gt;.</returns>
        Task<IEnumerable<IHashReport>> ImportHashReportAsync(IEnumerable<IMod> mods, IReadOnlyCollection<IHashReport> hashReports);

        /// <summary>
        /// Imports the paradox asynchronous.
        /// </summary>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        Task<IModCollection> ImportParadoxAsync();

        /// <summary>
        /// Imports the paradox launcher asynchronous.
        /// </summary>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        Task<IModCollection> ImportParadoxLauncherAsync();

        /// <summary>
        /// Imports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        Task<IModCollection> ImportParadoxLauncherJsonAsync(string file);

        /// <summary>
        /// Imports the paradoxos asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        Task<IModCollection> ImportParadoxosAsync(string file);

        /// <summary>
        /// Saves the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(IModCollection collection);

        #endregion Methods
    }
}
