// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 06-19-2020
// ***********************************************************************
// <copyright file="IModMergeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        /// Merges the collection asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> MergeCollectionAsync(IConflictResult conflictResult, IList<string> modOrder, string collectionName);

        #endregion Methods
    }
}
