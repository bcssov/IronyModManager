// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2020
// ***********************************************************************
// <copyright file="Cache.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.Cache
{
    /// <summary>
    /// Class Cache.
    /// Implements the <see cref="IronyModManager.Shared.Cache.ICache{TValue}" />
    /// Implements the <see cref="IronyModManager.Shared.Cache.ICache" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Cache.ICache" />
    /// <seealso cref="IronyModManager.Shared.Cache.ICache{TValue}" />
    public class Cache : ICache
    {
        #region Fields

        /// <summary>
        /// The cache
        /// </summary>
        private readonly Dictionary<string, CacheItem> cache;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache" /> class.
        /// </summary>
        public Cache()
        {
            cache = new Dictionary<string, CacheItem>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the specified prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public T Get<T>(string prefix, string key) where T : class
        {
            var cacheKey = ConstructKey(prefix, key);
            if (!cache.ContainsKey(cacheKey))
            {
                return default;
            }
            var item = cache[cacheKey];
            var value = item.Value as T;
            if ((item.Expiration != null && DateTimeOffset.Now - item.Created >= item.Expiration) || value == null)
            {
                cache.Remove(cacheKey);
                return default;
            }
            return value;
        }

        /// <summary>
        /// Sets the specified prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set<T>(string prefix, string key, T value) where T : class
        {
            Set(prefix, key, value, null);
        }

        /// <summary>
        /// Sets the specified prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The expiration.</param>
        public void Set<T>(string prefix, string key, T value, TimeSpan? expiration) where T : class
        {
            lock (this)
            {
                var cacheKey = ConstructKey(prefix, key);
                if (cache.ContainsKey(cacheKey))
                {
                    cache[key] = new CacheItem(value, expiration);
                }
                else
                {
                    cache.Add(cacheKey, new CacheItem(value, expiration));
                }
            }
        }

        /// <summary>
        /// Constructs the key.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        private string ConstructKey(string prefix, string key)
        {
            return $"{prefix}.{key}";
        }

        #endregion Methods
    }
}
