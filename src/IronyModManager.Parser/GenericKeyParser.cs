// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="GenericKeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class GenericEventParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IGenericParser" />
    public class GenericKeyParser : BaseParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The event identifier
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

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
                var cleaned = ClearWhitespace(line);
                if (!openBrackets.HasValue)
                {
                    if (cleaned.Contains(Constants.Scripts.DefinitionSeparator))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyFlags.Any(s => cleaned.Contains(s, StringComparison.OrdinalIgnoreCase)))
                        {
                            return true;
                        }
                        if (openBrackets == closeBrackets)
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
                    if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyFlags.Any(s => cleaned.Contains(s, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                    if (openBrackets == closeBrackets)
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
        protected override IDefinition FinalizeObjectDefinition(ParsingArgs args)
        {
            var result = base.FinalizeObjectDefinition(args);
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
        protected override void OnReadObjectLine(ParsingArgs args)
        {
            var cleaned = ClearWhitespace(args.Line);
            if (args.OpeningBracket - args.ClosingBracket <= 1 && Constants.Scripts.GenericKeyFlags.Any(s => cleaned.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                key = GetOperationValue(args.Line, Constants.Scripts.SeparatorOperators).Replace("\"", string.Empty);
            }
            base.OnReadObjectLine(args);
        }

        #endregion Methods
    }
}
