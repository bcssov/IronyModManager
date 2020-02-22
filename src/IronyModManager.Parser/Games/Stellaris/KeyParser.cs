// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-19-2020
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

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class KeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Games.Stellaris.BaseStellarisParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Games.Stellaris.BaseStellarisParser" />
    public class KeyParser : BaseStellarisParser
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
        public override bool CanParse(CanParseArgs args)
        {
            return IsStellaris(args) && (args.File.StartsWith(Constants.Stellaris.RandomNames, StringComparison.OrdinalIgnoreCase) || args.File.StartsWith(Constants.Stellaris.WorldGfx, StringComparison.OrdinalIgnoreCase));
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
            if (args.OpeningBracket - args.ClosingBracket <= 1 && Constants.Scripts.StellarisKeyIds.Any(s => !string.IsNullOrWhiteSpace(textParser.GetValue(cleaned, s))))
            {
                string sep = string.Empty;
                var bracketLocation = cleaned.IndexOf(Constants.Scripts.OpeningBracket);
                int idLoc = -1;
                foreach (var item in Constants.Scripts.StellarisKeyIds)
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
