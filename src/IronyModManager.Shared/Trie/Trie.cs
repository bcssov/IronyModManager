
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-14-2020
//
// Last Modified By : Mario
// Last Modified On : 06-26-2023
// ***********************************************************************
// <copyright file="Trie.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using TrieNet.Ukkonen;

namespace IronyModManager.Shared.Trie
{

    /// <summary>
    /// Class Trie.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Trie<T> : CharUkkonenTrie<T>
    {
        #region Methods

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="obj">The value.</param>
        /// <param name="words">The keys.</param>
        public void Add(T obj, IEnumerable<string> words)
        {
            if (words != null && words.Any())
            {
                foreach (var key in words)
                {
                    Add(obj, key);
                }
            }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="word">The key.</param>
        public void Add(T obj, string word)
        {
            Add(word, obj);
        }

        /// <summary>
        /// Gets the specified search query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        public HashSet<T> Get(string searchQuery)
        {
            var result = RetrieveSubstrings(searchQuery);
            if (result != null)
            {
                return result.Select(p => p.Value).ToHashSet();
            }
            return null;
        }

        #endregion Methods
    }
}
