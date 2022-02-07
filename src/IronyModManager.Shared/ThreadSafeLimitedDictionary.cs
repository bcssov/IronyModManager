// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-07-2022
//
// Last Modified By : Mario
// Last Modified On : 02-07-2022
// ***********************************************************************
// <copyright file="ThreadSafeLimitedDictionary.cs" company="Mario">
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
    /// Class ThreadSafeLimitedDictionary.
    /// Implements the <see cref="IronyModManager.Shared.LimitedDictionary{TKey, TValue}" />
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="IronyModManager.Shared.LimitedDictionary{TKey, TValue}" />
    public class ThreadSafeLimitedDictionary<TKey, TValue> : LimitedDictionary<TKey, TValue>
    {
        #region Fields

        /// <summary>
        /// The object lock
        /// </summary>
        private readonly object objLock = new();

        #endregion Fields

        #region Indexers

        /// <summary>
        /// Gets the <see cref="TValue" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        public override TValue this[TKey key]
        {
            get
            {
                lock (objLock)
                {
                    return base[key];
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Ensures the maximum items.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override void Add(TKey key, TValue value)
        {
            lock (objLock)
            {
                base.Add(key, value);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            lock (objLock)
            {
                base.Clear();
            }
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.</returns>
        public override bool Contains(TKey key)
        {
            lock (objLock)
            {
                return base.Contains(key);
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Dictionary`2.Enumerator.</returns>
        public override Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            lock (objLock)
            {
                return base.GetEnumerator();
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public override void Remove(TKey key)
        {
            lock (objLock)
            {
                base.Remove(key);
            }
        }

        #endregion Methods
    }
}
