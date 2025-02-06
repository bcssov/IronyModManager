// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 02-06-2025
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
using IronyModManager.DI;
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
    public class ParametrizedParser : IParametrizedParser
    {
        #region Fields

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
        private static readonly Regex mathRegex = new(@"@\[\s*([-+*/%()\d\s\w$]+)\s*\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The code parser
        /// </summary>
        private readonly ICodeParser codeParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParametrizedParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public ParametrizedParser(ICodeParser codeParser) => this.codeParser = codeParser;

        #endregion Constructors

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
                        return simpleResult.Value.StandardizeDirectorySeparator();
                    }

                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null);
                    var match = elObj?.Values.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        return (match.Value ?? string.Empty).Trim(Quotes).StandardizeDirectorySeparator();
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
                            return (match.Value ?? string.Empty).Trim(Quotes).StandardizeDirectorySeparator();
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
        /// <returns>System.String.</returns>
        public string Process(string code, string parameters)
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
                            var replacement = (value.Value ?? string.Empty).Trim(Quotes);
                            if (!id.Equals(Script, StringComparison.OrdinalIgnoreCase))
                            {
                                var key = $"{Terminator}{id}{Terminator}";
                                processed = processed.Replace(key, replacement, StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        processed = EvaluateMathExpression(processed);
                    }

                    return processed;
                }
                else if (elParams.Values.Count() == 1 && elParams.Values.FirstOrDefault()!.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var processed = code;
                    var elObj = elParams.Values.FirstOrDefault(p => p.Values != null)?.Values?.FirstOrDefault();
                    if (elObj is { Values: not null })
                    {
                        foreach (var value in elObj.Values)
                        {
                            var id = (value.Key ?? string.Empty).Trim(Quotes);
                            var replacement = (value.Value ?? string.Empty).Trim(Quotes);
                            if (!id.Equals(Script, StringComparison.OrdinalIgnoreCase))
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
        /// Evaluates the math expression.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>string.</returns>
        private string EvaluateMathExpression(string code)
        {
            string replaceMathExpression(string code)
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
                        catch (Exception e)
                        {
                            var log = DIResolver.Get<ILogger>();
                            log.Error(e);
                        }
                    }
                }

                return code;
            }

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
                            value.Value = replaceMathExpression((value.Value ?? string.Empty).Trim(Quotes));
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
                else if (elCode.Values.Count() == 1 && elCode.Values.FirstOrDefault()!.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
                {
                    var elObj = elCode.Values.FirstOrDefault(p => p.Values != null)?.Values?.FirstOrDefault();
                    if (elObj is { Values: not null })
                    {
                        foreach (var value in elObj.Values)
                        {
                            value.Value = replaceMathExpression((value.Value ?? string.Empty).Trim(Quotes));
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

            return code;
        }

        #endregion Methods
    }
}
