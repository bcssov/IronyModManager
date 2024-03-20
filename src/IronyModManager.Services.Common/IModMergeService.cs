// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
// ***********************************************************************
// <copyright file="IModMergeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IModMergeService
    /// </summary>
    public interface IModMergeService
    {
        #region Methods

        /// <summary>
        /// Allows the mod merge asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        ValueTask<bool> AllowModMergeAsync(string collectionName);

        /// <summary>
        /// Gets a merge collection mod name template.
        /// </summary>
        /// <returns>A string.</returns>
        string GetMergeCollectionModNameTemplate();

        /// <summary>
        /// Gets a merge collection name template.
        /// </summary>
        /// <returns>A string.</returns>
        string GetMergeCollectionNameTemplate();

        /// <summary>
        /// Determines whether [has enough free space asynchronous] [the specified collection name].
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> HasEnoughFreeSpaceAsync(string collectionName);

        /// <summary>
        /// Merges the collection by files asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IMod&gt;.</returns>
        Task<IMod> MergeCollectionByFilesAsync(string collectionName);

        /// <summary>
        /// Merges the compress collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="copiedNamePrefix">The copied name prefix.</param>
        /// <returns>Task&lt;IEnumerable&lt;IMod&gt;&gt;.</returns>
        Task<IEnumerable<IMod>> MergeCompressCollectionAsync(string collectionName, string copiedNamePrefix);

        /// <summary>
        /// Saves a merge collection mod name teplate.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>A string.</returns>
        bool SaveMergeCollectionModNameTeplate(string template);

        /// <summary>
        /// Saves a merged collection name template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>A string.</returns>
        bool SaveMergedCollectionNameTemplate(string template);

        #endregion Methods
    }
}
