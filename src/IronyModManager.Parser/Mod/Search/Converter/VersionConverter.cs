// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="VersionConverter.cs" company="Mario">
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
#nullable enable

    /// <summary>
    /// Class VersionConverter.
    /// Implements the <see cref="IronyModManager.Parser.Mod.Search.Converter.BaseConverter{IronyModManager.Shared.Version}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Mod.Search.Converter.BaseConverter{IronyModManager.Shared.Version}" />
    public class VersionConverter : BaseConverter<Shared.Version?>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionConverter" /> class.
        /// </summary>
        /// <param name="localizationRegistry">The localization registry.</param>
        public VersionConverter(ILocalizationRegistry localizationRegistry) : base(localizationRegistry)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the translation field keys.
        /// </summary>
        /// <value>The translation field keys.</value>
        public override IDictionary<string, string> TranslationFieldKeys => new Dictionary<string, string>() { { LocalizationResources.FilterCommands.Version, Fields.Version } };

        #endregion Properties

        #region Methods

        /// <summary>
        /// Converts the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public override Shared.Version? Convert(string locale, string value)
        {
            value ??= string.Empty;
            return value.ToVersion();
        }

        #endregion Methods

#nullable disable
    }
}
