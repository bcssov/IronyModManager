// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="BaseConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Common.Mod.Search.Converter;

namespace IronyModManager.Parser.Mod.Search.Converter
{
    /// <summary>
    /// Class BaseConverter.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.Search.Converter.ITypeConverter{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.Search.Converter.ITypeConverter{T}" />
    public abstract class BaseConverter<T> : ITypeConverter<T> where T : class
    {
        #region Fields

        /// <summary>
        /// The localization registry
        /// </summary>
        protected readonly ILocalizationRegistry localizationRegistry;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConverter" /> class.
        /// </summary>
        /// <param name="localizationRegistry">The localization registry.</param>
        public BaseConverter(ILocalizationRegistry localizationRegistry)
        {
            this.localizationRegistry = localizationRegistry;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the translation field keys.
        /// </summary>
        /// <value>The translation field keys.</value>
        public abstract IEnumerable<string> TranslationFieldKeys { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this instance can convert the specified locale; otherwise, <c>false</c>.</returns>
        public virtual bool CanConvert(string locale, string key)
        {
            var translation = GetTranslationValue(locale, key, TranslationFieldKeys, out var _);
            return !string.IsNullOrWhiteSpace(translation);
        }

#nullable enable

        /// <summary>
        /// Converts the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public abstract T Convert(string locale, string value);

        /// <summary>
        /// Gets the translation value.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <param name="translationKeys">The translation keys.</param>
        /// <param name="localeUsed">The locale used.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetTranslationValue(string locale, string value, IEnumerable<string> translationKeys, out string localeUsed)
        {
            foreach (var item in translationKeys)
            {
                var field = localizationRegistry.GetTranslation(locale, item) ?? string.Empty;
                if (field.StartsWith(value))
                {
                    localeUsed = locale;
                    return field;
                }
            }
            foreach (var item in translationKeys)
            {
                var fields = localizationRegistry.GetTranslations(item);
                if (fields != null)
                {
                    var field = fields.FirstOrDefault(f => (f.Value ?? string.Empty).StartsWith(value));
                    if (!string.IsNullOrWhiteSpace(field.Value))
                    {
                        localeUsed = field.Key;
                        return field.Value;
                    }
                }
            }
            localeUsed = string.Empty;
            return string.Empty;
        }

        #endregion Methods

#nullable disable
    }
}
