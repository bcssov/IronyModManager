// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-26-2020
//
// Last Modified By : Mario
// Last Modified On : 06-14-2020
// ***********************************************************************
// <copyright file="BaseKeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class BaseKeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    public abstract class BaseKeyParser : BaseParser
    {
        #region Fields

        /// <summary>
        /// The event identifier
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseKeyParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public BaseKeyParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Evals the contains key elements.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool EvalContainsKeyElements(CanParseArgs args)
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
                        openBrackets = line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                        if (openBrackets - closeBrackets <= 1 && Common.Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(codeParser.GetValue(line, s))))
                        {
                            int idLoc = -1;
                            foreach (var item in Common.Constants.Scripts.GenericKeyIds)
                            {
                                idLoc = cleaned.IndexOf(item);
                                if (idLoc > -1)
                                {
                                    break;
                                }
                            }
                            if (cleaned.Substring(0, idLoc).Count(s => s == Common.Constants.Scripts.OpeningBracket) == 1)
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
                    openBrackets += line.Count(s => s == Common.Constants.Scripts.OpeningBracket);
                    closeBrackets += line.Count(s => s == Common.Constants.Scripts.ClosingBracket);
                    if (openBrackets - closeBrackets <= 1 && Common.Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(codeParser.GetValue(line, s))))
                    {
                        var bracketLocation = cleaned.IndexOf(Common.Constants.Scripts.OpeningBracket.ToString());
                        int idLoc = -1;
                        foreach (var item in Common.Constants.Scripts.GenericKeyIds)
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
        /// Finalizes the object definition.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IDefinition.</returns>
        protected override IDefinition FinalizeSimpleParseObjectDefinition(ParsingArgs args)
        {
            var result = base.FinalizeSimpleParseObjectDefinition(args);
            if (!string.IsNullOrWhiteSpace(key))
            {
                result.Id = key;
                key = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Called when [read object line].
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnSimpleParseReadObjectLine(ParsingArgs args)
        {
            var cleaned = codeParser.CleanWhitespace(args.Line);
            if (args.OpeningBracket - args.ClosingBracket <= 1 && Common.Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(codeParser.GetValue(cleaned, s))))
            {
                string sep = string.Empty;
                var bracketLocation = cleaned.IndexOf(Common.Constants.Scripts.OpeningBracket.ToString());
                int idLoc = -1;
                foreach (var item in Common.Constants.Scripts.GenericKeyIds)
                {
                    idLoc = cleaned.IndexOf(item);
                    if (idLoc > -1)
                    {
                        sep = item;
                        break;
                    }
                }
                if (idLoc < bracketLocation || bracketLocation == -1 || (args.Inline && cleaned.Substring(0, idLoc).Count(s => s == Common.Constants.Scripts.OpeningBracket) == 1))
                {
                    key = codeParser.GetValue(cleaned, sep);
                    SimpleParserTags.Add(key);
                }
            }
            base.OnSimpleParseReadObjectLine(args);
        }

        #endregion Methods
    }
}
