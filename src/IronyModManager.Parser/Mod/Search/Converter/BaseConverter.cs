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
        /// Initializes a new instance of the <see cref="BaseConverter{T}" /> class.
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
        public abstract IDictionary<string, string> TranslationFieldKeys { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this instance can convert the specified locale; otherwise, <c>false</c>.</returns>
        public virtual CanParseResult CanConvert(string locale, string key)
        {
            var translation = GetTranslationValue(locale, key, TranslationFieldKeys, out var _, out var mappedStaticField);
            return new CanParseResult(!string.IsNullOrWhiteSpace(translation), mappedStaticField);
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
        /// <param name="mappedStaticField">The mapped static field.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetTranslationValue(string locale, string value, IDictionary<string, string> translationKeys, out string localeUsed, out string mappedStaticField)
        {
            foreach (var item in translationKeys)
            {
                var field = localizationRegistry.GetTranslation(locale, item.Key) ?? string.Empty;
                if (value.StartsWith(field))
                {
                    localeUsed = locale;
                    mappedStaticField = item.Value;
                    return field;
                }
            }
            foreach (var item in translationKeys)
            {
                var fields = localizationRegistry.GetTranslations(item.Key);
                if (fields != null)
                {
                    var field = fields.FirstOrDefault(f => value.StartsWith(f.Value ?? string.Empty));
                    if (!string.IsNullOrWhiteSpace(field.Value))
                    {
                        localeUsed = field.Key;
                        mappedStaticField = item.Value;
                        return field.Value;
                    }
                }
            }
            localeUsed = string.Empty;
            mappedStaticField = string.Empty;
            return string.Empty;
        }

        #endregion Methods

#nullable disable
    }
}
