// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
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
using CodexMicroORM.Core.Collections;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.Parser.Definitions
{
    /// <summary>
    /// Class IndexedDefinitions.
    /// Implements the <see cref="IronyModManager.Parser.Common.Definitions.IIndexedDefinitions" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Definitions.IIndexedDefinitions" />
    public class IndexedDefinitions : IIndexedDefinitions
    {
        #region Fields

        /// <summary>
        /// The hierarchical definitions
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentIndexedList<IHierarchicalDefinitions>> childHierarchicalDefinitions;

        /// <summary>
        /// The definitions
        /// </summary>
        private readonly ConcurrentIndexedList<IDefinition> definitions;

        /// <summary>
        /// The file keys
        /// </summary>
        private readonly HashSet<string> fileKeys;

        /// <summary>
        /// The main hierarchal definitions
        /// </summary>
        private readonly ConcurrentIndexedList<IHierarchicalDefinitions> mainHierarchalDefinitions;

        /// <summary>
        /// The type and identifier keys
        /// </summary>
        private readonly HashSet<string> typeAndIdKeys;

        /// <summary>
        /// The type keys
        /// </summary>
        private readonly HashSet<string> typeKeys;

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
            definitions = new ConcurrentIndexedList<IDefinition>(nameof(IDefinition.File), nameof(IDefinition.Type),
                nameof(IDefinition.TypeAndId), nameof(IDefinition.ParentDirectory));
            fileKeys = new HashSet<string>();
            typeAndIdKeys = new HashSet<string>();
            typeKeys = new HashSet<string>();
            childHierarchicalDefinitions = new ConcurrentDictionary<string, ConcurrentIndexedList<IHierarchicalDefinitions>>();
            mainHierarchalDefinitions = new ConcurrentIndexedList<IHierarchicalDefinitions>(nameof(IHierarchicalDefinitions.Name));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds to map.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public void AddToMap(IDefinition definition)
        {
            MapKeys(fileKeys, definition.File);
            MapKeys(typeKeys, definition.Type);
            MapKeys(typeAndIdKeys, ConstructKey(definition.Type, definition.Id));
            if (useHierarchalMap)
            {
                MapHierarchicalDefinition(definition);
            }
            definitions.Add(definition);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetAll()
        {
            return new HashSet<IDefinition>(definitions);
        }

        /// <summary>
        /// Gets all file keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllFileKeys()
        {
            return fileKeys.ToHashSet();
        }

        /// <summary>
        /// Gets all type and identifier keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllTypeAndIdKeys()
        {
            return typeAndIdKeys.ToHashSet();
        }

        /// <summary>
        /// Gets all type keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllTypeKeys()
        {
            return typeKeys.ToHashSet();
        }

        /// <summary>
        /// Gets the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByFile(string file)
        {
            return definitions.GetAllByNameNoLock(nameof(IDefinition.File), file);
        }

        /// <summary>
        /// Gets the by parent directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByParentDirectory(string directory)
        {
            return definitions.GetAllByNameNoLock(nameof(IDefinition.ParentDirectory), directory);
        }

        /// <summary>
        /// Gets the type of the by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByType(string type)
        {
            return definitions.GetAllByNameNoLock(nameof(IDefinition.Type), type);
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByTypeAndId(string type, string id)
        {
            return GetByTypeAndId(ConstructKey(type, id));
        }

        /// <summary>
        /// Gets the by type andi d.
        /// </summary>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByTypeAndId(string typeAndId)
        {
            return definitions.GetAllByNameNoLock(nameof(IDefinition.TypeAndId), typeAndId);
        }

        /// <summary>
        /// Gets the hierarchical definitions.
        /// </summary>
        /// <returns>IEnumerable&lt;IHierarchicalDefinitions&gt;.</returns>
        public IEnumerable<IHierarchicalDefinitions> GetHierarchicalDefinitions()
        {
            foreach (var item in mainHierarchalDefinitions)
            {
                if (childHierarchicalDefinitions.TryGetValue(item.Name, out var value))
                {
                    item.Children = value.Select(p => p).OrderBy(p => p.Name).ToHashSet();
                }
            }
            return mainHierarchalDefinitions.Select(p => p).OrderBy(p => p.Name).ToHashSet();
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="mapHierarchicalDefinitions">if set to <c>true</c> [map hierarchical definitions].</param>
        public void InitMap(IEnumerable<IDefinition> definitions, bool mapHierarchicalDefinitions = false)
        {
            useHierarchalMap = mapHierarchicalDefinitions;
            if (definitions != null)
            {
                foreach (var item in definitions)
                {
                    AddToMap(item);
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
        /// Maps the pretty print hierarchy.
        /// </summary>
        /// <param name="definition">The definition.</param>
        private void MapHierarchicalDefinition(IDefinition definition)
        {
            bool shouldAdd = false;
            var hierarchicalDefinition = mainHierarchalDefinitions.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.ParentDirectory);
            if (hierarchicalDefinition == null)
            {
                hierarchicalDefinition = DIResolver.Get<IHierarchicalDefinitions>();
                hierarchicalDefinition.Name = definition.ParentDirectory;
                childHierarchicalDefinitions.TryAdd(definition.ParentDirectory, new ConcurrentIndexedList<IHierarchicalDefinitions>(nameof(IHierarchicalDefinitions.Name)));
                shouldAdd = true;
            }
            bool exists = false;
            if (childHierarchicalDefinitions.TryGetValue(hierarchicalDefinition.Name, out var children))
            {
                var child = children.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.Id);
                exists = child != null;
            }
            if (!exists)
            {
                var child = DIResolver.Get<IHierarchicalDefinitions>();
                child.Name = definition.Id;
                child.Key = definition.TypeAndId;
                children.Add(child);
                if (shouldAdd)
                {
                    mainHierarchalDefinitions.Add(hierarchicalDefinition);
                }
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

        #endregion Methods
    }
}
