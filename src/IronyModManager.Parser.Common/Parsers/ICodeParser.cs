// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
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
        /// Cleans the parsed text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        string CleanParsedText(string text);

        /// <summary>
        /// Cleans the whitespace.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        string CleanWhitespace(string line);

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string GetKey(string line, char key);

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string GetKey(string line, string key);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string GetValue(string line, char key);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string GetValue(string line, string key);

        /// <summary>
        /// Parses the script.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="file">The file.</param>
        /// <returns>IParseResponse.</returns>
        IParseResponse ParseScript(IEnumerable<string> lines, string file);

        /// <summary>
        /// Prettifies the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        string PrettifyLine(string line);

        #endregion Methods
    }
}
