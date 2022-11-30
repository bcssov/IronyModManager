// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 11-30-2022
// ***********************************************************************
// <copyright file="IModMergeService.cs" company="Mario">
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

        #endregion Methods
    }
}
