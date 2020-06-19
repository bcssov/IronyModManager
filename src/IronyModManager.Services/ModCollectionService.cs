// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 06-19-2020
// ***********************************************************************
// <copyright file="ModCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModCollectionService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModCollectionService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModCollectionService" />
    public class ModCollectionService : ModBaseService, IModCollectionService
    {
        #region Fields

        /// <summary>
        /// The database lock
        /// </summary>
        private static readonly object serviceLock = new { };

        /// <summary>
        /// The mod collection exporter
        /// </summary>
        private readonly IModCollectionExporter modCollectionExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollectionService" /> class.
        /// </summary>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="modCollectionExporter">The mod collection exporter.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModCollectionService(IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IReader reader, IModWriter modWriter, IModParser modParser, IGameService gameService, IModCollectionExporter modCollectionExporter,
            IStorageProvider storageProvider, IMapper mapper) : base(definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.modCollectionExporter = modCollectionExporter;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>IModCollection.</returns>
        public virtual IModCollection Create()
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = GetModelInstance<IModCollection>();
            instance.Game = game.Type;
            return instance;
        }

        /// <summary>
        /// Deletes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Delete(string name)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections.Count() > 0)
                {
                    var existing = collections.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
        /// Existses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Exists(string name)
        {
            var all = GetAll();
            return all.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExportAsync(string file, IModCollection modCollection)
        {
            var game = GameService.GetSelected();
            if (game == null || modCollection == null)
            {
                return Task.FromResult(false);
            }
            var path = GetPatchDirectory(game, modCollection);
            return modCollectionExporter.ExportAsync(new ModCollectionExporterParams()
            {
                File = file,
                Mod = modCollection,
                ModDirectory = path
            });
        }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IModCollection.</returns>
        public virtual IModCollection Get(string name)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var collections = StorageProvider.GetModCollections();
            if (collections?.Count() > 0)
            {
                var collection = collections.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Game.Equals(game.Type));
                return collection;
            }
            return null;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;IModCollection&gt;.</returns>
        public virtual IEnumerable<IModCollection> GetAll()
        {
            return GetAllModCollectionsInternal();
        }

        /// <summary>
        /// get imported collection details as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual async Task<IModCollection> GetImportedCollectionDetailsAsync(string file)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = GetModelInstance<IModCollection>();
            var result = await modCollectionExporter.ImportAsync(new ModCollectionExporterParams()
            {
                File = file,
                Mod = instance
            });
            if (result)
            {
                return instance;
            }
            return null;
        }

        /// <summary>
        /// import as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public async Task<IModCollection> ImportAsync(string file)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = await GetImportedCollectionDetailsAsync(file);
            if (instance != null)
            {
                var path = GetPatchDirectory(game, instance);
                if (await modCollectionExporter.ImportModDirectoryAsync(new ModCollectionExporterParams()
                {
                    File = file,
                    ModDirectory = path,
                    Mod = instance
                }))
                {
                    return instance;
                }
            }
            return null;
        }

        /// <summary>
        /// import paradoxos as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual async Task<IModCollection> ImportParadoxosAsync(string file)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = Create();
            if (await modCollectionExporter.ImportParadoxosAsync(new ModCollectionExporterParams()
            {
                File = file,
                Mod = instance
            }))
            {
                return instance;
            }
            return null;
        }

        /// <summary>
        /// Saves the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">collection</exception>
        public virtual bool Save(IModCollection collection)
        {
            if (collection == null || string.IsNullOrWhiteSpace(collection.Game))
            {
                throw new ArgumentNullException("collection");
            }
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections.Count() > 0)
                {
                    var existing = collections.FirstOrDefault(p => p.Name.Equals(collection.Name, StringComparison.OrdinalIgnoreCase) && p.Game.Equals(game.Type));
                    if (existing != null)
                    {
                        collections.Remove(existing);
                    }
                    if (collection.IsSelected)
                    {
                        foreach (var item in collections.Where(p => p.Game.Equals(game.Type) && p.IsSelected))
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                collections.Add(collection);
                return StorageProvider.SetModCollections(collections);
            }
        }

        #endregion Methods
    }
}
