// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="FastKeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Default;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class FastKeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Default.FastDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.FastDefaultParser" />
    internal class FastKeyParser : FastDefaultParser
    {
        #region Fields

        /// <summary>
        /// The event identifier
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

        #region Methods

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
                }
            }
            base.OnReadObjectLine(args);
        }

        #endregion Methods
    }
}
