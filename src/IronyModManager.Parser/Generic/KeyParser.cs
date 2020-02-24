// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="KeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using IronyModManager.Parser.Default;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class KeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Default.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Generic.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Generic.IGenericParser" />
    public class KeyParser : BaseParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The event identifier
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public KeyParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

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
                var cleaned = textParser.CleanWhitespace(line);
                if (!openBrackets.HasValue)
                {
                    if (cleaned.Contains(Constants.Scripts.DefinitionSeparatorId) || cleaned.EndsWith(Constants.Scripts.VariableSeparatorId))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(textParser.GetValue(line, s))))
                        {
                            return true;
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
                    if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(textParser.GetValue(line, s))))
                    {
                        return true;
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
            var cleaned = textParser.CleanWhitespace(args.Line);
            if (args.OpeningBracket - args.ClosingBracket <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(textParser.GetValue(cleaned, s))))
            {
                string sep = string.Empty;
                var bracketLocation = cleaned.IndexOf(Constants.Scripts.OpeningBracket.ToString());
                int idLoc = -1;
                foreach (var item in Constants.Scripts.GenericKeyIds)
                {
                    idLoc = cleaned.IndexOf(item);
                    if (idLoc > -1)
                    {
                        sep = item;
                        break;
                    }
                }
                if (idLoc < bracketLocation || bracketLocation == -1 || args.Inline)
                {
                    key = textParser.GetValue(cleaned, sep);
                }
            }
            base.OnReadObjectLine(args);
        }

        #endregion Methods
    }
}
