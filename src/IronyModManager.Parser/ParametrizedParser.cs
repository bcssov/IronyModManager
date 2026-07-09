// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 07-10-2026
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
using IronyModManager.Parser.Common.Parsers.Models;
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
        /// Gets the first optimized script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string GetFirstOptimizedScriptPath(string parameters)
        {
            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams is not { Values: not null, Error: null })
            {
                return string.Empty;
            }

            var topLevel = elParams.Values.FirstOrDefault(IsInlineScript);
            if (topLevel != null)
            {
                if (!string.IsNullOrWhiteSpace(topLevel.Value))
                {
                    return CleanPath(topLevel.Value);
                }

                var match = topLevel.Values?.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                return CleanPath(match?.Value ?? string.Empty);
            }

            if (TryFindFirstInlineScript(elParams.Values, out _, out var inlineScript, out _))
            {
                if (!string.IsNullOrWhiteSpace(inlineScript.Value))
                {
                    return CleanPath(inlineScript.Value);
                }

                var match = inlineScript.Values?.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                return CleanPath(match?.Value ?? string.Empty);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the optimized script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string GetOptimizedScriptPath(string parameters)
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
        /// Processes the first optimized.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="logicProcessed">if set to <c>true</c> [logic processed].</param>
        /// <param name="forceProcessPath">if set to <c>true</c> [force process path].</param>
        /// <returns>System.String.</returns>
        public string ProcessFirstOptimized(string code, string parameters, out bool logicProcessed, bool forceProcessPath = false)
        {
            parameters = ReplaceEmptyQuotesOutsideStrings(parameters, "dummy_irony_empty");
            logicProcessed = false;

            if (IsCodeEmpty(code))
            {
                logicProcessed = true;
                return string.Empty;
            }

            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams is not { Values: not null, Error: null })
            {
                return string.Empty;
            }

            var root = elParams.Values.FirstOrDefault();
            if (root == null || !TryFindFirstInlineScript(elParams.Values, out var parent, out var inlineScript, out var index))
            {
                return string.Empty;
            }

            var processed = code;

            if (inlineScript.Values != null)
            {
                foreach (var value in inlineScript.Values)
                {
                    var id = (value.Key ?? string.Empty).Trim(Quotes);
                    var replaceValue = value.Value ?? string.Empty;
                    var replacement = ReplaceMathExpression(TrimQuotes(replaceValue));

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

            var replacementCode = codeParser.ParseScriptWithoutValidation(processed.SplitOnNewLine(), string.Empty);
            if (replacementCode is not { Values: not null, Error: null })
            {
                return string.Empty;
            }

            var parentValues = parent.Values.ToList();
            parentValues.RemoveAt(index);
            parentValues.InsertRange(index, replacementCode.Values);
            parent.Values = parentValues;

            logicProcessed = true;
            return FormatCode(codeParser.FormatCode(root));
        }

        /// <summary>
        /// Optimized processes logic.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="logicProcessed">if set to <c>true</c> [logic processed].</param>
        /// <param name="forceProcessPath">if set to <c>true</c> [force process path].</param>
        /// <returns>System.String.</returns>
        public string ProcessOptimized(string code, string parameters, out bool logicProcessed, bool forceProcessPath = false)
        {
            parameters = ReplaceEmptyQuotesOutsideStrings(parameters, "dummy_irony_empty");
            logicProcessed = false;

            if (IsCodeEmpty(code))
            {
                logicProcessed = true;
                return string.Empty;
            }

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

                    logicProcessed = true;
                    return FormatCode(processed);
                }
                else if (elParams.Values.Count() == 1 && elParams.Values.FirstOrDefault() != null && elParams.Values.FirstOrDefault()!.Values != null &&
                         elParams.Values.FirstOrDefault()!.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var processed = code;
                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null)?.Values?.FirstOrDefault(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase));
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
                                var newValues = newCode.Values.Where(value => !value.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)).ToList();
                                newValues.AddRange(replacementCode.Values);
                                newCode.Values = newValues;
                                processed = codeParser.FormatCode(newCode);
                                logicProcessed = true;
                                return FormatCode(processed);
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
                                logicProcessed = true;
                                return FormatCode(processed);
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
        /// Formats the code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>string.</returns>
        private string FormatCode(string code)
        {
            var sb = new StringBuilder();
            var root = codeParser.ParseScriptWithoutValidation(ReplaceEmpty(code).SplitOnNewLine(),
                string.Empty);
            foreach (var v in root.Values)
            {
                sb.AppendLine(codeParser.FormatCode(v));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether [has equals immediately before] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="index">The index.</param>
        /// <returns>bool.</returns>
        private bool HasEqualsImmediatelyBefore(string input, int index)
        {
            var i = index - 1;

            while (i >= 0 && input[i] != '\n' && input[i] != '\r' && char.IsWhiteSpace(input[i]))
            {
                i--;
            }

            return i >= 0 && input[i] == '=';
        }

        /// <summary>
        /// Determines whether provided code is empty.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>true if empty, otherwise false.</returns>
        private bool IsCodeEmpty(string code)
        {
            return string.IsNullOrWhiteSpace(code) ||
                   !code.SplitOnNewLine()
                       .Any(line =>
                       {
                           var t = line.Trim();
                           return t.Length > 0 && !t.StartsWith("#");
                       });
        }

        /// <summary>
        /// Determines whether [is inline script] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>bool.</returns>
        private bool IsInlineScript(IScriptElement element)
        {
            return element.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is match at] [the specified input].
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="index">The index.</param>
        /// <param name="token">The token.</param>
        /// <returns>bool.</returns>
        private bool IsMatchAt(string input, int index, string token)
        {
            return index + token.Length <= input.Length && input.AsSpan(index, token.Length).Equals(token.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Replaces the empty.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>string.</returns>
        private string ReplaceEmpty(string input)
        {
            const string token = "dummy_irony_empty";

            var sb = new StringBuilder(input.Length);

            for (var i = 0; i < input.Length;)
            {
                if (IsMatchAt(input, i, token))
                {
                    var hasEqualsBefore = HasEqualsImmediatelyBefore(input, i);

                    sb.Append(hasEqualsBefore ? "\"\"" : string.Empty);
                    i += token.Length;
                }
                else
                {
                    sb.Append(input[i]);
                    i++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Replaces the empty quotes outside strings.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>string.</returns>
        private string ReplaceEmptyQuotesOutsideStrings(string input, string replacement)
        {
            var sb = new StringBuilder(input.Length);
            var inString = false;

            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                var escaped = c == '"' && i > 0 && input[i - 1] == '\\';

                if (c == '"' && !escaped)
                {
                    if (!inString)
                    {
                        if (i + 1 < input.Length && input[i + 1] == '"')
                        {
                            sb.Append(replacement);
                            i++;
                            continue;
                        }

                        inString = true;
                    }
                    else
                    {
                        inString = false;
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
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

        /// <summary>
        /// Tries the find first inline script.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="inlineScript">The inline script.</param>
        /// <param name="index">The index.</param>
        /// <returns>bool.</returns>
        private bool TryFindFirstInlineScript(IEnumerable<IScriptElement> values, out IScriptElement parent, out IScriptElement inlineScript, out int index)
        {
            parent = null!;
            inlineScript = null!;
            index = -1;

            foreach (var node in values)
            {
                if (node.Values == null)
                {
                    continue;
                }

                var list = node.Values.ToList();

                for (var i = 0; i < list.Count; i++)
                {
                    if (IsInlineScript(list[i]))
                    {
                        parent = node;
                        inlineScript = list[i];
                        index = i;
                        return true;
                    }
                }

                if (TryFindFirstInlineScript(list, out parent, out inlineScript, out index))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Methods
    }
}
