// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-23-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
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
        /// The default region
        /// </summary>
        private const string DefaultRegion = "default_region";

        /// <summary>
        /// The cache
        /// </summary>
        private readonly Dictionary<string, LimitedDictionary<string, CacheItem>> cache;

        /// <summary>
        /// The service lock
        /// </summary>
        private readonly object serviceLock = new { };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache" /> class.
        /// </summary>
        public Cache()
        {
            cache = new Dictionary<string, LimitedDictionary<string, CacheItem>>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        public T Get<T>(CacheGetParameters parameters) where T : class
        {
            if (parameters == null)
            {
                return default;
            }
            var region = GetRegion(parameters.Region);
            if (!cache.ContainsKey(region))
            {
                return default;
            }
            var regionCache = cache[region];
            var cacheKey = ConstructKey(parameters.Prefix, parameters.Key);
            if (!regionCache.ContainsKey(cacheKey))
            {
                return default;
            }
            var item = regionCache[cacheKey];
            if ((item.Expiration != null && DateTimeOffset.Now - item.Created >= item.Expiration) || item.Value is not T value)
            {
                Invalidate(new CacheInvalidateParameters()
                {
                    Prefix = parameters.Prefix,
                    Region = parameters.Region,
                    Keys = new List<string>() { parameters.Key }
                });
                return default;
            }
            return value;
        }

        /// <summary>
        /// Invalidates the specified prefix.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void Invalidate(CacheInvalidateParameters parameters)
        {
            lock (serviceLock)
            {
                if (parameters == null)
                {
                    return;
                }
                var region = GetRegion(parameters.Region);
                if (!cache.ContainsKey(region))
                {
                    return;
                }
                var regionCache = cache[region];
                foreach (var key in parameters.Keys)
                {
                    var cacheKey = ConstructKey(parameters.Prefix, key);
                    if (regionCache.ContainsKey(cacheKey))
                    {
                        regionCache.Remove(cacheKey);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        public void Set<T>(CacheAddParameters<T> parameters) where T : class
        {
            lock (serviceLock)
            {
                if (parameters == null)
                {
                    return;
                }
                var region = GetRegion(parameters.Region);
                LimitedDictionary<string, CacheItem> regionCache;
                if (!cache.ContainsKey(region))
                {
                    regionCache = new LimitedDictionary<string, CacheItem>();
                    cache.Add(region, regionCache);
                }
                else
                {
                    regionCache = cache[region];
                }
                regionCache.MaxItems = parameters.MaxItems;
                regionCache.EnsureMaxItems();
                var cacheKey = ConstructKey(parameters.Prefix, parameters.Key);
                if (regionCache.ContainsKey(cacheKey))
                {
                    regionCache[cacheKey] = new CacheItem(parameters.Value, parameters.Expiration);
                }
                else
                {
                    regionCache.Add(cacheKey, new CacheItem(parameters.Value, parameters.Expiration));
                }
            }
        }

        /// <summary>
        /// Constructs the key.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <returns>string.</returns>
        private string ConstructKey(string prefix, string key)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                return $"{prefix}.{key}";
            }
            return key;
        }

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns>string.</returns>
        private string GetRegion(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                return DefaultRegion;
            }
            return region;
        }

        #endregion Methods
    }
}
