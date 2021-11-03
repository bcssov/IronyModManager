// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-25-2021
// ***********************************************************************
// <copyright file="ILocalizationRegistry.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Mod.Search
{
    /// <summary>
    /// Interface ILocalizationRegistry
    /// </summary>
    public interface ILocalizationRegistry
    {
        #region Methods

        /// <summary>
        /// Gets the translation.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string GetTranslation(string locale, string key);

        /// <summary>
        /// Gets the translation keys.
        /// </summary>
        /// <returns>System.String[].</returns>
        string[] GetTranslationKeys();

        /// <summary>
        /// Gets the translations.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IDictionary<string, string> GetTranslations(string key);

        /// <summary>
        /// Registers the translation.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="translationKey">The translation key.</param>
        /// <param name="translationValue">The translation value.</param>
        void RegisterTranslation(string locale, string translationKey, string translationValue);

        #endregion Methods
    }
}
