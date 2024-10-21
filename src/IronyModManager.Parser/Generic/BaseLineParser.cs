// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 08-31-2020
//
// Last Modified By : Mario
// Last Modified On : 10-17-2024
// ***********************************************************************
// <copyright file="BaseLineParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class BaseLineParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    public abstract class BaseLineParser : BaseParser
    {
        #region Fields

        /// <summary>
        /// The quotes regex
        /// </summary>
        protected static readonly Regex QuotesRegex = new("\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLineParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public BaseLineParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Cleans the parsed text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        protected virtual string CleanParsedText(string text)
        {
            var sb = new StringBuilder();
            foreach (var item in text)
            {
                if (!char.IsWhiteSpace(item) &&
                    !item.Equals(Common.Constants.Scripts.OpenObject) &&
                    !item.Equals(Common.Constants.Scripts.CloseObject))
                {
                    sb.Append(item);
                }
                else
                {
                    break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetKey(string line, string key)
        {
            var cleaned = CodeParser.CleanWhitespace(line);
            if (cleaned.Contains(key, StringComparison.OrdinalIgnoreCase))
            {
                var prev = cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (prev == 0 || !char.IsWhiteSpace(cleaned[prev - 1]))
                {
                    var parsed = cleaned.Split(key, StringSplitOptions.RemoveEmptyEntries);
                    if (parsed.Length > 0)
                    {
                        if (parsed.First().StartsWith("\""))
                        {
                            return QuotesRegex.Match(parsed.First().Trim()).Value.Replace("\"", string.Empty);
                        }
                        else
                        {
                            return CleanParsedText(parsed.First().Trim().Replace("\"", string.Empty));
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetValue(string line, string key)
        {
            var cleaned = CodeParser.CleanWhitespace(line);
            if (cleaned.Contains(key, StringComparison.OrdinalIgnoreCase))
            {
                var prev = cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (prev == 0 || char.IsWhiteSpace(cleaned[prev - 1]) || cleaned[prev - 1] == Common.Constants.Scripts.OpenObject || cleaned[prev - 1] == Common.Constants.Scripts.CloseObject)
                {
                    var part = cleaned[cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase)..];
                    var parsed = part.Split(key, StringSplitOptions.RemoveEmptyEntries);
                    if (parsed.Length > 0)
                    {
                        if (parsed.First().StartsWith("\""))
                        {
                            return QuotesRegex.Match(parsed.First().Trim()).Value.Replace("\"", string.Empty);
                        }
                        else
                        {
                            return CleanParsedText(parsed.First().Trim().Replace("\"", string.Empty));
                        }
                    }
                }
            }

            return string.Empty;
        }

        #endregion Methods
    }
}
