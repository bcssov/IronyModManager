// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
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
        /// The text parser
        /// </summary>
        protected readonly ITextParser textParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public BaseParser(ITextParser textParser)
        {
            this.textParser = textParser;
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
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public virtual IEnumerable<IDefinition> Parse(ParserArgs args)
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
                if (!openBrackets.HasValue)
                {
                    var cleaned = textParser.CleanWhitespace(line);
                    if (cleaned.Contains(Constants.Scripts.DefinitionSeparatorId) || cleaned.EndsWith(Constants.Scripts.VariableSeparatorId))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        sb.Clear();
                        var id = textParser.GetKey(line, Constants.Scripts.VariableSeparatorId);
                        definition = GetDefinitionInstance();
                        definition.Id = id;
                        definition.ValueType = ValueType.Object;
                        bool inline = openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets;
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, inline);
                        OnReadObjectLine(parsingArgs);
                        // incase some wise ass opened and closed an object definition in the same line
                        if (inline)
                        {
                            openBrackets = null;
                            closeBrackets = 0;
                            definition.Code = sb.ToString();
                            result.Add(FinalizeObjectDefinition(parsingArgs));
                        }
                    }
                    else if (line.Trim().Contains(Constants.Scripts.VariableSeparatorId))
                    {
                        definition = GetDefinitionInstance();
                        var id = textParser.GetKey(line, Constants.Scripts.VariableSeparatorId);
                        definition.Code = line;
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
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, true);
                        result.Add(FinalizeVariableDefinition(parsingArgs));
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
                    var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line, false);
                    OnReadObjectLine(parsingArgs);
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                        definition.Code = sb.ToString();
                        result.Add(FinalizeObjectDefinition(parsingArgs));
                    }
                }
            }
            return result;
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
        /// <param name="inline">if set to <c>true</c> [inline].</param>
        /// <returns>ParsingArgs.</returns>
        protected virtual ParsingArgs ConstructArgs(ParserArgs args, IDefinition definition, StringBuilder sb, int? openBrackets, int closeBrackets, string line, bool inline)
        {
            return new ParsingArgs()
            {
                Args = args,
                ClosingBracket = closeBrackets,
                Definition = definition,
                Line = line,
                OpeningBracket = openBrackets,
                StringBuilder = sb,
                Inline = inline
            };
        }

        /// <summary>
        /// Constructs the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>ParsingArgs.</returns>
        protected virtual ParsingArgs ConstructArgs(ParserArgs args, IDefinition definition)
        {
            return new ParsingArgs()
            {
                Args = args,
                Definition = definition,
            };
        }

        /// <summary>
        /// Finalizes the object definition.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition FinalizeObjectDefinition(ParsingArgs args)
        {
            MapDefinitionFromArgs(args);
            return args.Definition;
        }

        /// <summary>
        /// Finalizes the variable definition.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition FinalizeVariableDefinition(ParsingArgs args)
        {
            MapDefinitionFromArgs(args);
            return args.Definition;
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
            return $"{formatted}{Path.DirectorySeparatorChar}{(string.IsNullOrWhiteSpace(typeOverride) ? type : typeOverride)}";
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
        /// Maps the definition from arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual void MapDefinitionFromArgs(ParsingArgs args)
        {
            args.Definition.ContentSHA = args.Args.ContentSHA;
            args.Definition.Dependencies = args.Args.ModDependencies;
            args.Definition.ModName = args.Args.ModName;
            args.Definition.File = args.Args.File;
            var type = FormatType(args.Args.File);
            args.Definition.Type = type;
        }

        /// <summary>
        /// Called when [read object line].
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual void OnReadObjectLine(ParsingArgs args)
        {
            args.StringBuilder.AppendLine(args.Line);
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

            #endregion Properties
        }

        #endregion Classes
    }
}
