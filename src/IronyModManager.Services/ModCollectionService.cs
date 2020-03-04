// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 03-04-2020
// ***********************************************************************
// <copyright file="ModCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModCollectionService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModCollectionService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModCollectionService" />
    public class ModCollectionService : BaseService, IModCollectionService
    {
        #region Fields

        /// <summary>
        /// The database lock
        /// </summary>
        private static readonly object serviceLock = new { };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollectionService" /> class.
        /// </summary>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModCollectionService(IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>IModCollection.</returns>
        public virtual IModCollection Create()
        {
            return GetModelInstance<IModCollection>();
        }

        /// <summary>
        /// Deletes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Delete(string name)
        {
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections?.Count() > 0)
                {
                    var existing = collections.FirstOrDefault(p => p.Name.Equals(name));
                    if (existing != null)
                    {
                        collections.Remove(existing);
                        return StorageProvider.SetModCollections(collections);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IModCollection.</returns>
        public virtual IModCollection Get(string name)
        {
            var collections = StorageProvider.GetModCollections();
            if (collections?.Count() > 0)
            {
                var collection = collections.FirstOrDefault(c => c.Name.Equals(name));
                return collection;
            }
            return null;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public virtual IEnumerable<string> GetNames()
        {
            var collections = StorageProvider.GetModCollections();
            if (collections?.Count() > 0)
            {
                return collections.Select(n => n.Name);
            }
            return new List<string>();
        }

        /// <summary>
        /// Saves the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">collection</exception>
        public virtual bool Save(IModCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections?.Count() > 0)
                {
                    var existing = collections.FirstOrDefault(p => p.Name.Equals(collection.Name));
                    if (existing != null)
                    {
                        collections.Remove(existing);
                    }
                }
                collections.Add(collection);
                return StorageProvider.SetModCollections(collections);
            }
        }

        #endregion Methods
    }
}
