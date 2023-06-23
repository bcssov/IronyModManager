
// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2023
// ***********************************************************************
// <copyright file="IndexedDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodexMicroORM.Core.Collections;
using IronyModManager.DI;
using IronyModManager.Shared.KeyValueStore;
using IronyModManager.Shared.Models;
using IronyModManager.Shared.Trie;
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
        /// All file keys
        /// </summary>
        private HashSet<string> allFileKeys;

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
        private HashSet<string> directoryKeys;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The file keys
        /// </summary>
        private HashSet<string> fileKeys;

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
        /// The store
        /// </summary>
        private Store<List<IDefinition>> store;

        /// <summary>
        /// The trie
        /// </summary>
        private Trie<IDefinition> trie;

        /// <summary>
        /// The type and identifier keys
        /// </summary>
        private HashSet<string> typeAndIdKeys;

        /// <summary>
        /// The type keys
        /// </summary>
        private HashSet<string> typeKeys;

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
            fileKeys = new HashSet<string>();
            typeAndIdKeys = new HashSet<string>();
            typeKeys = new HashSet<string>();
            allFileKeys = new HashSet<string>();
            directoryKeys = new HashSet<string>();
            resetDefinitions = new HashSet<string>();
            childHierarchicalDefinitions = new ConcurrentDictionary<string, ConcurrentIndexedList<IHierarchicalDefinitions>>();
            mainHierarchalDefinitions = new ConcurrentIndexedList<IHierarchicalDefinitions>(nameof(IHierarchicalDefinitions.Name));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds to map.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="forceIgnoreHierarchical">if set to <c>true</c> [force ignore hierarchical].</param>
        /// <returns>Task.</returns>
        public Task AddToMapAsync(IDefinition definition, bool forceIgnoreHierarchical = false)
        {
            MapKeys(fileKeys, definition.FileCI);
            MapKeys(typeKeys, definition.Type);
            MapKeys(typeAndIdKeys, ConstructKey(definition.Type, definition.Id));
            MapKeys(allFileKeys, definition.FileCI);
            MapKeys(directoryKeys, definition.ParentDirectory);
            if (!string.IsNullOrWhiteSpace(definition.DiskFile))
            {
                MapKeys(allFileKeys, definition.DiskFile.ToLowerInvariant());
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
            definitions.Add(definition);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Changes the state of the hierarchical reset.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public Task<bool> ChangeHierarchicalResetStateAsync(IDefinition definition)
        {
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
                            return Task.FromResult(true);
                        }
                    }
                }
            }
            return Task.FromResult(false);
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
            directoryKeys.Clear();
            directoryKeys = null;
            fileKeys.Clear();
            fileKeys = null;
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
            return Task.FromResult<IEnumerable<IDefinition>>(new HashSet<IDefinition>(definitions));
        }

        /// <summary>
        /// Gets all directory keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public Task<IEnumerable<string>> GetAllDirectoryKeysAsync()
        {
            return Task.FromResult<IEnumerable<string>>(directoryKeys.ToHashSet());
        }

        /// <summary>
        /// Gets all file keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public Task<IEnumerable<string>> GetAllFileKeysAsync()
        {
            return Task.FromResult<IEnumerable<string>>(fileKeys.ToHashSet());
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
            return Task.FromResult<IEnumerable<string>>(typeKeys.ToHashSet());
        }

        /// <summary>
        /// Gets the by disk file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByDiskFileAsync(string file)
        {
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.DiskFileCI), file.ToLowerInvariant()));
        }

        /// <summary>
        /// Gets the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByFileAsync(string file)
        {
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.FileCI), file.ToLowerInvariant()));
        }

        /// <summary>
        /// Gets the by parent directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByParentDirectoryAsync(string directory)
        {
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
        public Task<IEnumerable<IDefinition>> GetByTypeAndIdAsync(string typeAndId)
        {
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.TypeAndId), typeAndId));
        }

        /// <summary>
        /// Gets the type of the by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByTypeAsync(string type)
        {
            return Task.FromResult(definitions.GetAllByNameNoLock(nameof(IDefinition.Type), type));
        }

        /// <summary>
        /// Gets the type of the by value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> GetByValueTypeAsync(ValueType type)
        {
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
        /// Initializes the search.
        /// </summary>
        /// <returns>Task.</returns>
        public Task InitSearchAsync()
        {
            trie = new Trie<IDefinition>();

            // We're not indexing definitions from the game
            foreach (var item in definitions.Where(p => (p.Tags?.Any()).GetValueOrDefault() && !p.IsFromGame))
            {
                trie.Add(item, item.Tags);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Task.</returns>
        public Task RemoveAsync(IDefinition definition)
        {
            if (definition.IsFromGame)
            {
                gameDefinitionsCount--;
            }
            if (gameDefinitionsCount < 0)
            {
                gameDefinitionsCount = 0;
            }
            AddOrRemoveFromResetDefinitions(definition, false);
            definitions.Remove(definition);
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
            return Task.CompletedTask;
        }

        /// <summary>
        /// Searches the definitions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public Task<IEnumerable<IDefinition>> SearchDefinitionsAsync(string searchTerm)
        {
            if (trie != null)
            {
                return Task.FromResult<IEnumerable<IDefinition>>(trie.Get(searchTerm.ToLowerInvariant()));
            }
            return Task.FromResult<IEnumerable<IDefinition>>(null);
        }

        /// <summary>
        /// Updates the definitions asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> UpdateDefinitionsAsync(IReadOnlyCollection<IDefinition> definitions)
        {
            // No implementation for in memory variants
            if (definitions == null || !definitions.Any())
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
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
            store = new Store<List<IDefinition>>(storePath);
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
                item.FileNames.ToList().ForEach(f => copy.FileNames.Add(f));
                copy.ResetType = item.ResetType;
                copy.Key = item.Key;
                copy.NonGameDefinitions = item.NonGameDefinitions;
                result.Add(copy);
            }
            return result;
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

        #endregion Methods
    }
}
