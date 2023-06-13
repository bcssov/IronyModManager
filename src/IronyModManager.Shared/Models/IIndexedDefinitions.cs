
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 06-13-2023
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
        bool ChangeHierarchicalResetState(IDefinition definition);

        /// <summary>
        /// Existses the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ExistsByFile(string file);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetAll();

        /// <summary>
        /// Gets all directory keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAllDirectoryKeys();

        /// <summary>
        /// Gets all file keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAllFileKeys();

        /// <summary>
        /// Gets all type and identifier keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAllTypeAndIdKeys();

        /// <summary>
        /// Gets all type keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAllTypeKeys();

        /// <summary>
        /// Gets the by disk file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByDiskFile(string file);

        /// <summary>
        /// Gets the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByFile(string file);

        /// <summary>
        /// Gets the by parent directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByParentDirectory(string directory);

        /// <summary>
        /// Gets the type of the by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByType(string type);

        /// <summary>
        /// Gets the by type andi d.
        /// </summary>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByTypeAndId(string typeAndId);

        /// <summary>
        /// Gets the by type and identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByTypeAndId(string type, string id);

        /// <summary>
        /// Gets the type of the by value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> GetByValueType(ValueType type);

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
