// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 03-24-2020
// ***********************************************************************
// <copyright file="IndexedDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
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
        /// The definitions
        /// </summary>
        private readonly ConcurrentIndexedList<IDefinition> definitions;

        /// <summary>
        /// The file keys
        /// </summary>
        private readonly HashSet<string> fileKeys;

        /// <summary>
        /// The hierarchical definitions
        /// </summary>
        private readonly ConcurrentIndexedList<IHierarchicalDefinitions> hierarchicalDefinitions;

        /// <summary>
        /// The type and identifier keys
        /// </summary>
        private readonly HashSet<string> typeAndIdKeys;

        /// <summary>
        /// The type keys
        /// </summary>
        private readonly HashSet<string> typeKeys;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedDefinitions" /> class.
        /// </summary>
        public IndexedDefinitions()
        {
            definitions = new ConcurrentIndexedList<IDefinition>(nameof(IDefinition.File), nameof(IDefinition.Type), nameof(IDefinition.TypeAndId));
            fileKeys = new HashSet<string>();
            typeAndIdKeys = new HashSet<string>();
            typeKeys = new HashSet<string>();
            hierarchicalDefinitions = new ConcurrentIndexedList<IHierarchicalDefinitions>(nameof(IHierarchicalDefinitions.Name));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetAll()
        {
            return definitions;
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
            return hierarchicalDefinitions.Select(p => p).OrderBy(p => p.Name).ToHashSet();
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="mapHierarhicalDefinitions">if set to <c>true</c> [map hierarhical definitions].</param>
        public void InitMap(IEnumerable<IDefinition> definitions, bool mapHierarhicalDefinitions = false)
        {
            foreach (var item in definitions)
            {
                MapKeys(fileKeys, item.File);
                MapKeys(typeKeys, item.Type);
                MapKeys(typeAndIdKeys, ConstructKey(item.Type, item.Id));
                if (mapHierarhicalDefinitions)
                {
                    MapHierarchicalDefinition(item);
                }
                this.definitions.Add(item);
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
            var hierarchicalDefinition = hierarchicalDefinitions.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.ParentDirectory);
            if (hierarchicalDefinition == null)
            {
                hierarchicalDefinition = DIResolver.Get<IHierarchicalDefinitions>();
                hierarchicalDefinition.Name = definition.ParentDirectory;
                shouldAdd = true;
            }
            bool exists = false;
            var children = hierarchicalDefinition.Children as ConcurrentIndexedList<IHierarchicalDefinitions>;
            if (children != null)
            {
                var child = children.GetFirstByNameNoLock(nameof(IHierarchicalDefinitions.Name), definition.Id);
                exists = child != null;
            }
            else
            {
                exists = hierarchicalDefinition.Children.Any(p => p.Name.Equals(definition.Id));
            }
            if (!exists)
            {
                var child = DIResolver.Get<IHierarchicalDefinitions>();
                child.Name = definition.Id;
                child.Key = definition.TypeAndId;
                hierarchicalDefinition.Children.Add(child);
                var newChild = new HashSet<IHierarchicalDefinitions>(hierarchicalDefinition.Children.Select(s => s).OrderBy(p => p.Name));
                hierarchicalDefinition.Children.Clear();
                foreach (var item in newChild)
                {
                    hierarchicalDefinition.Children.Add(item);
                }
                if (shouldAdd)
                {
                    hierarchicalDefinitions.Add(hierarchicalDefinition);
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
