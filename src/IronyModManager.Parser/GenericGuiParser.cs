// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="GenericGuiParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class GenericGuiParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IGenericParser" />
    public class GenericGuiParser : BaseParser, IGenericParser
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            return args.File.EndsWith(Constants.GuiExtension, StringComparison.OrdinalIgnoreCase);
        }

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
                if (line.Trim().StartsWith(Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var cleaned = ClearWhitespace(line);               
                if (cleaned.StartsWith(Constants.Scripts.GuiTypesId, StringComparison.OrdinalIgnoreCase))
                {
                    openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                    closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                    // incase some wise ass opened and closed an object definition in the same line
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        sb.Clear();
                        definition = GetDefinitionInstance();
                        definition.ValueType = ValueType.Object;
                        if (cleaned.Contains(Constants.Scripts.GraphicsTypeNameId, StringComparison.OrdinalIgnoreCase))
                        {
                            var id = GetOperationValue(line, Constants.Scripts.GraphicsTypeNameId);
                            definition.Id = id;
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line);
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
                    if (line.Contains(Constants.Scripts.OpeningBracket))
                    {
                        openBrackets += line.Count(s => s == Constants.Scripts.OpeningBracket);
                    }
                    if (line.Contains(Constants.Scripts.ClosingBracket))
                    {
                        closeBrackets += line.Count(s => s == Constants.Scripts.ClosingBracket);
                    }
                    if (openBrackets - closeBrackets <= 2)
                    {
                        if (definition == null)
                        {
                            sb.AppendLine($"{Constants.Scripts.GuiTypes} {Constants.Scripts.VariableSeparatorId} {Constants.Scripts.OpeningBracket}");
                            definition = GetDefinitionInstance();
                            definition.ValueType = ValueType.Object;
                        }
                        if (cleaned.Contains(Constants.Scripts.GraphicsTypeNameId, StringComparison.OrdinalIgnoreCase))
                        {
                            var id = GetOperationValue(line, Constants.Scripts.GraphicsTypeNameId);
                            definition.Id = id;
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line);
                        OnReadObjectLine(parsingArgs);
                        if (openBrackets - closeBrackets == 1)
                        {
                            sb.AppendLine(Constants.Scripts.ClosingBracket.ToString());
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
