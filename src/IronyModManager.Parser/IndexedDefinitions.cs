// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
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

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class IndexedDefinitions.
    /// Implements the <see cref="IronyModManager.Parser.IIndexedDefinitions" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.IIndexedDefinitions" />
    public class IndexedDefinitions : IIndexedDefinitions
    {
        #region Fields

        /// <summary>
        /// The files map
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentBag<IDefinition>> filesMap;

        /// <summary>
        /// The type and ids map
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentBag<IDefinition>> typeAndIdsMap;

        /// <summary>
        /// The type map
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentBag<IDefinition>> typeMap;

        /// <summary>
        /// The definitions
        /// </summary>
        private IEnumerable<IDefinition> definitions;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedDefinitions" /> class.
        /// </summary>
        public IndexedDefinitions()
        {
            filesMap = new ConcurrentDictionary<string, ConcurrentBag<IDefinition>>();
            typeAndIdsMap = new ConcurrentDictionary<string, ConcurrentBag<IDefinition>>();
            typeMap = new ConcurrentDictionary<string, ConcurrentBag<IDefinition>>();
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
            return filesMap.Select(s => s.Key);
        }

        /// <summary>
        /// Gets all type and identifier keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllTypeAndIdKeys()
        {
            return typeAndIdsMap.Select(s => s.Key);
        }

        /// <summary>
        /// Gets all type keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllTypeKeys()
        {
            return typeMap.Select(s => s.Key);
        }

        /// <summary>
        /// Gets the by file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByFile(string file)
        {
            return GetByKey(filesMap, file);
        }

        /// <summary>
        /// Gets the type of the by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByType(string type)
        {
            return GetByKey(typeMap, type);
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> GetByTypeAndId(string type, string id)
        {
            return GetByKey(typeAndIdsMap, ConstructKey(type, id));
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        public void InitMap(IEnumerable<IDefinition> definitions)
        {
            this.definitions = definitions;
            foreach (var item in definitions)
            {
                Map(filesMap, item.File, item);
                Map(typeMap, item.Type, item);
                Map(typeAndIdsMap, ConstructKey(item.Type, item.Id), item);
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
        /// Gets the by key.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <returns>ConcurrentBag&lt;IDefinition&gt;.</returns>
        private ConcurrentBag<IDefinition> GetByKey(ConcurrentDictionary<string, ConcurrentBag<IDefinition>> map, string key)
        {
            if (map.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Maps the specified map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        /// <param name="definition">The definition.</param>
        private void Map(ConcurrentDictionary<string, ConcurrentBag<IDefinition>> map, string key, IDefinition definition)
        {
            if (map.ContainsKey(key))
            {
                map[key].Add(definition);
            }
            else
            {
                var col = new ConcurrentBag<IDefinition>() { definition };
                map.TryAdd(key, col);
            }
        }

        #endregion Methods
    }
}
