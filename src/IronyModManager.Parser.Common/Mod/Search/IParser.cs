// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="IParser.cs" company="Mario">
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
    /// Interface IParser
    /// </summary>
    public interface IParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified text.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="text">The text.</param>
        /// <returns>ISearchParserResult.</returns>
        ISearchParserResult Parse(string locale, string text);

        #endregion Methods
    }
}
