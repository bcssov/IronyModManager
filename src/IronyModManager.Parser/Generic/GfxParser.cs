// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="GfxParser.cs" company="Mario">
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

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class GfxParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class GfxParser : BaseParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The ids
        /// </summary>
        private static readonly string[] ids = new string[]
        {
            Common.Constants.Scripts.ObjectTypesId,
            Common.Constants.Scripts.SpriteTypesId,
            Common.Constants.Scripts.BitmapFontsId,
            Common.Constants.Scripts.PositionTypeId
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GfxParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public GfxParser(ICodeParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(GfxParser);

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
        public bool CanParse(CanParseArgs args)
        {
            return args.File.EndsWith(Common.Constants.GfxExtension, StringComparison.OrdinalIgnoreCase);
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
            var sbLangs = new StringBuilder();
            int? openBrackets = null;
            int closeBrackets = 0;
            string typeId = string.Empty;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Common.Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var cleaned = textParser.CleanWhitespace(line);
                var localTypeId = ids.FirstOrDefault(s => cleaned.StartsWith(s, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(localTypeId))
                {
                    typeId = localTypeId;
                    openBrackets = line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                    closeBrackets = line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                    // incase some wise ass opened and closed an object definition in the same line
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        sb.Clear();
                        sbLangs.Clear();
                        definition = GetDefinitionInstance();
                        definition.ValueType = Common.ValueType.Object;
                        var id = textParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        foreach (var item in Common.Constants.Localization.Locales)
                        {
                            if (line.Contains(item, StringComparison.OrdinalIgnoreCase))
                            {
                                sbLangs.Append($"{item}-");
                            }
                        }
                        if (sbLangs.Length > 0)
                        {
                            id = $"{sbLangs}{id}";
                        }
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
                            sbLangs.Clear();
                            sb.AppendLine(textParser.PrettifyLine($"{typeId}{Common.Constants.Scripts.OpeningBracket}"));
                            definition = GetDefinitionInstance();
                            definition.ValueType = Common.ValueType.Object;
                            var initialKey = textParser.GetKey(line, Common.Constants.Scripts.VariableSeparatorId);
                            definition.Id = initialKey;
                        }
                        var id = textParser.GetValue(line, $"{Common.Constants.Scripts.GraphicsTypeName}{Common.Constants.Scripts.VariableSeparatorId}");
                        foreach (var item in Common.Constants.Localization.Locales)
                        {
                            if (line.Contains(item, StringComparison.OrdinalIgnoreCase))
                            {
                                sbLangs.Append($"{item}-");
                            }
                        }
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
                            definition.Id = definition.Id.Insert(0, sbLangs.ToString());
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
