// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-25-2021
//
// Last Modified By : Mario
// Last Modified On : 02-07-2022
// ***********************************************************************
// <copyright file="LimitedDictionary.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class LimitedDictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    public class LimitedDictionary<TKey, TValue>
    {
        #region Fields

        /// <summary>
        /// The dictionary
        /// </summary>
        private readonly Dictionary<TKey, TValue> dict;

        /// <summary>
        /// The keys
        /// </summary>
        private readonly Queue<TKey> keys;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedDictionary{TKey, TValue}" /> class.
        /// </summary>
        public LimitedDictionary()
        {
            keys = new Queue<TKey>();
            dict = new Dictionary<TKey, TValue>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the maximum items.
        /// </summary>
        /// <value>The maximum items.</value>
        public int? MaxItems { get; set; }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets the <see cref="TValue" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        public virtual TValue this[TKey key]
        {
            get { return dict[key]; }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Ensures the maximum items.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void Add(TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
            {
                EnsureMaxItems();
                dict.Add(key, value);
                keys.Enqueue(key);
            }
            else
            {
                dict[key] = value;
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            dict.Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.</returns>
        public virtual bool Contains(TKey key)
        {
            return dict.ContainsKey(key);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Dictionary`2.Enumerator.</returns>
        public virtual Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public virtual void Remove(TKey key)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
            }
        }

        /// <summary>
        /// Ensures the maximum items.
        /// </summary>
        private void EnsureMaxItems()
        {
            if (MaxItems.GetValueOrDefault() > 0 && MaxItems.GetValueOrDefault() < keys.Count)
            {
                while (MaxItems.GetValueOrDefault() < keys.Count)
                {
                    Remove(keys.Dequeue());
                }
            }
        }

        #endregion Methods
    }
}
