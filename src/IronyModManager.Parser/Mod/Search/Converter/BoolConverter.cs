// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="BoolConverter.cs" company="Mario">
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
    /// Class BoolConverter.
    /// Implements the <see cref="IronyModManager.Parser.Mod.Search.Converter.BaseConverter{IronyModManager.Parser.Common.Mod.Search.BoolFilterResult}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Mod.Search.Converter.BaseConverter{IronyModManager.Parser.Common.Mod.Search.BoolFilterResult}" />
    public class BoolConverter : BaseConverter<BoolFilterResult>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolConverter" /> class.
        /// </summary>
        /// <param name="localizationRegistry">The localization registry.</param>
        public BoolConverter(ILocalizationRegistry localizationRegistry) : base(localizationRegistry)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the translation field keys.
        /// </summary>
        /// <value>The translation field keys.</value>
        public override IEnumerable<string> TranslationFieldKeys => new List<string>() { LocalizationResources.FilterCommands.Achievements, LocalizationResources.FilterCommands.Selected };

        /// <summary>
        /// Gets the value keys.
        /// </summary>
        /// <value>The value keys.</value>
        private IEnumerable<string> ValueKeys => new List<string>() { LocalizationResources.FilterCommands.Yes, LocalizationResources.FilterCommands.False, LocalizationResources.FilterCommands.No, LocalizationResources.FilterCommands.True };

        #endregion Properties

        #region Methods

        /// <summary>
        /// Converts the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public override BoolFilterResult Convert(string locale, string value)
        {
            var translation = GetTranslationValue(locale, value, ValueKeys, out var localeUsed);
            if (!string.IsNullOrWhiteSpace(translation) && !string.IsNullOrWhiteSpace(localeUsed))
            {
                if (GetInclude(localeUsed).Any(x => translation.StartsWith(x)))
                {
                    return new BoolFilterResult(true);
                }
                else if (GetExclude(localeUsed).Any(x => translation.StartsWith(x)))
                {
                    return new BoolFilterResult(false);
                }
            }
            return new BoolFilterResult(null);
        }

        /// <summary>
        /// Gets the exclude.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected IEnumerable<string> GetExclude(string locale)
        {
            var no = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.No);
            var @false = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.False);
            return new List<string>() { no, @false };
        }

        /// <summary>
        /// Gets the include.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected IEnumerable<string> GetInclude(string locale)
        {
            var yes = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.Yes);
            var @true = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterCommands.True);
            return new List<string>() { yes, @true };
        }

        #endregion Methods
    }
}
