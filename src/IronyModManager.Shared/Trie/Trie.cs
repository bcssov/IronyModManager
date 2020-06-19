// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-14-2020
//
// Last Modified By : Mario
// Last Modified On : 06-14-2020
// ***********************************************************************
// <copyright file="Trie.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Trie
{
    /// <summary>
    /// Class Trie.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Trie<T>
    {
        #region Fields

        /// <summary>
        /// The node
        /// </summary>
        private readonly TrieNode<T> Node;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Trie{T}" /> class.
        /// </summary>
        public Trie()
        {
            Node = new TrieNode<T>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="words">The words.</param>
        public void Add(T obj, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                Add(obj, Node, word);
            }
        }

        /// <summary>
        /// Gets the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        public HashSet<T> Get(string text)
        {
            return Get(Node, text);
        }

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="node">The node.</param>
        /// <param name="text">The text.</param>
        private void Add(T obj, TrieNode<T> node, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (node.Children.TryGetValue(text.First(), out var child))
            {
                child.Objects.Add(obj);
                if (text.Count() > 0)
                {
                    Add(obj, child, text.Substring(1));
                }
            }
            else
            {
                child = new TrieNode<T>()
                {
                    Character = text.First(),
                };
                child.Objects.Add(obj);
                if (text.Count() > 1)
                {
                    Add(obj, child, text.Substring(1));
                }
                node.Children.Add(text.First(), child);
            }
        }

        /// <summary>
        /// Gets the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="text">The text.</param>
        /// <returns>HashSet&lt;T&gt;.</returns>
        private HashSet<T> Get(TrieNode<T> node, string text)
        {
            if (text.Count() == 0)
            {
                return node.Objects;
            }
            else if (node.Children.TryGetValue(text.First(), out var child))
            {
                return Get(child, text.Substring(1));
            }
            else
            {
                // Try other children for a partial match
                var results = new HashSet<T>();
                foreach (var childNode in node.Children)
                {
                    var result = Get(childNode.Value, text);
                    if (result?.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            results.Add(item);
                        }
                    }
                }
                if (results.Count > 0)
                {
                    return results;
                }
            }
            return null;
        }

        #endregion Methods
    }
}
