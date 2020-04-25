// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="SimpleGFXParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class SimpleGFXParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class SimpleGFXParser : BaseParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The ids
        /// </summary>
        private static readonly string[] ids = new string[]
        {
            Constants.Scripts.ObjectTypesId,
            Constants.Scripts.SpriteTypesId,
            Constants.Scripts.BitmapFontsId,
            Constants.Scripts.PositionTypeId
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleGFXParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public SimpleGFXParser(ICodeParser codeParser) : base(codeParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + typeof(SimpleGFXParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 2;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.File.EndsWith(Constants.GfxExtension, StringComparison.OrdinalIgnoreCase) && HasPassedComplexThreshold(args.Lines);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var error = EvalSimpleParseForErrorsOnly(args);
            if (error != null)
            {
                return error;
            }

            var result = new List<IDefinition>();
            IDefinition definition = null;
            var sb = new StringBuilder();
            var langs = new List<string>();
            int? openBrackets = null;
            int closeBrackets = 0;
            string typeId = string.Empty;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var cleaned = codeParser.CleanWhitespace(line);
                var localTypeId = ids.FirstOrDefault(s => cleaned.StartsWith(s, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(localTypeId))
                {
                    typeId = localTypeId;
                    openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                    closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                    // incase some wise ass opened and closed an object definition in the same line
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        sb.Clear();
                        langs.Clear();
                        definition = GetDefinitionInstance();
                        definition.ValueType = Common.ValueType.Object;
                        var id = codeParser.GetValue(line, $"{Constants.Scripts.GraphicsTypeName}{Constants.Scripts.VariableSeparatorId}");
                        foreach (var item in Constants.Localization.Locales)
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
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, true);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        definition.Code = sb.ToString();
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
                    if (line.Contains(Constants.Scripts.OpeningBracket))
                    {
                        currentOpenBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        openBrackets += currentOpenBrackets;
                    }
                    if (line.Contains(Constants.Scripts.ClosingBracket))
                    {
                        currentCloseBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        closeBrackets += currentCloseBrackets;
                    }
                    if (openBrackets - closeBrackets >= 2 || openBrackets - previousCloseBrackets >= 2 || (currentOpenBrackets > 0 && currentOpenBrackets == currentCloseBrackets))
                    {
                        if (definition == null)
                        {
                            sb.Clear();
                            langs.Clear();
                            sb.AppendLine(codeParser.PrettifyLine($"{typeId}{Constants.Scripts.OpeningBracket}"));
                            definition = GetDefinitionInstance();
                            definition.ValueType = Common.ValueType.Object;
                            var initialKey = codeParser.GetKey(line, Constants.Scripts.VariableSeparatorId);
                            definition.Id = initialKey;
                        }
                        var id = codeParser.GetValue(line, $"{Constants.Scripts.GraphicsTypeName}{Constants.Scripts.VariableSeparatorId}");
                        foreach (var item in Constants.Localization.Locales)
                        {
                            if (line.Contains(item, StringComparison.OrdinalIgnoreCase))
                            {
                                langs.Add(item);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(id) && (openBrackets - closeBrackets <= 2 || openBrackets - previousCloseBrackets <= 2))
                        {
                            definition.Id = id;
                        }
                        var trimEnding = line.TrimEnd();
                        if (trimEnding.EndsWith(Constants.Scripts.ClosingBracket) && openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            trimEnding = trimEnding[0..^1].TrimEnd();
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, trimEnding, false);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        if (openBrackets - closeBrackets <= 1)
                        {
                            if (langs.Count > 0)
                            {
                                definition.Id = definition.Id.Insert(0, $"{string.Join("-", langs.OrderBy(p => p))}-");
                            }
                            sb.AppendLine(Constants.Scripts.ClosingBracket.ToString());
                            definition.Code = sb.ToString();
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
