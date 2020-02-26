// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-26-2020
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
        private readonly ConcurrentDictionary<string, string> fileKeys;

        /// <summary>
        /// The type and identifier keys
        /// </summary>
        private readonly ConcurrentDictionary<string, string> typeAndIdKeys;

        /// <summary>
        /// The type keys
        /// </summary>
        private readonly ConcurrentDictionary<string, string> typeKeys;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedDefinitions" /> class.
        /// </summary>
        public IndexedDefinitions()
        {
            definitions = new ConcurrentIndexedList<IDefinition>(nameof(IDefinition.File), nameof(IDefinition.Type), nameof(IDefinition.TypeAndId));
            fileKeys = new ConcurrentDictionary<string, string>();
            typeAndIdKeys = new ConcurrentDictionary<string, string>();
            typeKeys = new ConcurrentDictionary<string, string>();
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
            return fileKeys.Select(s => s.Key); ;
        }

        /// <summary>
        /// Gets all type and identifier keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllTypeAndIdKeys()
        {
            return typeAndIdKeys.Select(s => s.Key);
        }

        /// <summary>
        /// Gets all type keys.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllTypeKeys()
        {
            return typeKeys.Select(s => s.Key); ;
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
            return definitions.GetAllByNameNoLock(nameof(IDefinition.TypeAndId), ConstructKey(type, id));
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        public void InitMap(IEnumerable<IDefinition> definitions)
        {
            foreach (var item in definitions)
            {
                MapKeys(fileKeys, item.File);
                MapKeys(typeKeys, item.Type);
                MapKeys(typeAndIdKeys, ConstructKey(item.Type, item.Id));
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
        /// Maps the keys.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="key">The key.</param>
        private void MapKeys(ConcurrentDictionary<string, string> map, string key)
        {
            if (!map.ContainsKey(key))
            {
                map.TryAdd(key, key);
            }
        }

        #endregion Methods
    }
}
