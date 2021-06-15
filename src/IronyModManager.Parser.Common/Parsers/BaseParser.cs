// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 06-15-2021
// ***********************************************************************
// <copyright file="BaseParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Class BaseParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
    public abstract class BaseParser : IDefaultParser
    {
        #region Fields

        /// <summary>
        /// The simple error check lines threshold
        /// </summary>
        protected const int SimpleErrorCheckLinesThreshold = 25000;

        /// <summary>
        /// The code parser
        /// </summary>
        protected readonly ICodeParser codeParser;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public BaseParser(ICodeParser codeParser, ILogger logger)
        {
            this.codeParser = codeParser;
            this.logger = logger;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public abstract string ParserName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public abstract bool CanParse(CanParseArgs args);

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public abstract IEnumerable<IDefinition> Parse(ParserArgs args);

        /// <summary>
        /// Gets the definition instance.
        /// </summary>
        /// <returns>IDefinition.</returns>
        protected static IDefinition GetDefinitionInstance()
        {
            return DIResolver.Get<IDefinition>();
        }

        /// <summary>
        /// Constructs the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="sb">The sb.</param>
        /// <param name="openBrackets">The open brackets.</param>
        /// <param name="closeBrackets">The close brackets.</param>
        /// <param name="line">The line.</param>
        /// <param name="inline">The inline.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">The is first level.</param>
        /// <returns>IronyModManager.Parser.Common.Parsers.BaseParser.ParsingArgs.</returns>
        protected virtual ParsingArgs ConstructArgs(ParserArgs args, IDefinition definition, StringBuilder sb = null,
            int? openBrackets = null, int? closeBrackets = null, string line = Shared.Constants.EmptyParam, bool? inline = null,
            string typeOverride = Shared.Constants.EmptyParam, bool isFirstLevel = true)
        {
            return new ParsingArgs()
            {
                Args = args,
                ClosingBracket = closeBrackets.GetValueOrDefault(),
                Definition = definition,
                Line = line,
                OpeningBracket = openBrackets.GetValueOrDefault(),
                StringBuilder = sb,
                Inline = inline.GetValueOrDefault(),
                TypeOverride = typeOverride,
                IsFirstLevel = isFirstLevel
            };
        }

        /// <summary>
        /// Evals the definition identifier.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="defaultId">The default identifier.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvalDefinitionId(IEnumerable<IScriptElement> values, string defaultId)
        {
            if (values?.Count() > 0)
            {
                foreach (var item in values)
                {
                    string id = EvalElementForId(item);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        return id;
                    }
                }
            }
            return defaultId;
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvalElementForId(IScriptElement value)
        {
            return string.Empty;
        }

        /// <summary>
        /// Evals for errors only.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> EvalForErrorsOnly(ParserArgs args)
        {
            try
            {
                var error = codeParser.PerformValidityCheck(args.Lines, args.File, ShouldSwitchToBasicChecking(args.Lines));
                if (error != null)
                {
                    return new List<IDefinition>() { TranslateScriptError(error, args) };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"CWTools has encountered an error, switching to simple parser. ModName: {args.ModName} File: {args.File}");
                var error = codeParser.PerformValidityCheck(args.Lines, args.File, true);
                if (error != null)
                {
                    return new List<IDefinition>() { TranslateScriptError(error, args) };
                }
            }
            return null;
        }

        /// <summary>
        /// Formats the code.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="skipVariables">if set to <c>true</c> [skip variables].</param>
        /// <returns>System.String.</returns>
        protected virtual string FormatCode(IScriptElement element, string parent = Shared.Constants.EmptyParam, bool skipVariables = false)
        {
            void performVariableCheck(StringBuilder sb, string item)
            {
                // Ignore variables as they are separate definitions
                if (skipVariables)
                {
                    if (!item.Trim().StartsWith(Constants.Scripts.VariableId))
                    {
                        sb.AppendLine(item);
                    }
                }
                else
                {
                    sb.AppendLine(item);
                }
            }

            if (string.IsNullOrWhiteSpace(parent))
            {
                var code = codeParser.FormatCode(element, 0);
                var lines = code.SplitOnNewLine();
                var sb = new StringBuilder();
                foreach (var item in lines)
                {
                    performVariableCheck(sb, item);
                }
                return sb.ToString();
            }
            else
            {
                var code = codeParser.FormatCode(element, 1);
                var lines = code.SplitOnNewLine();
                var sb = new StringBuilder();
                sb.AppendLine($"{parent} = {{");
                foreach (var item in lines)
                {
                    performVariableCheck(sb, item);
                }
                sb.Append('}');
                return sb.ToString();
            }
        }

        /// <summary>
        /// Formats the type.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>System.String.</returns>
        protected virtual string FormatType(string file, string typeOverride = Shared.Constants.EmptyParam)
        {
            var formatted = Path.GetDirectoryName(file);
            var type = Path.GetExtension(file).Trim('.');
            if (!Shared.Constants.TextExtensions.Any(s => s.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
            {
                type = Constants.TxtType;
            }
            return $"{formatted.ToLowerInvariant()}{Path.DirectorySeparatorChar}{(string.IsNullOrWhiteSpace(typeOverride) ? type : typeOverride)}";
        }

        /// <summary>
        /// Maps the definition from arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual void MapDefinitionFromArgs(ParsingArgs args)
        {
            args.Definition.ContentSHA = args.Args.ContentSHA;
            args.Definition.Dependencies = args.Args.ModDependencies;
            args.Definition.ModName = args.Args.ModName;
            args.Definition.OriginalModName = args.Args.ModName;
            args.Definition.OriginalFileName = args.Args.File;
            args.Definition.File = args.Args.File;
            args.Definition.Type = FormatType(args.Args.File, args.TypeOverride);
        }

        /// <summary>
        /// Parses the complex types.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseComplexTypes(IEnumerable<IScriptElement> values, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam, bool isFirstLevel = true)
        {
            var result = new List<IDefinition>();
            if (values?.Count() > 0)
            {
                foreach (var item in values.Where(p => !p.IsSimpleType))
                {
                    var definition = GetDefinitionInstance();
                    var sbLangs = new StringBuilder();
                    if (item.Values != null && item.Values.Any(s => s.Key.Equals(Constants.Scripts.LanguagesId, StringComparison.OrdinalIgnoreCase)))
                    {
                        var langNode = item.Values.FirstOrDefault(p => p.Key.Equals(Constants.Scripts.LanguagesId, StringComparison.OrdinalIgnoreCase));
                        if (langNode.Values?.Count() > 0)
                        {
                            foreach (var value in langNode.Values.OrderBy(p => p.Key))
                            {
                                sbLangs.Append($"{TrimId(value.Key)}-");
                            }
                        }
                    }
                    string id = EvalDefinitionId(item.Values, item.Key);
                    if (sbLangs.Length > 0)
                    {
                        id = $"{sbLangs}{id}";
                    }
                    MapDefinitionFromArgs(ConstructArgs(args, definition, typeOverride: typeOverride, isFirstLevel: isFirstLevel));
                    definition.Id = TrimId(id);
                    definition.ValueType = ValueType.Object;
                    definition.OriginalCode = definition.Code = FormatCode(item, parent);
                    if (!isFirstLevel)
                    {
                        definition.OriginalCode = FormatCode(item, skipVariables: true);
                        definition.CodeTag = parent;
                        definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                    }
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
            return result;
        }

        /// <summary>
        /// Parses the root.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="useSimpleValidation">if set to <c>true</c> [use simple validation].</param>
        /// <param name="skipValidation">if set to <c>true</c> [skip validation].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseRoot(ParserArgs args, bool useSimpleValidation = false, bool skipValidation = false)
        {
            var result = new List<IDefinition>();
            var values = skipValidation ? codeParser.ParseScriptWithoutValidation(args.Lines) : TryParse(args, useSimpleValidation);
            if (values.Error != null)
            {
                result.Add(TranslateScriptError(values.Error, args));
            }
            else
            {
                result.AddRange(ParseSimpleTypes(values.Values, args));
                result.AddRange(ParseComplexTypes(values.Values, args));
                result.AddRange(ParseTypesForVariables(values.Values, args));
            }
            return result;
        }

        /// <summary>
        /// Parses the script tags.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="defaultId">The default identifier.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected virtual IEnumerable<string> ParseScriptTags(IEnumerable<IScriptElement> values, string defaultId)
        {
            var tags = new List<string>
            {
                TrimId(defaultId)
            };
            if (values?.Count() > 0)
            {
                foreach (var item in values)
                {
                    string id = EvalElementForId(item);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        tags.Add(TrimId(id));
                    }
                }
            }
            return tags;
        }

        /// <summary>
        /// Parses the second level.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="useSimpleValidation">if set to <c>true</c> [use simple validation].</param>
        /// <param name="skipValidation">if set to <c>true</c> [skip validation].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseSecondLevel(ParserArgs args, bool useSimpleValidation = false, bool skipValidation = false)
        {
            var result = new List<IDefinition>();
            var values = skipValidation ? codeParser.ParseScriptWithoutValidation(args.Lines) : TryParse(args, useSimpleValidation);
            if (values.Error != null)
            {
                result.Add(TranslateScriptError(values.Error, args));
            }
            else
            {
                result.AddRange(ParseSimpleTypes(values.Values, args));
                if (values.Values?.Count() > 0)
                {
                    foreach (var item in values.Values.Where(p => !p.IsSimpleType))
                    {
                        result.AddRange(ParseSimpleTypes(item.Values, args, parent: item.Key, isFirstLevel: false));
                        result.AddRange(ParseComplexTypes(item.Values, args, parent: item.Key, isFirstLevel: false));
                        result.AddRange(ParseTypesForVariables(item.Values, args, parent: item.Key, isFirstLevel: false));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the simple types.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseSimpleTypes(IEnumerable<IScriptElement> values, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam, bool isFirstLevel = true)
        {
            var result = new List<IDefinition>();
            if (values?.Count() > 0)
            {
                foreach (var item in values.Where(p => p.IsSimpleType))
                {
                    var definition = GetDefinitionInstance();
                    MapDefinitionFromArgs(ConstructArgs(args, definition, isFirstLevel: isFirstLevel));
                    definition.OriginalCode = definition.Code = FormatCode(item, parent);
                    if (!isFirstLevel)
                    {
                        definition.OriginalCode = FormatCode(item);
                        definition.CodeTag = parent;
                        definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                    }
                    bool typeAssigned = false;
                    var op = item.Operator ?? string.Empty;
                    if (op.Equals(Constants.Scripts.EqualsOperator.ToString()))
                    {
                        if (item.Key.StartsWith(Constants.Scripts.Namespace, StringComparison.OrdinalIgnoreCase))
                        {
                            typeAssigned = true;
                            definition.Id = $"{Path.GetFileNameWithoutExtension(args.File)}-{TrimId(item.Key)}";
                            definition.ValueType = ValueType.Namespace;
                        }
                        else if (item.Key.StartsWith(Constants.Scripts.VariableId))
                        {
                            typeAssigned = true;
                            definition.Id = TrimId(item.Key);
                            definition.ValueType = ValueType.Variable;
                        }
                    }
                    if (typeAssigned)
                    {
                        result.Add(definition);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the complex script nodes for variables.
        /// </summary>
        /// <param name="values">The nodes.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseTypesForVariables(IEnumerable<IScriptElement> values, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam, bool isFirstLevel = true)
        {
            var result = new List<IDefinition>();
            if (values?.Count() > 0)
            {
                foreach (var item in values)
                {
                    if (item.Values?.Count() > 0)
                    {
                        var variables = ParseSimpleTypes(item.Values, args, parent, typeOverride, isFirstLevel);
                        if (variables.Any())
                        {
                            result.AddRange(variables);
                        }
                        variables = ParseTypesForVariables(item.Values, args, parent, typeOverride, isFirstLevel);
                        if (variables.Any())
                        {
                            result.AddRange(variables);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Shoulds the switch to basic checking.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldSwitchToBasicChecking(IEnumerable<string> lines)
        {
            if (lines != null)
            {
                return lines.Count() > SimpleErrorCheckLinesThreshold || lines.Any(p => !string.IsNullOrEmpty(p) && p.Contains(Constants.Scripts.FallbackToSimpleParserComment, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        /// <summary>
        /// Translates the script error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition TranslateScriptError(IScriptError error, ParserArgs args, string typeOverride = Shared.Constants.EmptyParam)
        {
            var definition = GetDefinitionInstance();
            definition.ErrorColumn = error.Column;
            definition.ErrorLine = error.Line;
            definition.ErrorMessage = error.Message;
            definition.Id = Path.GetFileName(args.File).ToLowerInvariant();
            definition.ValueType = ValueType.Invalid;
            definition.OriginalCode = definition.Code = string.Join(Environment.NewLine, args.Lines);
            MapDefinitionFromArgs(ConstructArgs(args, definition));
            return definition;
        }

        /// <summary>
        /// Trims the identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        protected virtual string TrimId(string id)
        {
            return id.Replace("\"", string.Empty);
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="simpleChecks">if set to <c>true</c> [simple checks].</param>
        /// <returns>IParseResponse.</returns>
        protected virtual IParseResponse TryParse(ParserArgs args, bool simpleChecks)
        {
            try
            {
                if (!simpleChecks)
                {
                    simpleChecks = ShouldSwitchToBasicChecking(args.Lines);
                }
                return codeParser.ParseScript(args.Lines, args.File, simpleChecks);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"CWTools has encountered an error, switching to simple parser. ModName: {args.ModName} File: {args.File}");
                return codeParser.ParseScript(args.Lines, args.File, true);
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ParsingArgs.
        /// </summary>
        protected class ParsingArgs
        {
            #region Properties

            /// <summary>
            /// Gets or sets the arguments.
            /// </summary>
            /// <value>The arguments.</value>
            public ParserArgs Args { get; set; }

            /// <summary>
            /// Gets or sets the closing bracket.
            /// </summary>
            /// <value>The closing bracket.</value>
            public int ClosingBracket { get; set; }

            /// <summary>
            /// Gets or sets the definition.
            /// </summary>
            /// <value>The definition.</value>
            public IDefinition Definition { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ParsingArgs" /> is inline.
            /// </summary>
            /// <value><c>true</c> if inline; otherwise, <c>false</c>.</value>
            public bool Inline { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is first level.
            /// </summary>
            /// <value><c>true</c> if this instance is first level; otherwise, <c>false</c>.</value>
            public bool IsFirstLevel { get; set; }

            /// <summary>
            /// Gets or sets the line.
            /// </summary>
            /// <value>The line.</value>
            public string Line { get; set; }

            /// <summary>
            /// Gets or sets the opening bracket.
            /// </summary>
            /// <value>The opening bracket.</value>
            public int? OpeningBracket { get; set; }

            /// <summary>
            /// Gets or sets the string builder.
            /// </summary>
            /// <value>The string builder.</value>
            public StringBuilder StringBuilder { get; set; }

            /// <summary>
            /// Gets or sets the type override.
            /// </summary>
            /// <value>The type override.</value>
            public string TypeOverride { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
