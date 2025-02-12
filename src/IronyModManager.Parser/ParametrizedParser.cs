// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 02-12-2025
// ***********************************************************************
// <copyright file="ParametrizedParser.cs" company="Mario">
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
using IronyModManager.Shared.Expressions;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class ParametrizedParser.
    /// Implements the <see cref="IParametrizedParser" />
    /// </summary>
    /// <seealso cref="IParametrizedParser" />
    /// <remarks>Initializes a new instance of the <see cref="ParametrizedParser" /> class.</remarks>
    public class ParametrizedParser(ICodeParser codeParser) : IParametrizedParser
    {
        #region Fields

        /// <summary>
        /// The escaped quote
        /// </summary>
        private const string EscapedQuote = "\\\"";

        /// <summary>
        /// The quote character
        /// </summary>
        private const char QuoteChar = '"';

        /// <summary>
        /// The quotes
        /// </summary>
        private const string Quotes = "\"";

        /// <summary>
        /// The script
        /// </summary>
        private const string Script = "script";

        /// <summary>
        /// The terminator
        /// </summary>
        private const char Terminator = '$'; // I'll be back

        /// <summary>
        /// The math regex
        /// </summary>
        private static readonly Regex mathRegex = new(@"@\[\s*([\s\S]+?)\s*\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The code parser
        /// </summary>
        private readonly ICodeParser codeParser = codeParser;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string GetScriptPath(string parameters)
        {
            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams is { Values: not null, Error: null })
            {
                if (elParams.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var simpleResult = elParams.Values.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Value));
                    if (simpleResult != null)
                    {
                        return CleanPath(simpleResult.Value);
                    }

                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null);
                    var match = elObj?.Values.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        return CleanPath(match.Value ?? string.Empty);
                    }
                }
                else if (elParams.Values.Count() == 1 && elParams.Values.FirstOrDefault()!.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null)?.Values.FirstOrDefault();
                    if (elObj != null)
                    {
                        if (!string.IsNullOrWhiteSpace(elObj.Value))
                        {
                            return elObj.Value.StandardizeDirectorySeparator();
                        }

                        var match = elObj.Values.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                        if (match != null)
                        {
                            return CleanPath(match.Value ?? string.Empty);
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Processes the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="forceProcessPath">if set to <c>true</c> [force process path].</param>
        /// <returns>System.String.</returns>
        public string Process(string code, string parameters, bool forceProcessPath = false)
        {
            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams is { Values: not null, Error: null })
            {
                if (elParams.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var processed = code;
                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null);
                    if (elObj != null)
                    {
                        foreach (var value in elObj.Values)
                        {
                            var id = (value.Key ?? string.Empty).Trim(Quotes);
                            var replaceValue = value.Value ?? string.Empty;
                            var replacement = ReplaceMathExpression(TrimQuotes(replaceValue));

                            // If it ends with escaped quotes it means we've got an expression so we need to fix quotes
                            if (replacement.EndsWith(EscapedQuote) && !replacement.StartsWith(EscapedQuote))
                            {
                                var first = replacement.IndexOf(EscapedQuote, StringComparison.OrdinalIgnoreCase);
                                var last = replacement.LastIndexOf(EscapedQuote, StringComparison.OrdinalIgnoreCase);
                                if (first > 0 && last > 0)
                                {
                                    var left = replacement[..first];
                                    var right = replacement.Substring(first + EscapedQuote.Length, last - left.Length - EscapedQuote.Length);
                                    replacement = $"{left} \"{right}\"";
                                }
                            }

                            if (!id.Equals(Script, StringComparison.OrdinalIgnoreCase) || forceProcessPath)
                            {
                                var key = $"{Terminator}{id}{Terminator}";
                                processed = processed.Replace(key, replacement, StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        processed = EvaluateMathExpression(processed);
                    }

                    return processed;
                }
                else if (elParams.Values.Count() == 1 && elParams.Values.FirstOrDefault() != null && elParams.Values.FirstOrDefault()!.Values != null &&
                         elParams.Values.FirstOrDefault()!.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var processed = code;
                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null)?.Values?.FirstOrDefault();
                    if (elObj is { Values: not null })
                    {
                        foreach (var value in elObj.Values)
                        {
                            var id = (value.Key ?? string.Empty).Trim(Quotes);
                            var replaceValue = value.Value ?? string.Empty;
                            var replacement = ReplaceMathExpression(TrimQuotes(replaceValue));

                            // If it ends with escaped quotes it means we've got an expression so we need to fix quotes
                            if (replacement.EndsWith(EscapedQuote) && !replacement.StartsWith(EscapedQuote))
                            {
                                var first = replacement.IndexOf(EscapedQuote, StringComparison.OrdinalIgnoreCase);
                                var last = replacement.LastIndexOf(EscapedQuote, StringComparison.OrdinalIgnoreCase);
                                if (first > 0 && last > 0)
                                {
                                    var left = replacement[..first];
                                    var right = replacement.Substring(first + EscapedQuote.Length, last - left.Length - EscapedQuote.Length);
                                    replacement = $"{left} \"{right}\"";
                                }
                            }

                            if (!id.Equals(Script, StringComparison.OrdinalIgnoreCase) || forceProcessPath)
                            {
                                var key = $"{Terminator}{id}{Terminator}";
                                processed = processed.Replace(key, replacement, StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        processed = EvaluateMathExpression(processed);

                        var replacementCode = codeParser.ParseScriptWithoutValidation(processed.SplitOnNewLine(), string.Empty);
                        if (replacementCode is { Values: not null, Error: null })
                        {
                            var newCode = elParams.Values.FirstOrDefault(p => p.Values != null);
                            if (newCode != null)
                            {
                                newCode.Values = replacementCode.Values;
                                processed = codeParser.FormatCode(newCode);
                                return processed;
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(elObj?.Value))
                    {
                        var replacementCode = codeParser.ParseScriptWithoutValidation(processed.SplitOnNewLine(), string.Empty);
                        if (replacementCode is { Values: not null, Error: null })
                        {
                            var newCode = elParams.Values.FirstOrDefault(p => p.Values != null);
                            if (newCode != null)
                            {
                                newCode.Values = replacementCode.Values;
                                processed = codeParser.FormatCode(newCode);
                                return processed;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Cleans the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>string.</returns>
        private string CleanPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? string.Empty : path.Trim(Quotes).StandardizeDirectorySeparator();
        }

        /// <summary>
        /// Evaluates the math expression.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>string.</returns>
        private string EvaluateMathExpression(string code)
        {
            var elCode = codeParser.ParseScriptWithoutValidation(code.SplitOnNewLine(), string.Empty);
            if (elCode is { Values: not null, Error: null })
            {
                if (elCode.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var elObj = elCode.Values.FirstOrDefault(p => p.Values != null);
                    if (elObj != null)
                    {
                        foreach (var value in elObj.Values)
                        {
                            value.Value = ReplaceMathExpression(value.Value ?? string.Empty);
                        }

                        var sb = new StringBuilder();
                        var indent = 0;
                        foreach (var node in elCode.Values)
                        {
                            sb.AppendLine(codeParser.FormatCode(node, indent));
                            indent++;
                        }

                        return sb.ToString();
                    }
                }
                else if (elCode.Values.Count() == 1)
                {
                    var item = elCode.Values.FirstOrDefault();
                    if (item != null)
                    {
                        if (item.IsSimpleType)
                        {
                            item.Value = ReplaceMathExpression(item.Value ?? string.Empty);
                        }
                        else if (item.Values != null && item.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                        {
                            var elObj = elCode.Values.FirstOrDefault(p => p.Values != null)?.Values?.FirstOrDefault();
                            if (elObj is { Values: not null })
                            {
                                foreach (var value in elObj.Values)
                                {
                                    value.Value = ReplaceMathExpression(value.Value ?? string.Empty);
                                }

                                var sb = new StringBuilder();
                                var indent = 0;
                                foreach (var node in elCode.Values)
                                {
                                    sb.AppendLine(codeParser.FormatCode(node, indent));
                                    indent++;
                                }

                                return sb.ToString();
                            }
                        }
                    }
                }
            }

            return code;
        }

        /// <summary>
        /// Replaces the math expression.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>string.</returns>
        private string ReplaceMathExpression(string code)
        {
            var matches = mathRegex.Matches(code);
            if (matches.Count != 0)
            {
                foreach (Match m in matches)
                {
                    var expression = m.Groups[1].Value;
                    var locale = MathExpression.Culture;
                    var parser = MathExpression.GetParser();
                    try
                    {
                        var result = parser.Parse(expression.Replace(".", locale.NumberFormat.NumberDecimalSeparator));
                        var output = result % 1 == 0 ? ((long)result).ToString() : result.ToString(locale);
                        code = code.Replace(m.Value, output);
                    }
                    catch
                    {
                    }
                }
            }

            return code;
        }

        /// <summary>
        /// Trims the quotes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>string.</returns>
        private string TrimQuotes(string input)
        {
            if (!string.IsNullOrWhiteSpace(input) && input[0] == QuoteChar && input[^1] == QuoteChar)
            {
                return input[1..^1];
            }

            return input;
        }

        #endregion Methods
    }
}
