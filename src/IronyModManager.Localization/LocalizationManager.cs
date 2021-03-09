// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 03-09-2021
// ***********************************************************************
// <copyright file="LocalizationManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Shared.Cache;
using Newtonsoft.Json.Linq;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class LocalizationManager.
    /// Implements the <see cref="IronyModManager.Localization.ILocalizationManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizationManager" />
    public class LocalizationManager : ILocalizationManager
    {
        #region Fields

        /// <summary>
        /// The cached cultures prefix
        /// </summary>
        protected const string CachedCulturesPrefix = "Culture";

        /// <summary>
        /// The cache prefix
        /// </summary>
        protected const string CachePrefix = "Localization";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationManager" /> class.
        /// </summary>
        /// <param name="resourceProviders">The resource providers.</param>
        /// <param name="cache">The cache.</param>
        public LocalizationManager(IEnumerable<ILocalizationResourceProvider> resourceProviders, ICache cache)
        {
            ResourceProviders = resourceProviders;
            Cache = cache;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>The cache.</value>
        protected ICache Cache { get; private set; }

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
            return GetResource(CurrentLocale.CultureName, key);
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetResource(string locale, string key)
        {
            var resource = TryGetCachedResource(locale, key);
            if (resource == null)
            {
                CacheLocalization(locale);
                resource = TryGetCachedResource(locale, key);
            }

            return resource;
        }

        /// <summary>
        /// Caches the localization.
        /// </summary>
        /// <param name="locale">The locale.</param>
        protected virtual void CacheLocalization(string locale)
        {
            var resource = new JObject();
            foreach (var provider in ResourceProviders)
            {
                var content = provider.ReadResource(locale);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var obj = JObject.Parse(content);
                    resource.Merge(obj, new JsonMergeSettings()
                    {
                        MergeArrayHandling = MergeArrayHandling.Replace,
                        MergeNullValueHandling = MergeNullValueHandling.Merge,
                        PropertyNameComparison = StringComparison.OrdinalIgnoreCase
                    });
                }
            }
            Cache.Set(CachePrefix, locale, resource);
        }

        /// <summary>
        /// Gets the cached resource.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetCachedResource(string locale, string key)
        {
            var resource = Cache.Get<JObject>(CachePrefix, locale);
            if (resource != null)
            {
                var token = resource.SelectToken(key);
                if (token != null)
                {
                    var content = token.Value<string>();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        return content;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns>CultureInfo.</returns>
        protected virtual CultureInfo GetCulture(string locale)
        {
            var culture = Cache.Get<CultureInfo>(CachedCulturesPrefix, locale);
            if (culture == null)
            {
                culture = new CultureInfo(locale);
                Cache.Set(CachedCulturesPrefix, locale, culture);
            }
            return culture;
        }

        /// <summary>
        /// Tries the get cached resource.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        protected virtual string TryGetCachedResource(string locale, string key)
        {
            var cached = GetCachedResource(locale, key);
            if (cached == null)
            {
                var culture = GetCulture(locale);
                if (culture.TwoLetterISOLanguageName != culture.Name)
                {
                    // fallback to iso language
                    cached = GetCachedResource(culture.TwoLetterISOLanguageName, key);
                    if (cached == null)
                    {
                        CacheLocalization(culture.TwoLetterISOLanguageName);
                        cached = GetCachedResource(culture.TwoLetterISOLanguageName, key);
                    }
                }
            }
            return cached;
        }

        #endregion Methods
    }
}
