// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="ITextParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Interface ITextParser
    /// </summary>
    public interface ITextParser
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

        #endregion Methods
    }
}
