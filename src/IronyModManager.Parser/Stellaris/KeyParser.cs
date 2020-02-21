// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-19-2020
//
// Last Modified By : Mario
// Last Modified On : 02-21-2020
// ***********************************************************************
// <copyright file="KeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;

namespace IronyModManager.Parser.Stellaris
{
    /// <summary>
    /// Class RandomNamesParser.
    /// Implements the <see cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    /// Implements the <see cref="IronyModManager.Parser.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    /// <seealso cref="IronyModManager.Parser.IGameParser" />
    public class KeyParser : BaseStellarisParser, IGameParser
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
            var cleaned = CleanWhitespace(args.Line);
            if (args.OpeningBracket - args.ClosingBracket <= 1 && Constants.Scripts.StellarisKeyIds.Any(s => !string.IsNullOrWhiteSpace(GetValue(cleaned, s))))
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
                    key = GetValue(cleaned, sep);
                }
            }
            base.OnReadObjectLine(args);
        }

        #endregion Methods
    }
}
