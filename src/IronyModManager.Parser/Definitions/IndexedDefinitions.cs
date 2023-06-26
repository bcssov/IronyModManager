
// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 06-26-2023
// ***********************************************************************
// <copyright file="IndexedDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodexMicroORM.Core.Collections;
using IronyModManager.DI;
using IronyModManager.Shared.KeyValueStore;
using IronyModManager.Shared.Models;
using IronyModManager.Shared.Trie;
using LiteDB;
using Nito.AsyncEx;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Definitions
{

    /// <summary>
    /// Class IndexedDefinitions.
    /// Implements the <see cref="IronyModManager.Shared.Models.IIndexedDefinitions" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IIndexedDefinitions" />
    public class IndexedDefinitions : IIndexedDefinitions
    {
        #region Fields

        /// <summary>
        /// The maximum allowed insert trie operations
        /// </summary>
        private const int MaxAllowedInsertTrieOperations = 20;

        /// <summary>
        /// The search database
        /// </summary>
        private const string SearchDB = "irony.db";

        /// <summary>
        /// The search table name
        /// </summary>
        private const string SearchTableName = "search";

        /// <summary>
        /// The op lock
        /// </summary>
        private readonly AsyncLock opLock = new();

        /// <summary>
        /// The trie lock
        /// </summary>
        private readonly object trieLock = new { };

        /// <summary>
        /// All file keys
        /// </summary>
        private HashSet<string> allFileKeys;

        /// <summary>
        /// The allowed type
        /// </summary>
        private AddToMapAllowedType allowedType = AddToMapAllowedType.All;

        /// <summary>
        /// The hierarchical definitions
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentIndexedList<IHierarchicalDefinitions>> childHierarchicalDefinitions;

        /// <summary>
        /// The definitions
        /// </summary>
        private ConcurrentIndexedList<IDefinition> definitions;

        /// <summary>
        /// The directory keys
        /// </summary>
        private Dictionary<string, HashSet<string>> directoryCIKeys;

        /// <summary>
        /// The disk file ci keys
        /// </summary>
        private Dictionary<string, HashSet<string>> diskFileCIKeys;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The file ci keys
        /// </summary>
        private Dictionary<string, HashSet<string>> fileCIKeys;

        /// <summary>
        /// The game definitions count
        /// </summary>
        private long gameDefinitionsCount;

        /// <summary>
        /// The main hierarchal definitions
        /// </summary>
        private ConcurrentIndexedList<IHierarchicalDefinitions> mainHierarchalDefinitions;

        /// <summary>
        /// The reset definitions count
        /// </summary>
        private HashSet<string> resetDefinitions;

        /// <summary>
        /// The search database
        /// </summary>
        private LiteDatabase searchDb = null;

        /// <summary>
        /// The search database path
        /// </summary>
        private string searchDbPath = string.Empty;
        /// <summary>
        /// The store
        /// </summary>
        private Store<List<IDefinition>> store;

        /// <summary>
        /// The trie
        /// </summary>
        private Trie<string> trie;

        /// <summary>
        /// The type and identifier keys
        /// </summary>
        private HashSet<string> typeAndIdKeys;

        /// <summary>
        /// The type keys
        /// </summary>
        private Dictionary<string, HashSet<string>> typeKeys;

        /// <summary>
        /// The type values
        /// </summary>
        private Dictionary<ValueType, HashSet<string>> typeKeyValues;
        /// <summary>
        /// The use hierarchal map
        /// </summary>
        private bool useHierarchalMap = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedDefinitions" /> class.
        /// </summary>
        public IndexedDefinitions()
        {
            definitions = new ConcurrentIndexedList<IDefinition>(nameof(IDefinition.FileCI), nameof(IDefinition.Type),
                nameof(IDefinition.TypeAndId), nameof(IDefinition.ParentDirectoryCI), nameof(IDefinition.ValueType), nameof(IDefinition.DiskFileCI));
            fileCIKeys = new Dictionary<string, HashSet<string>>();
            diskFileCIKeys = new Dictionary<string, HashSet<string>>();
            typeAndIdKeys = new HashSet<string>();
            typeKeys = new Dictionary<string, HashSet<string>>();
            allFileKeys = new HashSet<string>();
            directoryCIKeys = new Dictionary<string, HashSet<string>>();
            resetDefinitions = new HashSet<string>();
            typeKeyValues = new Dictionary<ValueType, HashSet<string>>();
            childHierarchicalDefinitions = new ConcurrentDictionary<string, ConcurrentIndexedList<IHierarchicalDefinitions>>();
            mainHierarchalDefinitions = new ConcurrentIndexedList<IHierarchicalDefinitions>(nameof(IHierarchicalDefinitions.Name));
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [processed search item].
        /// </summary>
        public event EventHandler<ProcessedArgs> ProcessedSearchItem;

        #endregion Events

        #region Methods

        /// <summary>
        /// Adds to map.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="forceIgnoreHierarchical">if set to <c>true</c> [force ignore hierarchical].</param>
        /// <returns>Task.</returns>
        public async Task AddToMapAsync(IDefinition definition, bool forceIgnoreHierarchical = false)
        {
            async Task addDefinition()
            {
                if (store != null)
                {
                    await UpdateStoreDefinitionAsync(definition);
                }
                else
                {
                    definitions.Add(definition);
                }
            }

            using var mutex = await opLock.LockAsync();
            MapKeys(fileCIKeys, definition.FileCI, definition.TypeAndId);
            MapKeys(typeKeys, definition.Type, definition.TypeAndId);
            MapKeys(typeAndIdKeys, ConstructKey(definition.Type, definition.Id));
            MapKeys(allFileKeys, definition.FileCI);
            MapKeys(directoryCIKeys, definition.ParentDirectoryCI, definition.TypeAndId);
            MapKeys(typeKeyValues, definition.ValueType, definition.TypeAndId);
            if (!string.IsNullOrWhiteSpace(definition.DiskFileCI))
            {
                MapKeys(diskFileCIKeys, definition.DiskFileCI, definition.TypeAndId);
                MapKeys(allFileKeys, definition.DiskFileCI);
            }
            if (definition.OverwrittenFileNames?.Count > 0)
            {
                foreach (var item in definition.OverwrittenFileNames)
                {
                    MapKeys(allFileKeys, item.ToLowerInvariant());
                }
            }
            if (useHierarchalMap && !forceIgnoreHierarchical)
            {
                MapHierarchicalDefinition(definition);
            }
            if (definition.IsFromGame)
            {
                gameDefinitionsCount++;
            }

            switch (allowedType)
            {
                case AddToMapAllowedType.InvalidAndSpecial:
                    if (definition.ValueType == ValueType.Invalid || definition.IsSpecialFolder)
                    {
                        await addDefinition();
                    }
                    break;
                default:
                    await addDefinition();
                    break;
            }

            mutex.Dispose();
        }

        /// <summary>
        /// Changes the state of the hierarchical reset.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ChangeHierarchicalResetStateAsync(IDefinition definition)
        {
            using var mutex = await opLock.LockAsync();
            if (definition != null)
            {
                var parentDirectoryCI = ResolveHierarchalParentDirectory(definition);
                var hierarchicalDefinition = mainHierarchalDefinitions.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), parentDirectoryCI);
                if (hierarchicalDefinition != null)
                {
                    if (childHierarchicalDefinitions.TryGetValue(hierarchicalDefinition.Name, out var children))
                    {
                        var child = children.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.Id);
                        if (child != null)
                        {
                            child.ResetType = definition.ResetType;
                            AddOrRemoveFromResetDefinitions(definition, false);
                            hierarchicalDefinition.ResetType = children.Any(p => p.ResetType != ResetType.None) ? ResetType.Any : ResetType.None;
                            mutex.Dispose();
                            return true;
                        }
                    }
                }
            }
            mutex.Dispose();
            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            GC.SuppressFinalize(this);
            disposed = true;
            definitions.Clear();
            definitions = null;
            directoryCIKeys.Clear();
            directoryCIKeys = null;
            fileCIKeys.Clear();
            fileCIKeys = null;
            diskFileCIKeys.Clear();
            diskFileCIKeys = null;
            typeAndIdKeys.Clear();
            typeAndIdKeys = null;
            typeKeys.Clear();
            typeKeys = null;
            allFileKeys.Clear();
            allFileKeys = null;
            childHierarchicalDefinitions.Clear();
            childHierarchicalDefinitions = null;
            mainHierarchalDefinitions.Clear();
            mainHierarchalDefinitions = null;
            trie = null;
            resetDefinitions.Clear();
            resetDefinitions = null;
            typeKeyValues.Clear();
            typeKeyValues = null;
            store?.Dispose();
            store = null;
            DisposeSearchDB();
        }

        /// <summary>
        /// Exists by file asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExistsByFileAsync(string file)
        {
            return Task.FromResult(allFileKeys.Contains(file.ToLowerInvariant()));
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetAllAsync()
        {
            EnsureAllowedAllIsRespected();
            if (store != null)
            {
                return ReadDefinitionsFromStoreAsync(typeAndIdKeys);
            }
            return Task.FromResult<IEnumerable<IDefinition>>(new HashSet<IDefinition>(definitions));
        }

        /// <summary>
        /// Gets all directory keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public Task<IEnumerable<string>> GetAllDirectoryKeysAsync()
        {
            return Task.FromResult<IEnumerable<string>>(directoryCIKeys.Keys.ToHashSet());
        }

        /// <summary>
        /// Gets all file keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public Task<IEnumerable<string>> GetAllFileKeysAsync()
        {
            return Task.FromResult<IEnumerable<string>>(fileCIKeys.Keys.ToHashSet());
        }

        /// <summary>
        /// Gets all type and identifier keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public Task<IEnumerable<string>> GetAllTypeAndIdKeysAsync()
        {
            return Task.FromResult<IEnumerable<string>>(typeAndIdKeys.ToHashSet());
        }

        /// <summary>
        /// Gets all type keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public Task<IEnumerable<string>> GetAllTypeKeysAsync()
        {
            return Task.FromResult<IEnumerable<string>>(typeKeys.Keys.ToHashSet());
        }

        /// <summary>
        /// Gets the by disk file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByDiskFileAsync(string file)
        {
            EnsureAllowedAllIsRespected();
            if (store != null)
            {
                if (diskFileCIKeys.TryGetValue(file, out var value))
                {
                    return ReadDefinitionsFromStoreAsync(value);
                }
                return Task.FromResult<IEnumerable<IDefinition>>(Array.Empty<IDefinition>());
            }
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.DiskFileCI), file.ToLowerInvariant()));
        }

        /// <summary>
        /// Gets the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByFileAsync(string file)
        {
            EnsureAllowedAllIsRespected();
            if (store != null)
            {
                if (fileCIKeys.TryGetValue(file, out var value))
                {
                    return ReadDefinitionsFromStoreAsync(value);
                }
                return Task.FromResult<IEnumerable<IDefinition>>(Array.Empty<IDefinition>());
            }
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.FileCI), file.ToLowerInvariant()));
        }

        /// <summary>
        /// Gets the by parent directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByParentDirectoryAsync(string directory)
        {
            EnsureAllowedAllIsRespected(true);
            if (store != null)
            {
                if (directoryCIKeys.TryGetValue(directory, out var value))
                {
                    return ReadDefinitionsFromStoreAsync(value);
                }
                return Task.FromResult<IEnumerable<IDefinition>>(Array.Empty<IDefinition>());
            }
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.ParentDirectoryCI), directory.ToLowerInvariant()));
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByTypeAndIdAsync(string type, string id)
        {
            return GetByTypeAndIdAsync(ConstructKey(type, id));
        }

        /// <summary>
        /// Gets the by type andi d.
        /// </summary>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public async Task<IEnumerable<IDefinition>> GetByTypeAndIdAsync(string typeAndId)
        {
            EnsureAllowedAllIsRespected(true);
            if (store != null)
            {
                return await store.ReadAsync(typeAndId);
            }
            return definitions.GetAllByNameNoLock(nameof(IDefinition.TypeAndId), typeAndId);
        }

        /// <summary>
        /// Gets the type of the by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByTypeAsync(string type)
        {
            EnsureAllowedAllIsRespected();
            if (store != null)
            {
                if (typeKeys.TryGetValue(type, out var value))
                {
                    return ReadDefinitionsFromStoreAsync(value);
                }
                return Task.FromResult<IEnumerable<IDefinition>>(Array.Empty<IDefinition>());
            }
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.Type), type));
        }

        /// <summary>
        /// Gets the type of the by value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        /// <exception cref="System.ArgumentException">Only invalid types can be queried.</exception>
        public Task<IEnumerable<IDefinition>> GetByValueTypeAsync(ValueType type)
        {
            if (allowedType == AddToMapAllowedType.InvalidAndSpecial && type != ValueType.Invalid)
            {
                throw new ArgumentException("Only invalid types can be queried.");
            }
            if (store != null)
            {
                if (typeKeyValues.TryGetValue(type, out var value))
                {
                    return ReadDefinitionsFromStoreAsync(value);
                }
                return Task.FromResult<IEnumerable<IDefinition>>(Array.Empty<IDefinition>());
            }
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.ValueType), type));
        }

        /// <summary>
        /// Gets the hierarchical definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;IHierarchicalDefinitions&gt;.</returns>
        public IEnumerable<IHierarchicalDefinitions> GetHierarchicalDefinitions()
        {
            var hierarchicalDefinitions = CopyHierarchicalDefinition(mainHierarchalDefinitions);
            foreach (var item in hierarchicalDefinitions)
            {
                if (childHierarchicalDefinitions.TryGetValue(item.Name, out var value))
                {
                    item.Children = CopyHierarchicalDefinition(value.Select(p => p).OrderBy(p => p.Name).ToHashSet()).ToHashSet();
                }
            }
            return hierarchicalDefinitions.Select(p => p).OrderBy(p => p.Name).ToHashSet();
        }

        /// <summary>
        /// Determines whether [has game definitions].
        /// </summary>
        /// <returns><c>true</c> if [has game definitions]; otherwise, <c>false</c>.</returns>
        public Task<bool> HasGameDefinitionsAsync()
        {
            return Task.FromResult(gameDefinitionsCount > 0);
        }

        /// <summary>
        /// Determines whether [has reset definitions].
        /// </summary>
        /// <returns><c>true</c> if [has reset definitions]; otherwise, <c>false</c>.</returns>
        public Task<bool> HasResetDefinitionsAsync()
        {
            return Task.FromResult(resetDefinitions.Any());
        }

        /// <summary>
        /// Initializes the search asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task.</returns>
        public async Task InitializeSearchAsync(IReadOnlyCollection<IDefinition> definitions)
        {
            if (definitions != null && definitions.Any())
            {
                var total = definitions.Count;
                var counter = 0;
                if (trie != null)
                {
                    definitions.AsParallel().WithDegreeOfParallelism(MaxAllowedInsertTrieOperations).ForAll(definition =>
                    {
                        lock (trieLock)
                        {
                            counter++;
                            var displayName = $"{definition.Id} - {definition.File} - {definition.ModName}";
                            trie.Add(displayName, definition.Tags);
                            OnProcessedSearchItem(counter, total);
                        }
                    });
                }
                else if (!string.IsNullOrWhiteSpace(searchDbPath))
                {
                    searchDb ??= GetDatabase(searchDbPath);
                    await Task.Run(() =>
                    {
                        var items = new List<DefinitionSearch>();
                        foreach (var definition in definitions)
                        {
                            counter++;
                            var displayName = $"{definition.Id} - {definition.File} - {definition.ModName}";
                            var item = new DefinitionSearch() { DisplayName = displayName, Tags = definition.Tags.ToArray() };
                            items.Add(item);
                            OnProcessedSearchItem(counter, total);
                        }
                        var col = searchDb.GetCollection<DefinitionSearch>(SearchTableName);
                        col.EnsureIndex(x => x.Tags);
                        col.InsertBulk(items);
                    });
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                }
            }
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="mapHierarchicalDefinitions">if set to <c>true</c> [map hierarchical definitions].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InitMapAsync(IEnumerable<IDefinition> definitions, bool mapHierarchicalDefinitions = false)
        {
            useHierarchalMap = mapHierarchicalDefinitions;
            if (definitions != null)
            {
                foreach (var item in definitions)
                {
                    await AddToMapAsync(item);
                }
            }
        }

        /// <summary>
        /// Removes the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Task.</returns>
        public async Task RemoveAsync(IDefinition definition)
        {
            using var mutex = await opLock.LockAsync();
            if (definition.IsFromGame)
            {
                gameDefinitionsCount--;
            }
            if (gameDefinitionsCount < 0)
            {
                gameDefinitionsCount = 0;
            }
            AddOrRemoveFromResetDefinitions(definition, false);
            if (store != null)
            {
                var defs = await store.ReadAsync(definition.TypeAndId);
                if (defs != null && defs.Any(p => p.FileCI == definition.FileCI && p.DefinitionSHA == definition.DefinitionSHA))
                {
                    var existing = defs.FirstOrDefault(p => p.FileCI == definition.FileCI && p.DefinitionSHA == definition.DefinitionSHA);
                    defs.Remove(existing);
                    await store.InsertAsync(definition.TypeAndId, defs);
                }
            }
            else
            {
                definitions.Remove(definition);
            }
            var hierarchicalDefinition = mainHierarchalDefinitions.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), ResolveHierarchalParentDirectory(definition));
            if (hierarchicalDefinition != null)
            {
                if (childHierarchicalDefinitions.TryGetValue(hierarchicalDefinition.Name, out var children))
                {
                    var child = children.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.Id);
                    if (child != null)
                    {
                        children.Remove(child);
                    }
                    bool removed = false;
                    if (!children.Select(p => p).Any())
                    {
                        removed = true;
                        childHierarchicalDefinitions.TryRemove(hierarchicalDefinition.Name, out _);
                        mainHierarchalDefinitions.Remove(hierarchicalDefinition);
                    }
                    if (!removed)
                    {
                        hierarchicalDefinition.ResetType = children.Any() && children.Any(p => p.ResetType != ResetType.None) ? ResetType.Any : ResetType.None;
                    }
                }
            }
            mutex.Dispose();
        }

        /// <summary>
        /// Searches the definitions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public async Task<IEnumerable<string>> SearchDefinitionsAsync(string searchTerm, CancellationToken? token = null)
        {
            if (token != null && token.GetValueOrDefault().IsCancellationRequested)
            {
                return null;
            }
            if (trie != null)
            {
                var tags = trie.Get(searchTerm.ToLowerInvariant());
                if (tags != null)
                {
                    return tags.Distinct();
                }
            }
            else if (!string.IsNullOrEmpty(searchDbPath))
            {
                searchDb ??= GetDatabase(searchDbPath);
                var result = await Task.Run(() =>
                {
                    var col = searchDb.GetCollection<DefinitionSearch>(SearchTableName);
                    var result = col.Query().Where(x => x.Tags.Any(f => f.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))).Select(p => p.DisplayName).ToList();
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                    GC.WaitForPendingFinalizers();
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
                    return Task.FromResult(result.Distinct());
                }, token ?? CancellationToken.None);
                return result;
            }
            return null;
        }

        /// <summary>
        /// Sets the type of the allowed.
        /// </summary>
        /// <param name="allowedType">Type of the allowed.</param>
        /// <exception cref="System.InvalidOperationException">Cannot set allowed type index definition is already initialized.</exception>
        public void SetAllowedType(AddToMapAllowedType allowedType)
        {
            if (typeKeys.Any())
            {
                throw new InvalidOperationException("Cannot set allowed type index definition is already initialized.");
            }
            this.allowedType = allowedType;
        }

        /// <summary>
        /// Updates the definitions asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> UpdateDefinitionsAsync(IReadOnlyCollection<IDefinition> definitions)
        {
            // No implementation for in memory variants
            if (definitions == null || !definitions.Any())
            {
                return false;
            }
            if (store != null)
            {
                using var mutex = await opLock.LockAsync();
                var group = definitions.GroupBy(p => p.TypeAndId);
                foreach (var item in group)
                {
                    await store.InsertAsync(item.Key, item.ToList());
                }
                mutex.Dispose();
                return true;
            }
            return true;
        }

        /// <summary>
        /// Uses the disk store.
        /// </summary>
        /// <param name="storePath">The store path.</param>
        /// <exception cref="System.InvalidOperationException">Unable to switch to disk store as there are items in the memory.</exception>
        public void UseDiskStore(string storePath)
        {
            if (definitions.Any())
            {
                throw new InvalidOperationException("Unable to switch to disk store as there are items in the memory.");
            }
            store = new Store<List<IDefinition>>(ResolveStoragePath(storePath), (type) =>
            {
                if (type.Equals(nameof(IDefinition)))
                {
                    return DIResolver.GetImplementationType(typeof(IDefinition));
                }
                return null;
            });
        }

        /// <summary>
        /// Uses the search.
        /// </summary>
        /// <param name="dbPath">The database path which is specified indicates that db provider is used.</param>
        /// <param name="dbPathSuffix">The database path suffix. Not used if dbPath is not provided</param>
        public void UseSearch(string dbPath = Shared.Constants.EmptyParam, string dbPathSuffix = Shared.Constants.EmptyParam)
        {
            if (!string.IsNullOrWhiteSpace(dbPath))
            {
                searchDbPath = !string.IsNullOrWhiteSpace(dbPathSuffix) ? Path.Combine(ResolveStoragePath(dbPath), dbPathSuffix, SearchDB) : Path.Combine(ResolveStoragePath(dbPath), SearchDB);
                DisposeSearchDB();
                if (!Directory.Exists(Path.GetDirectoryName(searchDbPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(searchDbPath));
                }
            }
            else
            {
                trie = new Trie<string>();
            }
        }

        /// <summary>
        /// Resolves the storage path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        protected virtual string ResolveStoragePath(string path)
        {
            return Path.Combine(path, Parser.Common.Constants.StoreCacheRootRolder);
        }

        /// <summary>
        /// Adds the or remove from reset definitions.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="add">if set to <c>true</c> [add].</param>
        private void AddOrRemoveFromResetDefinitions(IDefinition definition, bool add)
        {
            if (definition.ResetType != ResetType.None)
            {
                if (add)
                {
                    resetDefinitions.Add(definition.TypeAndId);
                }
                else
                {
                    resetDefinitions.Remove(definition.TypeAndId);
                }
            }
        }

        /// <summary>
        /// Constructs the key.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        private string ConstructKey(params string[] keys)
        {
            return string.Join("-", keys);
        }

        /// <summary>
        /// Copies the hierarchical definition.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>IEnumerable&lt;IHierarchicalDefinitions&gt;.</returns>
        private IEnumerable<IHierarchicalDefinitions> CopyHierarchicalDefinition(IEnumerable<IHierarchicalDefinitions> source)
        {
            var result = new HashSet<IHierarchicalDefinitions>();
            foreach (var item in source)
            {
                var copy = DIResolver.Get<IHierarchicalDefinitions>();
                copy.Name = item.Name;
                copy.Mods = item.Mods;
                copy.AdditionalData = item.AdditionalData;
                item.FileNames.ToList().ForEach(copy.FileNames.Add);
                copy.ResetType = item.ResetType;
                copy.Key = item.Key;
                copy.NonGameDefinitions = item.NonGameDefinitions;
                result.Add(copy);
            }
            return result;
        }

        /// <summary>
        /// Disposes the search database.
        /// </summary>
        private void DisposeSearchDB()
        {
            try
            {
                searchDb?.Dispose();
                if (!string.IsNullOrWhiteSpace(searchDbPath))
                {
                    var dir = Path.GetDirectoryName(searchDbPath);
                    if (Directory.Exists(dir))
                    {
                        var dirInfo = new DirectoryInfo(dir) { Attributes = FileAttributes.Normal };
                        foreach (var item in dirInfo.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly))
                        {
                            item.Attributes = FileAttributes.Normal;
                        }
                        dirInfo.Delete(true);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Ensures the allowed all is respected.
        /// </summary>
        /// <param name="allowInvalid">if set to <c>true</c> [allow invalid].</param>
        /// <exception cref="System.ArgumentException">Collection is empty.</exception>
        private void EnsureAllowedAllIsRespected(bool allowInvalid = false)
        {
            if (allowedType != AddToMapAllowedType.All)
            {
                if (!allowInvalid)
                {
                    throw new ArgumentException("Collection is empty.");
                }
            }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>LiteDatabase.</returns>
        private LiteDatabase GetDatabase(string path)
        {
            return new LiteDatabase(path, new DefinitionSearchBsonMapper());
        }

        /// <summary>
        /// Maps the pretty print hierarchy.
        /// </summary>
        /// <param name="definition">The definition.</param>
        private void MapHierarchicalDefinition(IDefinition definition)
        {
            bool shouldAdd = false;
            var parentDirectoryCI = ResolveHierarchalParentDirectory(definition);
            var hierarchicalDefinition = mainHierarchalDefinitions.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), parentDirectoryCI);
            if (hierarchicalDefinition == null)
            {
                hierarchicalDefinition = DIResolver.Get<IHierarchicalDefinitions>();
                hierarchicalDefinition.Name = parentDirectoryCI;
                childHierarchicalDefinitions.TryAdd(parentDirectoryCI, new ConcurrentIndexedList<IHierarchicalDefinitions>(nameof(IHierarchicalDefinitions.Name)));
                shouldAdd = true;
            }
            bool exists = false;
            IHierarchicalDefinitions child = null;
            if (childHierarchicalDefinitions.TryGetValue(hierarchicalDefinition.Name, out var children))
            {
                child = children.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.Id);
                exists = child != null;
            }
            if (!exists)
            {
                child = DIResolver.Get<IHierarchicalDefinitions>();
                child.Name = definition.Id;
                child.Key = definition.TypeAndId;
                child.FileNames.Add(definition.FileCI);
                children.Add(child);
                if (shouldAdd)
                {
                    mainHierarchalDefinitions.Add(hierarchicalDefinition);
                }
            }
            else
            {
                if (!child.FileNames.Contains(definition.FileCI))
                {
                    child.FileNames.Add(definition.FileCI);
                }
            }
            if (definition.ResetType != ResetType.None)
            {
                child.ResetType = definition.ResetType;
                hierarchicalDefinition.ResetType = ResetType.Any;
                AddOrRemoveFromResetDefinitions(definition, true);
            }
            child.Mods ??= new List<string>();
            if (!child.Mods.Contains(definition.ModName) && !definition.IsFromGame)
            {
                child.Mods.Add(definition.ModName);
            }
            if (!definition.IsFromGame)
            {
                child.NonGameDefinitions++;
                hierarchicalDefinition.NonGameDefinitions++;
            }
            hierarchicalDefinition.Mods ??= new List<string>();
            if (!hierarchicalDefinition.Mods.Contains(definition.ModName) && !definition.IsFromGame)
            {
                hierarchicalDefinition.Mods.Add(definition.ModName);
            }
        }

        /// <summary>
        /// Maps the keys.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        private void MapKeys(HashSet<string> map, string key)
        {
            if (!map.Contains(key))
            {
                map.Add(key);
            }
        }

        /// <summary>
        /// Maps the keys.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <param name="cacheValue">The cache value.</param>
        private void MapKeys<T>(Dictionary<T, HashSet<string>> map, T key, string cacheValue)
        {
            if (key == null)
            {
                return;
            }
            else if (key is string strKey && string.IsNullOrWhiteSpace(strKey))
            {
                return;
            }
            if (map.TryGetValue(key, out var values))
            {
                values.Add(cacheValue);
            }
            else
            {
                map[key] = new HashSet<string>() { cacheValue };
            }
        }

        /// <summary>
        /// Called when [processed search item].
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="total">The total.</param>
        private void OnProcessedSearchItem(int current, int total)
        {
            ProcessedSearchItem?.Invoke(this, new ProcessedArgs() { Current = current, Total = total });
        }

        /// <summary>
        /// Read definitions from store as an asynchronous operation.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>A Task&lt;IEnumerable`1&gt; representing the asynchronous operation.</returns>
        private async Task<IEnumerable<IDefinition>> ReadDefinitionsFromStoreAsync(IReadOnlyCollection<string> keys)
        {
            var tasks = keys.Distinct().Select(store.ReadAsync);
            await Task.WhenAll(tasks);
            return tasks.Where(p => p.IsCompleted && !p.IsFaulted && p.Result != null).SelectMany(r => r.Result).ToList();
        }

        /// <summary>
        /// Resolves the hierarchal parent directory.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        private string ResolveHierarchalParentDirectory(IDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.VirtualParentDirectoryCI))
            {
                return definition.ParentDirectoryCI;
            }
            return definition.VirtualParentDirectoryCI;
        }

        /// <summary>
        /// Update store definition as an asynchronous operation.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task UpdateStoreDefinitionAsync(IDefinition definition)
        {
            var result = await store.ReadAsync(definition.TypeAndId);
            if (result != null)
            {
                result.Add(definition);
                await store.InsertAsync(definition.TypeAndId, result);
            }
            else
            {
                await store.InsertAsync(definition.TypeAndId, new List<IDefinition>() { definition });
            }
        }

        #endregion Methods
    }
}
