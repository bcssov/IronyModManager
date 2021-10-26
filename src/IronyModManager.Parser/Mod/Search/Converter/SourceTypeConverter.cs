// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="SourceTypeConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Mod.Search.Converter
{
    /// <summary>
    /// Class SourceTypeConverter.
    /// Implements the <see cref="IronyModManager.Parser.Mod.Search.Converter.BaseConverter{IronyModManager.Parser.Common.Mod.Search.SourceTypeResult}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Mod.Search.Converter.BaseConverter{IronyModManager.Parser.Common.Mod.Search.SourceTypeResult}" />
    public class SourceTypeConverter : BaseConverter<SourceTypeResult>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTypeConverter" /> class.
        /// </summary>
        /// <param name="localizationRegistry">The localization registry.</param>
        public SourceTypeConverter(ILocalizationRegistry localizationRegistry) : base(localizationRegistry)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the translation field keys.
        /// </summary>
        /// <value>The translation field keys.</value>
        public override IEnumerable<string> TranslationFieldKeys => new List<string>() { LocalizationResources.FilterCommands.Source };

        /// <summary>
        /// Gets the value keys.
        /// </summary>
        /// <value>The value keys.</value>
        private IEnumerable<string> ValueKeys => new List<string>() { LocalizationResources.FilterCommands.Paradox, LocalizationResources.FilterCommands.Local, LocalizationResources.FilterCommands.Steam };

        #endregion Properties

        #region Methods

        /// <summary>
        /// Converts the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public override SourceTypeResult Convert(string locale, string value)
        {
            var translation = GetTranslationValue(locale, value, ValueKeys, out var localeUsed);
            if (!string.IsNullOrWhiteSpace(translation) && !string.IsNullOrWhiteSpace(localeUsed))
            {
                if (GetSteam(localeUsed).StartsWith(translation))
                {
                    return new SourceTypeResult(SourceType.Steam);
                }
                else if (GetParadox(localeUsed).StartsWith(translation))
                {
                    return new SourceTypeResult(SourceType.Paradox);
                }
                else if (GetLocal(localeUsed).StartsWith(translation))
                {
                    return new SourceTypeResult(SourceType.Local);
                }
            }
            return new SourceTypeResult(SourceType.None);
        }

        /// <summary>
        /// Gets the local.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns>System.String.</returns>
        protected string GetLocal(string locale)
        {
            return localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.Local);
        }

        /// <summary>
        /// Gets the paradox.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns>System.String.</returns>
        protected string GetParadox(string locale)
        {
            return localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.Paradox);
        }

        /// <summary>
        /// Gets the steam.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns>System.String.</returns>
        protected string GetSteam(string locale)
        {
            return localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.Steam);
        }

        #endregion Methods
    }
}
