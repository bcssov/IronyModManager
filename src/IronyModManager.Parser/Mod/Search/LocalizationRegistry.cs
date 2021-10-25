// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-25-2021
// ***********************************************************************
// <copyright file="LocalizationRegistry.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;

namespace IronyModManager.Parser.Mod.Search
{
    /// <summary>
    /// Class LocalizationRegistry.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.Search.ILocalizationRegistry" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.Search.ILocalizationRegistry" />
    public class LocalizationRegistry : ILocalizationRegistry
    {
        #region Fields

        /// <summary>
        /// The cache region
        /// </summary>
        private const string CacheRegion = "LocalizationRegistry";

        /// <summary>
        /// The registry lock
        /// </summary>
        private static readonly object registryLock = new();

        /// <summary>
        /// The translation keys
        /// </summary>
        private static readonly string[] translationKeys = new string[] { LocalizationResources.FilterCommands.Achievements, LocalizationResources.FilterCommands.False, LocalizationResources.FilterCommands.No,
        LocalizationResources.FilterCommands.Selected, LocalizationResources.FilterCommands.Source, LocalizationResources.FilterCommands.True, LocalizationResources.FilterCommands.Version, LocalizationResources.FilterCommands.Yes};

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationRegistry" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public LocalizationRegistry(ICache cache)
        {
            this.cache = cache;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the translation.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetTranslation(string locale, string key)
        {
            var cached = cache.Get<Dictionary<string, string>>(new CacheGetParameters()
            {
                Key = key,
                Region = CacheRegion
            });
            if (cached != null && cached.TryGetValue(locale, out var translationValue))
            {
                return translationValue;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the translation keys.
        /// </summary>
        /// <returns>System.String[].</returns>
        public string[] GetTranslationKeys()
        {
            return translationKeys;
        }

        /// <summary>
        /// Gets the translations.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public IDictionary<string, string> GetTranslations(string key)
        {
            var cached = cache.Get<Dictionary<string, string>>(new CacheGetParameters()
            {
                Key = key,
                Region = CacheRegion
            });
            if (cached != null)
            {
                return cached.ToDictionary(k => k.Key, v => v.Value);
            }
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Registers the translation.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="translationValue">The translation value.</param>
        public void RegisterTranslation(string locale, string translationKey, string translationValue)
        {
            lock (registryLock)
            {
                var entry = cache.Get<Dictionary<string, string>>(new CacheGetParameters()
                {
                    Key = translationKey,
                    Region = CacheRegion
                });
                if (entry == null)
                {
                    entry = new Dictionary<string, string>();
                }
                if (entry.ContainsKey(locale))
                {
                    entry[locale] = translationValue.ToLowerInvariant();
                }
                else
                {
                    entry.Add(locale, translationValue.ToLowerInvariant());
                }
                cache.Set(new CacheAddParameters<Dictionary<string, string>>()
                {
                    Key = translationKey,
                    Region = CacheRegion,
                    Value = entry
                });
            }
        }

        #endregion Methods
    }
}
