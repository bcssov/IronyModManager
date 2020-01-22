// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="LocalizationManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IronyModManager.Localization.ResourceProviders;
using Newtonsoft.Json.Linq;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class LocalizationManager.
    /// Implements the <see cref="IronyModManager.Localization.ILocalizationManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizationManager" />
    internal class LocalizationManager : ILocalizationManager
    {
        #region Fields

        /// <summary>
        /// The cache
        /// </summary>
        private static readonly ConcurrentDictionary<string, JObject> cache = new ConcurrentDictionary<string, JObject>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationManager" /> class.
        /// </summary>
        /// <param name="resourceProviders">The resource providers.</param>
        public LocalizationManager(IEnumerable<ILocalizationResourceProvider> resourceProviders)
        {
            ResourceProviders = resourceProviders;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the resource providers.
        /// </summary>
        /// <value>The resource providers.</value>
        protected IEnumerable<ILocalizationResourceProvider> ResourceProviders { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public virtual string GetResource(string key)
        {
            var resource = TryGetCachedResource(key);
            if (resource == null)
            {
                CacheLocalization(CurrentLocale.CultureName);
                resource = TryGetCachedResource(key);
            }

            return resource;
        }

        /// <summary>
        /// Caches the localization.
        /// </summary>
        /// <param name="locale">The locale.</param>
        private void CacheLocalization(string locale)
        {
            var resource = new JObject();
            foreach (var provider in ResourceProviders)
            {
                var content = JObject.Parse(provider.ReadResource(locale));
                resource.Merge(content, new JsonMergeSettings()
                {
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    MergeNullValueHandling = MergeNullValueHandling.Merge,
                    PropertyNameComparison = StringComparison.OrdinalIgnoreCase
                });
            }
            cache.TryAdd(locale, resource);
        }

        /// <summary>
        /// Gets the cached resource.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        private string GetCachedResource(string key)
        {
            if (cache.TryGetValue(CurrentLocale.CultureName, out var value))
            {
                if (value.TryGetValue(key, out var propValue))
                {
                    var content = propValue.Value<string>();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        return content;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries the get cached resource.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        private string TryGetCachedResource(string key)
        {
            var cached = GetCachedResource(key);
            if (cached == null && CurrentLocale.CurrentCulture.TwoLetterISOLanguageName != CurrentLocale.CultureName)
            {
                // fallback to iso language
                cached = GetCachedResource(CurrentLocale.CurrentCulture.TwoLetterISOLanguageName);
            }
            return cached;
        }

        #endregion Methods
    }
}
