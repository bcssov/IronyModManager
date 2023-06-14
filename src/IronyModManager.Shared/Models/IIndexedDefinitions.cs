
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 06-14-2023
// ***********************************************************************
// <copyright file="IIndexedDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
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
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        Task<bool> ChangeHierarchicalResetStateAsync(IDefinition definition);

        /// <summary>
        /// Existses the by file asynchronous.
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
        /// Determines whether [has game definitions].
        /// </summary>
        /// <returns><c>true</c> if [has game definitions]; otherwise, <c>false</c>.</returns>
        bool HasGameDefinitions();

        /// <summary>
        /// Determines whether [has reset definitions].
        /// </summary>
        /// <returns><c>true</c> if [has reset definitions]; otherwise, <c>false</c>.</returns>
        bool HasResetDefinitions();

        /// <summary>
        /// Initializes the map asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="mapHierarchicalDefinitions">if set to <c>true</c> [map hierarchical definitions].</param>
        /// <returns>Task.</returns>
        Task InitMapAsync(IEnumerable<IDefinition> definitions, bool mapHierarchicalDefinitions = false);

        /// <summary>
        /// Initializes the search.
        /// </summary>
        void InitSearch();

        /// <summary>
        /// Removes the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        void Remove(IDefinition definition);

        /// <summary>
        /// Searches the definitions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> SearchDefinitions(string searchTerm);

        #endregion Methods
    }
}
