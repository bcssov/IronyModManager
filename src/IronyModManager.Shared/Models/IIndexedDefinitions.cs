// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 10-21-2024
// ***********************************************************************
// <copyright file="IIndexedDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Interface IIndexedDefinitions
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IIndexedDefinitions : IDisposable
    {
        #region Events

        /// <summary>
        /// Occurs when [processed search item].
        /// </summary>
        public event EventHandler<ProcessedArgs> ProcessedSearchItem;

        #endregion Events

        #region Methods

        /// <summary>
        /// Adds to map asynchronous.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="forceIgnoreHierarchical">if set to <c>true</c> [force ignore hierarchical].</param>
        /// <returns>Task.</returns>
        Task AddToMapAsync(IDefinition definition, bool forceIgnoreHierarchical = false);

        /// <summary>
        /// Changes the state of the hierarchical reset.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if changed, <c>false</c> otherwise.</returns>
        Task<bool> ChangeHierarchicalResetStateAsync(IDefinition definition);

        /// <summary>
        /// Exists the by file asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExistsByFileAsync(string file);

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetAllAsync();

        /// <summary>
        /// Gets all directory keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        Task<IEnumerable<string>> GetAllDirectoryKeysAsync();

        /// <summary>
        /// Gets all file keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        Task<IEnumerable<string>> GetAllFileKeysAsync();

        /// <summary>
        /// Gets all type and identifier keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        Task<IEnumerable<string>> GetAllTypeAndIdKeysAsync();

        /// <summary>
        /// Gets all type keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        Task<IEnumerable<string>> GetAllTypeKeysAsync();

        /// <summary>
        /// Gets the by disk file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByDiskFileAsync(string file);

        /// <summary>
        /// Gets the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByFileAsync(string file);

        /// <summary>
        /// Gets the type of the by merge.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByMergeTypeAsync(MergeType type);

        /// <summary>
        /// Gets the by parent directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByParentDirectoryAsync(string directory);

        /// <summary>
        /// Gets the by type andi d.
        /// </summary>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByTypeAndIdAsync(string typeAndId);

        /// <summary>
        /// Gets the by type and identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByTypeAndIdAsync(string type, string id);

        /// <summary>
        /// Gets the type of the by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByTypeAsync(string type);

        /// <summary>
        /// Gets the by value type asynchronous.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        Task<IEnumerable<IDefinition>> GetByValueTypeAsync(ValueType type);

        /// <summary>
        /// Gets the hierarchical definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;IHierarchicalDefinitions&gt;.</returns>
        IEnumerable<IHierarchicalDefinitions> GetHierarchicalDefinitions();

        /// <summary>
        /// Determines whether [has game definitions asynchronous].
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> HasGameDefinitionsAsync();

        /// <summary>
        /// Determines whether [has reset definitions asynchronous].
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> HasResetDefinitionsAsync();

        /// <summary>
        /// Initializes the search asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task.</returns>
        Task InitializeSearchAsync(IReadOnlyCollection<IDefinition> definitions);

        /// <summary>
        /// Initializes the map asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="mapHierarchicalDefinitions">if set to <c>true</c> [map hierarchical definitions].</param>
        /// <returns>Task.</returns>
        Task InitMapAsync(IEnumerable<IDefinition> definitions, bool mapHierarchicalDefinitions = false);

        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Task.</returns>
        Task RemoveAsync(IDefinition definition);

        /// <summary>
        /// Searches the definitions asynchronous.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        Task<IEnumerable<string>> SearchDefinitionsAsync(string searchTerm, CancellationToken? token = null);

        /// <summary>
        /// Sets the type of the allowed.
        /// </summary>
        /// <param name="allowedType">Type of the allowed.</param>
        void SetAllowedType(AddToMapAllowedType allowedType);

        /// <summary>
        /// Updates the definitions asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> UpdateDefinitionsAsync(IReadOnlyCollection<IDefinition> definitions);

        /// <summary>
        /// Uses the disk store.
        /// </summary>
        /// <param name="storePath">The store path.</param>
        void UseDiskStore(string storePath);

        /// <summary>
        /// Uses the search.
        /// </summary>
        /// <param name="dbPath">The database path which is specified indicates that db provider is used.</param>
        /// <param name="dbPathSuffix">The database path suffix. Not used if dbPath is not provided</param>
        void UseSearch(string dbPath = Constants.EmptyParam, string dbPathSuffix = Constants.EmptyParam);

        #endregion Methods
    }
}
