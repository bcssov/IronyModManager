// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="KeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class KeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class KeyParser : BaseParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public KeyParser(ICodeParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(KeyParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 10;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            int? openBrackets = null;
            int closeBrackets = 0;
            foreach (var line in args.Lines)
            {
                var cleaned = codeParser.CleanWhitespace(line);
                if (!openBrackets.HasValue)
                {
                    if (cleaned.Contains(Common.Constants.Scripts.DefinitionSeparatorId) || cleaned.EndsWith(Common.Constants.Scripts.VariableSeparatorId))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(codeParser.GetValue(line, s))))
                        {
                            int idLoc = -1;
                            foreach (var item in Constants.Scripts.GenericKeyIds)
                            {
                                idLoc = cleaned.IndexOf(item);
                                if (idLoc > -1)
                                {
                                    break;
                                }
                            }
                            if (cleaned.Substring(0, idLoc).Count(s => s == Constants.Scripts.OpeningBracket) == 1)
                            {
                                return true;
                            }
                        }
                        if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            openBrackets = null;
                            closeBrackets = 0;
                        }
                    }
                }
                else
                {
                    openBrackets += line.Count(s => s == Constants.Scripts.OpeningBracket);
                    closeBrackets += line.Count(s => s == Constants.Scripts.ClosingBracket);
                    if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(codeParser.GetValue(line, s))))
                    {
                        var bracketLocation = cleaned.IndexOf(Constants.Scripts.OpeningBracket.ToString());
                        int idLoc = -1;
                        foreach (var item in Constants.Scripts.GenericKeyIds)
                        {
                            idLoc = cleaned.IndexOf(item);
                            if (idLoc > -1)
                            {
                                break;
                            }
                        }
                        if (idLoc < bracketLocation || bracketLocation == -1)
                        {
                            return true;
                        }
                    }
                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            // CWTools is slow on large files, so skip these and use a legacy parser
            if (args.Lines.Count() > MaxLines)
            {
                var parser = new FastKeyParser();
                return parser.Parse(args);
            }
            return ParseRoot(args);
        }

        /// <summary>
        /// Evals the key value for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalKeyValueForId(IScriptKeyValue value)
        {
            if (Constants.Scripts.GenericKeys.Any(s => s.Equals(value.Key, StringComparison.OrdinalIgnoreCase)))
            {
                return value.Value;
            }
            return base.EvalKeyValueForId(value);
        }

        #endregion Methods
    }
}
