// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="TextParser.cs" company="Mario">
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

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class TextParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.ITextParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.ITextParser" />
    [ExcludeFromCoverage("Text parser is tested in parser implementations.")]
    public class TextParser : ITextParser
    {
        #region Fields

        /// <summary>
        /// The cleaner conversion map
        /// </summary>
        protected static readonly Dictionary<string, string> cleanerConversionMap = new Dictionary<string, string>()
        {
            { $" {Common.Constants.Scripts.VariableSeparatorId}", Common.Constants.Scripts.VariableSeparatorId.ToString() },
            { $"{Common.Constants.Scripts.VariableSeparatorId} ", Common.Constants.Scripts.VariableSeparatorId.ToString() },
            { $" {Common.Constants.Scripts.OpeningBracket}", Common.Constants.Scripts.OpeningBracket.ToString() },
            { $"{Common.Constants.Scripts.OpeningBracket} ", Common.Constants.Scripts.OpeningBracket.ToString() },
            { $" {Common.Constants.Scripts.ClosingBracket}", Common.Constants.Scripts.ClosingBracket.ToString() },
            { $"{Common.Constants.Scripts.ClosingBracket} ", Common.Constants.Scripts.ClosingBracket.ToString() },
        };

        /// <summary>
        /// The quotes regex
        /// </summary>
        protected static readonly Regex quotesRegex = new Regex("\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The reverse cleaner conversion map
        /// </summary>
        protected static readonly Dictionary<string, string> reverseCleanerConversionMap = new Dictionary<string, string>()
        {
            { Common.Constants.Scripts.VariableSeparatorId.ToString(), $" {Common.Constants.Scripts.VariableSeparatorId} " },
            { Common.Constants.Scripts.OpeningBracket.ToString(), $" {Common.Constants.Scripts.OpeningBracket} " },
            { Common.Constants.Scripts.ClosingBracket.ToString(), $" {Common.Constants.Scripts.ClosingBracket} " },
        };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Cleans the parsed text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public string CleanParsedText(string text)
        {
            var sb = new StringBuilder();
            foreach (var item in text)
            {
                if (!char.IsWhiteSpace(item) &&
                    !item.Equals(Common.Constants.Scripts.OpeningBracket) &&
                    !item.Equals(Common.Constants.Scripts.ClosingBracket))
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
        /// Cleans the whitespace.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        public string CleanWhitespace(string line)
        {
            var cleaned = string.Join(' ', line.Trim().Replace("\t", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries));
            foreach (var item in cleanerConversionMap)
            {
                cleaned = cleaned.Replace(item.Key, item.Value);
            }
            return cleaned;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetKey(string line, char key)
        {
            return GetKey(line, key.ToString());
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetKey(string line, string key)
        {
            var cleaned = CleanWhitespace(line);
            if (cleaned.Contains(key, StringComparison.OrdinalIgnoreCase))
            {
                var prev = cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (prev == 0 || !char.IsWhiteSpace(cleaned[prev - 1]))
                {
                    var parsed = cleaned.Split(key, StringSplitOptions.RemoveEmptyEntries);
                    if (parsed.Count() > 0)
                    {
                        if (parsed.First().StartsWith("\""))
                        {
                            return quotesRegex.Match(parsed.First().Trim()).Value.Replace("\"", string.Empty);
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
        public string GetValue(string line, char key)
        {
            return GetValue(line, key.ToString());
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetValue(string line, string key)
        {
            var cleaned = CleanWhitespace(line);
            if (cleaned.Contains(key, StringComparison.OrdinalIgnoreCase))
            {
                var prev = cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (prev == 0 || (char.IsWhiteSpace(cleaned[prev - 1]) || cleaned[prev - 1] == Common.Constants.Scripts.OpeningBracket || cleaned[prev - 1] == Common.Constants.Scripts.ClosingBracket))
                {
                    var part = cleaned.Substring(cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase));
                    var parsed = part.Split(key, StringSplitOptions.RemoveEmptyEntries);
                    if (parsed.Count() > 0)
                    {
                        if (parsed.First().StartsWith("\""))
                        {
                            return quotesRegex.Match(parsed.First().Trim()).Value.Replace("\"", string.Empty);
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
        /// Prettifies the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        public string PrettifyLine(string line)
        {
            var cleaned = CleanWhitespace(line);
            foreach (var item in reverseCleanerConversionMap)
            {
                cleaned = cleaned.Replace(item.Key, item.Value);
            }
            cleaned = string.Join(' ', cleaned.Trim().Replace("\t", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return cleaned;
        }

        #endregion Methods
    }
}
