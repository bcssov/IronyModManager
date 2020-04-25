// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="FastGUIParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Default;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class FastGUIParser.
    /// Implements the <see cref="IronyModManager.Parser.Default.FastDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.FastDefaultParser" />
    internal class FastGUIParser : FastDefaultParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = new List<IDefinition>();
            IDefinition definition = null;
            var sb = new StringBuilder();
            int? openBrackets = null;
            int closeBrackets = 0;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Common.Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var cleaned = codeParser.CleanWhitespace(line);
                if (cleaned.StartsWith(Common.Constants.Scripts.GuiTypesId, StringComparison.OrdinalIgnoreCase))
                {
                    openBrackets = line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                    closeBrackets = line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                    // incase some wise ass opened and closed an object definition in the same line
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        sb.Clear();
                        definition = GetDefinitionInstance();
                        definition.ValueType = Common.ValueType.Object;
                        var id = codeParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            definition.Id = id;
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, true);
                        OnReadObjectLine(parsingArgs);
                        definition.Code = sb.ToString();
                        result.Add(FinalizeObjectDefinition(parsingArgs));
                        definition = null;
                        openBrackets = null;
                        closeBrackets = 0;
                    }
                }
                else
                {
                    int currentOpenBrackets = 0;
                    int currentCloseBrackets = 0;
                    var previousCloseBrackets = closeBrackets;
                    if (line.Contains(Common.Constants.Scripts.OpeningBracket))
                    {
                        currentOpenBrackets = line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                        openBrackets += currentOpenBrackets;
                    }
                    if (line.Contains(Common.Constants.Scripts.ClosingBracket))
                    {
                        currentCloseBrackets = line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                        closeBrackets += currentCloseBrackets;
                    }
                    if (openBrackets - closeBrackets >= 2 || openBrackets - previousCloseBrackets >= 2 || (currentOpenBrackets > 0 && currentOpenBrackets == currentCloseBrackets))
                    {
                        if (definition == null)
                        {
                            sb.Clear();
                            sb.AppendLine($"{Common.Constants.Scripts.GuiTypes} {Common.Constants.Scripts.VariableSeparatorId} {Common.Constants.Scripts.OpeningBracket}");
                            definition = GetDefinitionInstance();
                            definition.ValueType = Common.ValueType.Object;
                            var initialKey = codeParser.GetKey(line, Common.Constants.Scripts.VariableSeparatorId);
                            definition.Id = initialKey;
                        }
                        var id = codeParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        if (!string.IsNullOrWhiteSpace(id) && (openBrackets - closeBrackets <= 2 || openBrackets - previousCloseBrackets <= 2))
                        {
                            definition.Id = id;
                        }
                        var trimEnding = line.TrimEnd();
                        if (trimEnding.EndsWith(Common.Constants.Scripts.ClosingBracket) && openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            trimEnding = trimEnding[0..^1].TrimEnd();
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, trimEnding, false);
                        OnReadObjectLine(parsingArgs);
                        if (openBrackets - closeBrackets <= 1)
                        {
                            sb.AppendLine(Common.Constants.Scripts.ClosingBracket.ToString());
                            definition.Code = sb.ToString();
                            result.Add(FinalizeObjectDefinition(parsingArgs));
                            definition = null;
                        }
                    }
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                    }
                }
            }
            return result;
        }

        #endregion Methods
    }
}
