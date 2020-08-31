// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="ICodeParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Interface ITextParser
    /// </summary>
    public interface ICodeParser
    {
        #region Methods

        /// <summary>
        /// Cleans the whitespace.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        string CleanWhitespace(string line);

        /// <summary>
        /// Formats the code.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="indentLevel">The indent level.</param>
        /// <returns>System.String.</returns>
        string FormatCode(IScriptElement element, int indentLevel = 0);

        /// <summary>
        /// Parses the script.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="file">The file.</param>
        /// <param name="performSimpleCheck">if set to <c>true</c> [perform simple check].</param>
        /// <returns>IParseResponse.</returns>
        IParseResponse ParseScript(IEnumerable<string> lines, string file, bool performSimpleCheck = false);

        /// <summary>
        /// Parses the script without validation.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IParseResponse.</returns>
        IParseResponse ParseScriptWithoutValidation(IEnumerable<string> lines);

        /// <summary>
        /// Performs the validity check.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="file">The file.</param>
        /// <param name="performSimpleCheck">if set to <c>true</c> [perform simple check].</param>
        /// <returns>IScriptError.</returns>
        IScriptError PerformValidityCheck(IEnumerable<string> lines, string file, bool performSimpleCheck = false);

        #endregion Methods
    }
}
