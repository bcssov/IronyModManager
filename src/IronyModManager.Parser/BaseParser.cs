// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-17-2020
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

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class BaseParser.
    /// Implements the <see cref="IronyModManager.Parser.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.IDefaultParser" />
    public abstract class BaseParser : IDefaultParser
    {
        #region Properties

        /// <summary>
        /// Gets the type of the parser.
        /// </summary>
        /// <value>The type of the parser.</value>
        public abstract string ParserType { get; }

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
        public virtual IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = new List<IDefinition>();
            IDefinition definition = null;
            var sb = new StringBuilder();
            int? openBrackets = null;
            int closeBrackets = 0;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Constants.Scripts.ScriptComment))
                {
                    continue;
                }
                if (!openBrackets.HasValue)
                {
                    if (ClearWhitespace(line).Contains(Constants.Scripts.DefinitionSeparator))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        sb.Clear();
                        var id = GetOperationKey(line, Constants.Scripts.SeparatorOperators);
                        definition = GetDefinitionInstance();
                        definition.Id = id;
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line);
                        OnReadObjectLine(parsingArgs);
                        // incase some wise ass opened and closed an object definition in the same line
                        if (openBrackets == closeBrackets)
                        {
                            openBrackets = null;
                            closeBrackets = 0;
                            definition.Code = sb.ToString();
                            result.Add(FinalizeObjectDefinition(parsingArgs));
                        }
                    }
                    else if (line.Trim().Contains(Constants.Scripts.VariableSeparator))
                    {
                        definition = GetDefinitionInstance();
                        var id = GetOperationKey(line, Constants.Scripts.SeparatorOperators);
                        definition.Id = id;
                        definition.Code = line;
                        var parsingArgs = ConstructArgs(args, definition, sb, openBrackets, closeBrackets, line);
                        result.Add(FinalizeVariableDefinition(parsingArgs));
                    }
                }
                else
                {
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
                    if (openBrackets == closeBrackets)
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
        /// Clears the whitespace.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        protected virtual string ClearWhitespace(string line)
        {
            return line.Trim().Replace(" ", string.Empty).Replace("\t", string.Empty);
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
        /// <returns>ParsingArgs.</returns>
        protected virtual ParsingArgs ConstructArgs(ParserArgs args, IDefinition definition, StringBuilder sb, int? openBrackets, int closeBrackets, string line)
        {
            return new ParsingArgs()
            {
                Args = args,
                ClosingBracket = closeBrackets,
                Definition = definition,
                Line = line,
                OpeningBracket = openBrackets,
                StringBuilder = sb
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
        /// <returns>System.String.</returns>
        protected virtual string FormatType(string file)
        {
            var lines = file.Split(Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries);
            var formatted = string.Join(Path.DirectorySeparatorChar, lines.Take(lines.Length - 1));
            return formatted;
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
        /// Gets the operation key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationKey(string line, params string[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }

        /// <summary>
        /// Gets the operation key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationKey(string line, params char[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }

        /// <summary>
        /// Gets the operation value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationValue(string line, params string[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        }

        /// <summary>
        /// Gets the operation value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationValue(string line, params char[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        }

        /// <summary>
        /// Maps the definition from arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual void MapDefinitionFromArgs(ParsingArgs args)
        {
            args.Definition.ContentSHA = args.Args.ContentSHA;
            args.Definition.Dependencies = args.Args.Dependencies;
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
