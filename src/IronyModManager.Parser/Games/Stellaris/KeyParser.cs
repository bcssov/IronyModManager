// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 09-10-2024
//
// Last Modified By : Mario
// Last Modified On : 10-29-2024
// ***********************************************************************
// <copyright file="KeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class KeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.KeyParser" />
    /// Implements the <see cref="IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.KeyParser" />
    /// <seealso cref="IGameParser" />
    public class KeyParser(ICodeParser codeParser, ILogger logger) : Generic.KeyParser(codeParser, logger), IGameParser
    {
        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(KeyParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public override int Priority => 10;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsStellaris() && args.File.StartsWith(Common.Constants.Stellaris.SpecialProjects);
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            return value.Key.Equals("key", StringComparison.OrdinalIgnoreCase) ? value.Value : base.EvalElementForId(value);
        }

        #endregion Methods
    }
}
