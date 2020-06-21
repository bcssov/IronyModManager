// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 06-21-2020
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
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;

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
        /// The complex parse lines threshold
        /// </summary>
        protected const int ComplexParseLinesThreshold = 20000;

        /// <summary>
        /// The code parser
        /// </summary>
        protected readonly ICodeParser codeParser;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// The simple parser tags
        /// </summary>
        protected List<string> SimpleParserTags;

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
        /// Constructs the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="sb">The sb.</param>
        /// <param name="openBrackets">The open brackets.</param>
        /// <param name="closeBrackets">The close brackets.</param>
        /// <param name="line">The line.</param>
        /// <param name="inline">if set to <c>true</c> [inline].</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>ParsingArgs.</returns>
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
        /// Evals the complex parse definition identifier.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="defaultId">The default identifier.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvalComplexParseDefinitionId(IEnumerable<IScriptKeyValue> values, string defaultId)
        {
            if (values?.Count() > 0)
            {
                foreach (var item in values)
                {
                    string id = EvalComplexParseKeyValueForId(item);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        return id;
                    }
                }
            }
            return defaultId;
        }

        /// <summary>
        /// Evals the complex parse for errors only.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> EvalComplexParseForErrorsOnly(ParserArgs args)
        {
            var value = codeParser.ParseScript(args.Lines, args.File);
            if (value.Error != null)
            {
                return new List<IDefinition>() { ParseScriptError(value.Error, args) };
            }
            return null;
        }

        /// <summary>
        /// Evals the complex parse key value for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvalComplexParseKeyValueForId(IScriptKeyValue value)
        {
            return string.Empty;
        }

        /// <summary>
        /// Evals for errors only.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="fallbackToSimpleParser">if set to <c>true</c> [fallback to simple parser].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> EvalForErrorsOnly(ParserArgs args, bool fallbackToSimpleParser = true)
        {
            if (HasPassedComplexThreshold(args.Lines))
            {
                return EvalSimpleParseForErrorsOnly(args);
            }
            if (fallbackToSimpleParser)
            {
                try
                {
                    return EvalComplexParseForErrorsOnly(args);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"CWTools has encountered an error, switching to simple parser. ModName: {args.ModName} File: {args.File}");
                    return EvalSimpleParseForErrorsOnly(args);
                }
            }
            return EvalComplexParseForErrorsOnly(args);
        }

        /// <summary>
        /// Evals the simple parse for errors only.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> EvalSimpleParseForErrorsOnly(ParserArgs args)
        {
            var text = string.Join(Environment.NewLine, args.Lines);
            var openBracket = text.Count(s => s == Constants.Scripts.OpeningBracket);
            var closeBracket = text.Count(s => s == Constants.Scripts.ClosingBracket);
            if (openBracket != closeBracket)
            {
                var error = DIResolver.Get<IScriptError>();
                error.Message = "Number of open and close curly brackets does not match. This indicates a syntax error somewhere in the file.";
            }
            return null;
        }

        /// <summary>
        /// Finalizes the simple parse object definition.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition FinalizeSimpleParseObjectDefinition(ParsingArgs args)
        {
            MapDefinitionFromArgs(args);
            var tags = ParseSimpleScriptTags(args);
            if (tags.Count() > 0)
            {
                foreach (var tag in tags)
                {
                    var lower = tag.ToLowerInvariant();
                    if (!args.Definition.Tags.Contains(lower))
                    {
                        args.Definition.Tags.Add(lower);
                    }
                }
            }
            SimpleParserTags = new List<string>();
            return args.Definition;
        }

        /// <summary>
        /// Finalizes the simple parse variable definition.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition FinalizeSimpleParseVariableDefinition(ParsingArgs args)
        {
            MapDefinitionFromArgs(args);
            return args.Definition;
        }

        /// <summary>
        /// Finds the code between curly braces.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>System.String.</returns>
        protected virtual string FindCodeBetweenCurlyBraces(string code)
        {
            var filtered = code.Substring(code.IndexOf("{") + 1);
            filtered = filtered.Substring(0, filtered.LastIndexOf("}")).Trim();
            return filtered;
        }

        /// <summary>
        /// Formats the code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parent">The parent.</param>
        /// <returns>System.String.</returns>
        protected virtual string FormatCode(string code, string parent)
        {
            if (string.IsNullOrWhiteSpace(parent))
            {
                return code;
            }
            var lines = code.SplitOnNewLine();
            var sb = new StringBuilder();
            sb.AppendLine($"{parent} = {{");
            foreach (var item in lines)
            {
                sb.AppendLine($"{new string(' ', 4)}{item}");
            }
            sb.Append("}");
            return sb.ToString();
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
        /// Gets the definition instance.
        /// </summary>
        /// <returns>IDefinition.</returns>
        protected IDefinition GetDefinitionInstance()
        {
            return DIResolver.Get<IDefinition>();
        }

        /// <summary>
        /// Determines whether [has passed complex threshold] [the specified lines].
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns><c>true</c> if [has passed complex threshold] [the specified lines]; otherwise, <c>false</c>.</returns>
        protected virtual bool HasPassedComplexThreshold(IEnumerable<string> lines)
        {
            return lines?.Count() > ComplexParseLinesThreshold;
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
            args.Definition.File = args.Args.File;
            args.Definition.Type = FormatType(args.Args.File, args.TypeOverride);
        }

        /// <summary>
        /// Called when [simple parse read object line].
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual void OnSimpleParseReadObjectLine(ParsingArgs args)
        {
            args.StringBuilder.AppendLine(args.Line);
        }

        /// <summary>
        /// Parses the complex root.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="fallbackToSimpleParser">if set to <c>true</c> [fallback to simple parser].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseComplexRoot(ParserArgs args, bool fallbackToSimpleParser = true)
        {
            List<IDefinition> parse()
            {
                var result = new List<IDefinition>();
                var values = codeParser.ParseScript(args.Lines, args.File);
                if (values.Error != null)
                {
                    result.Add(ParseScriptError(values.Error, args));
                }
                else
                {
                    result.AddRange(ParseComplexScriptKeyValues(values.Value.KeyValues, args));
                    result.AddRange(ParseComplexScriptNodes(values.Value.Nodes, args));
                }
                return result;
            }
            if (fallbackToSimpleParser)
            {
                try
                {
                    return parse();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"CWTools has encountered an error, switching to simple parser. ModName: {args.ModName} File: {args.File}");
                    var error = EvalSimpleParseForErrorsOnly(args);
                    if (error != null)
                    {
                        return error;
                    }
                    return ParseSimple(args, false);
                }
            }
            else
            {
                return parse();
            }
        }

        /// <summary>
        /// Parses the complex script key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseComplexScriptKeyValues(IEnumerable<IScriptKeyValue> keyValues, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam, bool isFirstLevel = true)
        {
            var result = new List<IDefinition>();
            if (keyValues?.Count() > 0)
            {
                foreach (var item in keyValues)
                {
                    var definition = GetDefinitionInstance();
                    MapDefinitionFromArgs(ConstructArgs(args, definition, isFirstLevel: isFirstLevel));
                    definition.OriginalCode = definition.Code = FormatCode(item.Code, parent);
                    if (!isFirstLevel)
                    {
                        definition.OriginalCode = item.Code;
                        definition.CodeTag = parent;
                        definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                    }
                    if (item.Key.StartsWith(Constants.Scripts.Namespace, StringComparison.OrdinalIgnoreCase))
                    {
                        definition.Id = $"{Path.GetFileNameWithoutExtension(args.File)}-{item.Key}";
                        definition.ValueType = ValueType.Namespace;
                    }
                    else
                    {
                        definition.Id = item.Key;
                        definition.ValueType = ValueType.Variable;
                    }
                    result.Add(definition);
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the complex script nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseComplexScriptNodes(IEnumerable<IScriptNode> nodes, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam, bool isFirstLevel = true)
        {
            var result = new List<IDefinition>();
            if (nodes?.Count() > 0)
            {
                foreach (var item in nodes)
                {
                    var definition = GetDefinitionInstance();
                    var sbLangs = new StringBuilder();
                    if (item.Nodes != null && item.Nodes.Any(s => s.Key.Equals(Constants.Scripts.LanguagesId, StringComparison.OrdinalIgnoreCase)))
                    {
                        var langNode = item.Nodes.FirstOrDefault(p => p.Key.Equals(Constants.Scripts.LanguagesId, StringComparison.OrdinalIgnoreCase));
                        if (langNode.Values?.Count > 0)
                        {
                            foreach (var value in langNode.Values.OrderBy(p => p.Value))
                            {
                                sbLangs.Append($"{value.Value}-");
                            }
                        }
                    }
                    string id = EvalComplexParseDefinitionId(item.KeyValues, item.Key);
                    if (sbLangs.Length > 0)
                    {
                        id = $"{sbLangs}{id}";
                    }
                    MapDefinitionFromArgs(ConstructArgs(args, definition, typeOverride: typeOverride, isFirstLevel: isFirstLevel));
                    definition.Id = id;
                    definition.ValueType = ValueType.Object;
                    definition.OriginalCode = definition.Code = FormatCode(item.Code, parent);
                    if (!isFirstLevel)
                    {
                        definition.OriginalCode = item.Code;
                        definition.CodeTag = parent;
                        definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                    }
                    var tags = ParseComplexScriptTags(item.KeyValues, item.Key);
                    if (tags.Count() > 0)
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
        /// Parses the complex script tags.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="defaultId">The default identifier.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected virtual IEnumerable<string> ParseComplexScriptTags(IEnumerable<IScriptKeyValue> values, string defaultId)
        {
            var tags = new List<string>
            {
                defaultId
            };
            foreach (var item in values)
            {
                string id = EvalComplexParseKeyValueForId(item);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    tags.Add(id);
                }
            }
            return tags;
        }

        /// <summary>
        /// Parses the complex second level.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="fallbackToSimpleParser">if set to <c>true</c> [fallback to simple parser].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseComplexSecondLevel(ParserArgs args, bool fallbackToSimpleParser = true)
        {
            List<IDefinition> parse()
            {
                var result = new List<IDefinition>();
                var value = codeParser.ParseScript(args.Lines, args.File);
                if (value.Error != null)
                {
                    result.Add(ParseScriptError(value.Error, args));
                }
                else
                {
                    if (value.Value.Nodes?.Count() > 0)
                    {
                        foreach (var item in value.Value.Nodes)
                        {
                            result.AddRange(ParseComplexScriptKeyValues(item.KeyValues, args, parent: item.Key, isFirstLevel: false));
                            result.AddRange(ParseComplexScriptNodes(item.Nodes, args, parent: item.Key, isFirstLevel: false));
                        }
                    }
                }
                return result;
            }
            if (fallbackToSimpleParser)
            {
                try
                {
                    return parse();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"CWTools has encountered an error, switching to simple parser. ModName: {args.ModName} File: {args.File}");
                    var error = EvalSimpleParseForErrorsOnly(args);
                    if (error != null)
                    {
                        return error;
                    }
                    return ParseSimple(args);
                }
            }
            else
            {
                return parse();
            }
        }

        /// <summary>
        /// Parses the script error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition ParseScriptError(IScriptError error, ParserArgs args, string typeOverride = Shared.Constants.EmptyParam)
        {
            var definition = GetDefinitionInstance();
            definition.ErrorColumn = error.Column;
            definition.ErrorLine = error.Line;
            definition.ErrorMessage = error.Message;
            definition.Id = Constants.Scripts.Invalid;
            definition.ValueType = ValueType.Invalid;
            definition.OriginalCode = definition.Code = string.Join(Environment.NewLine, args.Lines);
            MapDefinitionFromArgs(ConstructArgs(args, definition));
            return definition;
        }

        /// <summary>
        /// Parses the simple.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="isFirstLevel">if set to <c>true</c> [is first level].</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseSimple(ParserArgs args, bool isFirstLevel = true)
        {
            SimpleParserTags = new List<string>();
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
                if (!openBrackets.HasValue)
                {
                    var cleaned = codeParser.CleanWhitespace(line);
                    if (cleaned.Contains(Constants.Scripts.DefinitionSeparatorId) || cleaned.EndsWith(Constants.Scripts.VariableSeparatorId))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        sb.Clear();
                        var id = codeParser.GetKey(line, Constants.Scripts.VariableSeparatorId);
                        definition = GetDefinitionInstance();
                        definition.Id = id;
                        definition.ValueType = ValueType.Object;
                        bool inline = openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets;
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, inline, isFirstLevel: isFirstLevel);
                        OnSimpleParseReadObjectLine(parsingArgs);
                        // incase some wise ass opened and closed an object definition in the same line
                        if (inline)
                        {
                            openBrackets = null;
                            closeBrackets = 0;
                            definition.OriginalCode = definition.Code = sb.ToString();
                            if (!isFirstLevel)
                            {
                                definition.CodeTag = id.Split("=:{".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                                definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                                definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.OriginalCode);
                            }
                            result.Add(FinalizeSimpleParseObjectDefinition(parsingArgs));
                        }
                    }
                    else if (line.Trim().Contains(Constants.Scripts.VariableSeparatorId))
                    {
                        definition = GetDefinitionInstance();
                        var id = codeParser.GetKey(line, Constants.Scripts.VariableSeparatorId);
                        definition.OriginalCode = definition.Code = line;
                        if (!isFirstLevel)
                        {
                            definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.OriginalCode);
                            definition.CodeTag = id.Split("=:{".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                            definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                        }
                        if (cleaned.Contains(Constants.Scripts.NamespaceId, StringComparison.OrdinalIgnoreCase))
                        {
                            definition.Id = $"{Path.GetFileNameWithoutExtension(args.File)}-{id}";
                            definition.ValueType = ValueType.Namespace;
                        }
                        else
                        {
                            definition.Id = id;
                            definition.ValueType = ValueType.Variable;
                        }
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, true, isFirstLevel: isFirstLevel);
                        result.Add(FinalizeSimpleParseVariableDefinition(parsingArgs));
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
                    var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, false, isFirstLevel: isFirstLevel);
                    OnSimpleParseReadObjectLine(parsingArgs);
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                        definition.OriginalCode = definition.Code = sb.ToString();
                        if (!isFirstLevel)
                        {
                            definition.CodeTag = definition.Id.Split("=:{".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                            definition.CodeSeparator = Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                            definition.OriginalCode = FindCodeBetweenCurlyBraces(definition.OriginalCode);
                        }
                        result.Add(FinalizeSimpleParseObjectDefinition(parsingArgs));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the simple script tags.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected virtual IEnumerable<string> ParseSimpleScriptTags(ParsingArgs args)
        {
            if (SimpleParserTags == null)
            {
                SimpleParserTags = new List<string>();
            }
            SimpleParserTags.Add(args.Definition.Id);
            return SimpleParserTags;
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
