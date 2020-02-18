// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="GenericGfxParser.cs" company="Mario">
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
    /// Class GenericGfxParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IGenericParser" />
    public class GenericGfxParser : BaseParser, IGenericParser
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            return args.File.EndsWith(Constants.GfxExtension);
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
                if (!openBrackets.HasValue)
                {
                    if (cleaned.Contains(Constants.Scripts.SpriteTypeId, StringComparison.OrdinalIgnoreCase))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        sb.Clear();
                        sb.AppendLine($"{Constants.Scripts.SpriteTypes} {Constants.Scripts.OpeningBracket}");
                        definition = GetDefinitionInstance();
                        if (cleaned.Contains(Constants.Scripts.GraphicsTypeNameId))
                        {
                            var id = GetOperationValue(line, Constants.Scripts.GraphicsTypeNameId);
                            definition.Id = id;
                        }
                        definition.ValueType = ValueType.Object;
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line);
                        OnReadObjectLine(parsingArgs);
                        // incase some wise ass opened and closed an object definition in the same line
                        if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            openBrackets = null;
                            closeBrackets = 0;
                            definition.Code = sb.ToString();
                            result.Add(FinalizeObjectDefinition(parsingArgs));
                        }
                    }
                }
                else
                {
                    if (cleaned.Contains(Constants.Scripts.GraphicsTypeNameId))
                    {
                        var id = GetOperationValue(line, Constants.Scripts.GraphicsTypeNameId);
                        definition.Id = id;
                    }
                    var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line);
                    OnReadObjectLine(parsingArgs);
                    if (line.Contains(Constants.Scripts.OpeningBracket))
                    {
                        openBrackets += line.Count(s => s == Constants.Scripts.OpeningBracket);
                    }
                    if (line.Contains(Constants.Scripts.ClosingBracket))
                    {
                        closeBrackets += line.Count(s => s == Constants.Scripts.ClosingBracket);
                    }
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                        sb.AppendLine(Constants.Scripts.ClosingBracket.ToString());
                        definition.Code = sb.ToString();
                        result.Add(FinalizeObjectDefinition(parsingArgs));
                    }
                }
            }
            return result;
        }

        #endregion Methods
    }
}
