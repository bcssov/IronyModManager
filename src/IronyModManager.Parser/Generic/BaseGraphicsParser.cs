// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-26-2020
//
// Last Modified By : Mario
// Last Modified On : 06-21-2020
// ***********************************************************************
// <copyright file="BaseGraphicsParser.cs" company="Mario">
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
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class BaseGraphicsParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    public abstract class BaseGraphicsParser : BaseParser
    {
        #region Fields

        /// <summary>
        /// The ids
        /// </summary>
        private static readonly string[] gfxIds = new string[]
        {
            Common.Constants.Scripts.ObjectTypesId,
            Common.Constants.Scripts.SpriteTypesId,
            Common.Constants.Scripts.BitmapFontsId,
            Common.Constants.Scripts.PositionTypeId
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGraphicsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public BaseGraphicsParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the GFX.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected IEnumerable<IDefinition> ParseGFX(ParserArgs args)
        {
            var error = EvalSimpleParseForErrorsOnly(args);
            if (error != null)
            {
                return error;
            }

            SimpleParserTags = new List<string>();
            var result = new List<IDefinition>();
            IDefinition definition = null;
            var sb = new StringBuilder();
            var langs = new List<string>();
            int? openBrackets = null;
            int closeBrackets = 0;
            string typeId = string.Empty;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Common.Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var cleaned = codeParser.CleanWhitespace(line);
                var localTypeId = gfxIds.FirstOrDefault(s => cleaned.StartsWith(s, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(localTypeId))
                {
                    typeId = localTypeId;
                    openBrackets = line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                    closeBrackets = line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                    // incase some wise ass opened and closed an object definition in the same line
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        sb.Clear();
                        langs.Clear();
                        definition = GetDefinitionInstance();
                        definition.ValueType = Common.ValueType.Object;
                        var id = codeParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        foreach (var item in Common.Constants.Localization.Locales)
                        {
                            if (line.Contains(item, StringComparison.OrdinalIgnoreCase))
                            {
                                langs.Add(item);
                            }
                        }
                        if (langs.Count > 0)
                        {
                            id = $"{string.Join("-", langs.OrderBy(p => p))}{id}";
                        }
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            definition.Id = id;
                            SimpleParserTags.Add(id);
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, true, isFirstLevel: false);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        definition.Code = sb.ToString();
                        definition.CodeTag = typeId.Split("=:{".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                        definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                        definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.Code);
                        result.Add(FinalizeSimpleParseObjectDefinition(parsingArgs));
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
                            langs.Clear();
                            sb.AppendLine(codeParser.PrettifyLine($"{typeId}{Common.Constants.Scripts.OpeningBracket}"));
                            definition = GetDefinitionInstance();
                            definition.ValueType = Common.ValueType.Object;
                            var initialKey = codeParser.GetKey(line, Common.Constants.Scripts.VariableSeparatorId);
                            definition.Id = initialKey;
                            SimpleParserTags.Add(initialKey);
                        }
                        var id = codeParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        foreach (var item in Common.Constants.Localization.Locales)
                        {
                            if (line.Contains(item, StringComparison.OrdinalIgnoreCase))
                            {
                                langs.Add(item);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(id) && (openBrackets - closeBrackets <= 2 || openBrackets - previousCloseBrackets <= 2))
                        {
                            definition.Id = id;
                            SimpleParserTags.Add(id);
                        }
                        var trimEnding = line.TrimEnd();
                        if (trimEnding.EndsWith(Common.Constants.Scripts.ClosingBracket) && openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            trimEnding = trimEnding[0..^1].TrimEnd();
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, trimEnding, false, isFirstLevel: false);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        if (openBrackets - closeBrackets <= 1)
                        {
                            if (langs.Count > 0)
                            {
                                if (SimpleParserTags.Contains(definition.Id))
                                {
                                    SimpleParserTags.Remove(definition.Id);
                                }
                                definition.Id = definition.Id.Insert(0, $"{string.Join("-", langs.OrderBy(p => p))}-");
                                SimpleParserTags.Add(definition.Id);
                            }
                            sb.AppendLine(Common.Constants.Scripts.ClosingBracket.ToString());
                            definition.Code = sb.ToString();
                            definition.CodeTag = typeId.Split("=:{".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                            definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                            definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.Code);
                            result.Add(FinalizeSimpleParseObjectDefinition(parsingArgs));
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

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected IEnumerable<IDefinition> ParseGUI(ParserArgs args)
        {
            var error = EvalSimpleParseForErrorsOnly(args);
            if (error != null)
            {
                return error;
            }

            SimpleParserTags = new List<string>();
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
                            SimpleParserTags.Add(id);
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, true, isFirstLevel: false);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        definition.Code = sb.ToString();
                        definition.CodeTag = Common.Constants.Scripts.GuiTypes;
                        definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                        definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.Code);
                        result.Add(FinalizeSimpleParseObjectDefinition(parsingArgs));
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
                            SimpleParserTags.Add(initialKey);
                        }
                        var id = codeParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        if (!string.IsNullOrWhiteSpace(id) && (openBrackets - closeBrackets <= 2 || openBrackets - previousCloseBrackets <= 2))
                        {
                            definition.Id = id;
                            SimpleParserTags.Add(id);
                        }
                        var trimEnding = line.TrimEnd();
                        if (trimEnding.EndsWith(Common.Constants.Scripts.ClosingBracket) && openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            trimEnding = trimEnding[0..^1].TrimEnd();
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, trimEnding, false, isFirstLevel: false);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        if (openBrackets - closeBrackets <= 1)
                        {
                            sb.AppendLine(Common.Constants.Scripts.ClosingBracket.ToString());
                            definition.Code = sb.ToString();
                            definition.CodeTag = Common.Constants.Scripts.GuiTypes;
                            definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                            definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.Code);
                            result.Add(FinalizeSimpleParseObjectDefinition(parsingArgs));
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
