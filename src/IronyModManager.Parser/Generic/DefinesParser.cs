// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-30-2022
// ***********************************************************************
// <copyright file="DefinesParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

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
        #region Fields

        /// <summary>
        /// The clean comma after brace
        /// </summary>
        private static readonly Regex cleanCommaAfterBrace = new("(}\\s*)([,;])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The clean comma before brace
        /// </summary>
        private static readonly Regex cleanCommaBeforeBrace = new("[,;](\\s*})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The clean dot notation
        /// </summary>
        private static readonly Regex cleanDotNotation = new("([,;](\\s*))+(?![^{]*})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinesParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public DefinesParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
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
        public override bool CanParse(CanParseArgs args)
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
            var isLua = false;
            IEnumerable<string> lines = args.Lines;
            if (codeParser.IsLua(args.File) && lines != null && lines.Any())
            {
                isLua = true;
                var isComplex = false;
                var text = string.Join(Environment.NewLine, lines);
                var firstMatch = lines.FirstOrDefault(l => l.Contains(Common.Constants.Scripts.EqualsOperator));
                if (firstMatch != null && !firstMatch.Split(Common.Constants.Scripts.EqualsOperator, StringSplitOptions.RemoveEmptyEntries)[0].Contains('.'))
                {
                    lines = text[..(text.LastIndexOf("}") + 1)].SplitOnNewLine(false);
                    isComplex = true;
                }
                lines = codeParser.CleanCode(args.File, lines);
                text = string.Join(Environment.NewLine, lines);
                text = cleanCommaBeforeBrace.Replace(text, "$1");
                text = cleanDotNotation.Replace(text, "$2");
                text = cleanCommaAfterBrace.Replace(text, "$1");
                lines = text.SplitOnNewLine(false);
                if (isComplex)
                {
                    var newLines = new List<string>();
                    var curlyCount = 0;
                    var curlyCloseCount = 0;
                    foreach (var item in lines)
                    {
                        var line = item;
                        curlyCount += line.Count(s => s == Common.Constants.Scripts.OpenObject);
                        curlyCloseCount += line.Count(s => s == Common.Constants.Scripts.CloseObject);
                        if (curlyCount - curlyCloseCount == 2 && (line.EndsWith(',') || line.EndsWith(';')))
                        {
                            if (line.EndsWith(','))
                            {
                                line = line[..line.LastIndexOf(",")];
                            }
                            else
                            {
                                line = line[..line.LastIndexOf(";")];
                            }
                        }
                        newLines.Add(line);
                    }
                    lines = newLines;
                }
            }
            var localArgs = new ParserArgs(args)
            {
                Lines = lines
            };
            if (isLua && localArgs.ValidationType == Common.ValidationType.Full)
            {
                // Switching to simple since we butchered the formatting
                localArgs.ValidationType = Common.ValidationType.SimpleOnly;
            }
            var data = TryParse(localArgs);
            if (data.Error != null)
            {
                return new List<IDefinition>() { TranslateScriptError(data.Error, localArgs) };
            }
            var result = new List<IDefinition>();
            if (data.Values?.Count() > 0)
            {
                foreach (var dataItem in data.Values)
                {
                    if (!string.IsNullOrWhiteSpace(dataItem.Key) && !string.IsNullOrWhiteSpace(dataItem.Operator) && dataItem.Key.Contains('.'))
                    {
                        //Dot notation is used
                        var definition = GetDefinitionInstance();
                        var id = dataItem.Key.Substring(dataItem.Key.LastIndexOf(".") + 1, dataItem.Key.Length - dataItem.Key.LastIndexOf(".") - 1);
                        var type = dataItem.Key[..dataItem.Key.LastIndexOf(".")];
                        MapDefinitionFromArgs(ConstructArgs(localArgs, definition, typeOverride: $"{type}-{Common.Constants.TxtType}"));
                        definition.Id = TrimId(id);
                        definition.ValueType = ValueType.SpecialVariable;
                        definition.Code = FormatCode(dataItem);
                        definition.OriginalCode = FormatCode(dataItem, skipVariables: true);
                        var tags = ParseScriptTags(new List<IScriptElement>() { dataItem }, id);
                        if (tags.Any())
                        {
                            foreach (var tag in tags)
                            {
                                var lower = tag.ToLowerInvariant();
                                if (!definition.Tags.Contains(lower))
                                {
                                    definition.Tags.Add(lower);
                                }
                            }
                        }
                        result.Add(definition);
                    }
                    else if (dataItem.Values != null)
                    {
                        if (isLua)
                        {
                            // Turn into dot notation, easier to maintain
                            foreach (var middleValue in dataItem.Values)
                            {
                                foreach (var item in middleValue.Values)
                                {
                                    var definition = GetDefinitionInstance();
                                    var id = EvalDefinitionId(item.Values, item.Key);
                                    var type = $"{dataItem.Key}.{middleValue.Key}";
                                    MapDefinitionFromArgs(ConstructArgs(localArgs, definition, typeOverride: $"{type}-{Common.Constants.TxtType}"));
                                    definition.Id = TrimId(id);
                                    definition.OriginalId = TrimId(item.Key);
                                    definition.ValueType = ValueType.SpecialVariable;
                                    definition.OriginalCode = definition.Code = $"{type}.{FormatCode(item, skipVariables: true)}";
                                    var tags = ParseScriptTags(item.Values, item.Key);
                                    if (tags.Any())
                                    {
                                        foreach (var tag in tags)
                                        {
                                            var lower = tag.ToLowerInvariant();
                                            if (!definition.Tags.Contains(lower))
                                            {
                                                definition.Tags.Add(lower);
                                            }
                                        }
                                    }
                                    result.Add(definition);
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in dataItem.Values)
                            {
                                var definition = GetDefinitionInstance();
                                string id = EvalDefinitionId(item.Values, item.Key);
                                MapDefinitionFromArgs(ConstructArgs(localArgs, definition, typeOverride: $"{dataItem.Key}-{Common.Constants.TxtType}"));
                                definition.Id = TrimId(id);
                                definition.OriginalId = TrimId(item.Key);
                                definition.ValueType = ValueType.SpecialVariable;
                                definition.Code = FormatCode(item, dataItem.Key);
                                definition.OriginalCode = FormatCode(item, skipVariables: true);
                                definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                                definition.CodeTag = dataItem.Key;
                                var tags = ParseScriptTags(item.Values, item.Key);
                                if (tags.Any())
                                {
                                    foreach (var tag in tags)
                                    {
                                        var lower = tag.ToLowerInvariant();
                                        if (!definition.Tags.Contains(lower))
                                        {
                                            definition.Tags.Add(lower);
                                        }
                                    }
                                }
                                result.Add(definition);
                            }
                        }
                    }
                    else
                    {
                        // No operator detected means something is wrong in the mod file
                        if (string.IsNullOrWhiteSpace(dataItem.Operator))
                        {
                            var definesError = DIResolver.Get<IScriptError>();
                            definesError.Message = $"There appears to be a syntax error detected in: {localArgs.File}";
                            return new List<IDefinition>() { TranslateScriptError(definesError, localArgs) };
                        }
                    }
                }
            }
            return result;
        }

        #endregion Methods
    }
}
