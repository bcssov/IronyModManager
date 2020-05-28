// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 05-28-2020
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
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportAsync(string file, IModCollection modCollection);

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
