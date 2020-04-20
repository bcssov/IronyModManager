// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-21-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="DefinesParser.cs" company="Mario">
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
    /// Class DefinesParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class DefinesParser : BaseParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinesParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public DefinesParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(DefinesParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            return args.File.StartsWith(Common.Constants.DefinesPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = new List<IDefinition>();
            int? openBrackets = null;
            int closeBrackets = 0;
            string type = string.Empty;
            IDefinition definition = null;
            StringBuilder sb = null;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Common.Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                if (!openBrackets.HasValue)
                {
                    var cleaned = textParser.CleanWhitespace(line);
                    if (cleaned.Contains(Common.Constants.Scripts.DefinitionSeparatorId) || cleaned.EndsWith(Common.Constants.Scripts.VariableSeparatorId))
                    {
                        if (string.IsNullOrEmpty(type))
                        {
                            type = textParser.GetKey(line, Common.Constants.Scripts.VariableSeparatorId);
                        }
                        openBrackets = line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                        var content = textParser.PrettifyLine(cleaned.Replace($"{type}{Common.Constants.Scripts.DefinitionSeparatorId}", string.Empty).Replace($"{type}{Common.Constants.Scripts.VariableSeparatorId}", string.Empty).Trim());
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            if (sb == null)
                            {
                                sb = new StringBuilder();
                            }
                            if (sb.Length == 0)
                            {
                                var definesType = textParser.PrettifyLine($"{type}{Common.Constants.Scripts.DefinitionSeparatorId}");
                                sb.AppendLine(definesType);
                            }
                            sb.AppendLine(content);                            
                            if (definition == null)
                            {
                                definition = GetDefinitionInstance();
                                var parsingArgs = ConstructArgs(args, definition);
                                MapDefinitionFromArgs(parsingArgs);
                                definition.Type = FormatType(args.File, $"{type}-{Common.Constants.TxtType}");
                                definition.ValueType = Common.ValueType.SpecialVariable;
                            }
                            if (string.IsNullOrEmpty(definition.Id))
                            {
                                var key = textParser.GetKey(content, Common.Constants.Scripts.VariableSeparatorId);
                                definition.Id = key;
                            }
                        }
                        if (openBrackets.GetValueOrDefault() > 0 && openBrackets - closeBrackets <= 1)
                        {
                            if (definition != null)
                            {
                                sb.AppendLine(Common.Constants.Scripts.ClosingBracket.ToString());
                                definition.Code = sb.ToString();
                                result.Add(definition);
                            }
                            definition = null;
                            sb = null;
                        }
                        if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            type = string.Empty;
                            openBrackets = null;
                            closeBrackets = 0;
                        }
                    }
                }
                else
                {
                    if (line.Contains(Common.Constants.Scripts.OpeningBracket))
                    {
                        openBrackets += line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                    }
                    if (line.Contains(Common.Constants.Scripts.ClosingBracket))
                    {
                        closeBrackets += line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                    }
                    var cleaned = textParser.CleanWhitespace(line);
                    if (cleaned.EndsWith(Common.Constants.Scripts.ClosingBracket) && openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        cleaned = cleaned[0..^1];
                    }
                    if (!string.IsNullOrWhiteSpace(cleaned))
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                        }
                        if (sb.Length == 0)
                        {
                            var definesType = textParser.PrettifyLine($"{type}{Common.Constants.Scripts.DefinitionSeparatorId}");
                            sb.AppendLine(definesType);
                        }
                        sb.AppendLine(textParser.PrettifyLine(cleaned));
                        if (definition == null)
                        {
                            definition = GetDefinitionInstance();
                            var parsingArgs = ConstructArgs(args, definition);
                            MapDefinitionFromArgs(parsingArgs);
                            definition.Type = FormatType(args.File, $"{type}-{Common.Constants.TxtType}");
                            definition.ValueType = Common.ValueType.SpecialVariable;
                        }
                        if (string.IsNullOrEmpty(definition.Id))
                        {
                            var key = textParser.GetKey(cleaned, Common.Constants.Scripts.VariableSeparatorId);
                            definition.Id = key;
                        }
                    }
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets - closeBrackets <= 1)
                    {
                        if (definition != null)
                        {
                            sb.AppendLine(Common.Constants.Scripts.ClosingBracket.ToString());
                            definition.Code = sb.ToString();
                            result.Add(definition);
                        }
                        definition = null;
                        sb = null;
                    }
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        type = string.Empty;
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
