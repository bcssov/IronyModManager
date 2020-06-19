// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-14-2020
//
// Last Modified By : Mario
// Last Modified On : 06-14-2020
// ***********************************************************************
// <copyright file="TrieNode.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.Trie
{
    /// <summary>
    /// Class TrieNode.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TrieNode<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrieNode{T}"/> class.
        /// </summary>
        public TrieNode()
        {
            Children = new Dictionary<char, TrieNode<T>>();
            Objects = new HashSet<T>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        /// <value>The character.</value>
        public char Character { get; set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public Dictionary<char, TrieNode<T>> Children { get; }

        /// <summary>
        /// Gets the objects.
        /// </summary>
        /// <value>The objects.</value>
        public HashSet<T> Objects { get; }

        #endregion Properties
    }
}
