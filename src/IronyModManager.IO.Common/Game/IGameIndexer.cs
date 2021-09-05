// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 09-05-2021
// ***********************************************************************
// <copyright file="IGameIndexer.cs" company="Mario">
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

namespace IronyModManager.IO.Common.Game
{
    /// <summary>
    /// Interface IGameIndexer
    /// </summary>
    public interface IGameIndexer
    {
        #region Methods

        /// <summary>
        /// Checks whether the cached definitions version signatures are the same.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CachedDefinitionsSameAsync(string storagePath, IGame game, int version);

        /// <summary>
        /// Clears the definition asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ClearDefinitionAsync(string storagePath, IGame game);

        /// <summary>
        /// Checks whether the folder is cached.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> FolderCachedAsync(string storagePath, IGame game, string path);

        /// <summary>
        /// Check whether the cached game info is the same.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> GameVersionsSameAsync(string storagePath, IGame game, IEnumerable<string> versions);

        /// <summary>
        /// Gets the definitions asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetDefinitionsAsync(string storagePath, IGame game, string path);

        /// <summary>
        /// Saves the definitions asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SaveDefinitionsAsync(string storagePath, IGame game, IEnumerable<IDefinition> definitions);

        /// <summary>
        /// Writes the version asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="gameVersions">The game versions.</param>
        /// <param name="cacheVersion">The cache version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> WriteVersionAsync(string storagePath, IGame game, IEnumerable<string> gameVersions, int cacheVersion);

        #endregion Methods
    }
}
