// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 10-24-2021
// ***********************************************************************
// <copyright file="LocalizationRegistry.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Common.Mod.Search
{
    /// <summary>
    /// Class LocalizationRegistry.
    /// </summary>
    public static class LocalizationRegistry
    {
        #region Fields

        /// <summary>
        /// The registry
        /// </summary>
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> registry = new();

        /// <summary>
        /// The translation keys
        /// </summary>
        private static readonly string[] translationKeys = new string[] { LocalizationResources.FilterCommands.Achievements, LocalizationResources.FilterCommands.False, LocalizationResources.FilterCommands.No,
        LocalizationResources.FilterCommands.Selected, LocalizationResources.FilterCommands.Source, LocalizationResources.FilterCommands.True, LocalizationResources.FilterCommands.Version, LocalizationResources.FilterCommands.Yes};

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the translation.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetTranslation(string locale, string key)
        {
            if (registry.TryGetValue(key, out var value) && value.TryGetValue(locale, out var translationValue))
            {
                return translationValue;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the translation keys.
        /// </summary>
        /// <returns>System.String[].</returns>
        public static string[] GetTranslationKeys()
        {
            return translationKeys;
        }

        /// <summary>
        /// Gets the translations.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetTranslations(string key)
        {
            if (registry.TryGetValue(key, out var value))
            {
                return value.Select(p => p.Value).ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// Registers the translation.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="translationValue">The translation value.</param>
        public static void RegisterTranslation(string locale, string translationKey, string translationValue)
        {
            registry.AddOrUpdate(translationKey, new Dictionary<string, string>() { { locale, translationValue.ToLowerInvariant() } }, (key, oldValue) =>
            {
                oldValue.Add(locale, translationValue.ToLowerInvariant());
                return oldValue;
            });
        }

        #endregion Methods
    }
}
