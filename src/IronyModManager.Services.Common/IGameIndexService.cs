// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="IGameIndexService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Models.Common;
using IronyModManager.Shared.Models;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IGameIndexService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IGameIndexService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Indexes the definitions asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> IndexDefinitionsAsync(IGame game, IEnumerable<string> versions, IIndexedDefinitions indexedDefinitions);

        /// <summary>
        /// Loads the definitions asynchronous.
        /// </summary>
        /// <param name="modDefinitions">The mod definitions.</param>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <param name="gameLanguages">The game languages.</param>
        /// <returns>Task&lt;IIndexedDefinitions&gt;.</returns>
        Task<IIndexedDefinitions> LoadDefinitionsAsync(IIndexedDefinitions modDefinitions, IGame game, IEnumerable<string> versions, IReadOnlyCollection<IGameLanguage> gameLanguages);

        #endregion Methods
    }
}
