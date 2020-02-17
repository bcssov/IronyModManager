// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-17-2020
// ***********************************************************************
// <copyright file="OnActionsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Stellaris
{
    /// <summary>
    /// Class OnActionsParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IParser" />
    public class OnActionsParser : BaseParser, IParser
    {
        #region Properties

        /// <summary>
        /// Gets the type of the game.
        /// </summary>
        /// <value>The type of the game.</value>
        public string GameType => Shared.Constants.GamesTypes.Stellaris;

        /// <summary>
        /// Gets the type of the parser.
        /// </summary>
        /// <value>The type of the parser.</value>
        public override string ParserType => Constants.StellarisOnActionsFlag;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.File.StartsWith(Constants.StellarisOnActionsFlag, StringComparison.OrdinalIgnoreCase) && args.GameType.Equals(GameType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            // This type is a bit different and only will conflict in filenames.
            var def = GetDefinitionInstance();
            var parsingArgs = ConstructArgs(args, def, null, null, 0, null);
            MapDefinitionFromArgs(parsingArgs);
            def.Code = string.Join(Environment.NewLine, args.Lines);
            def.Id = args.File.Split(Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            return new List<IDefinition> { def };
        }

        #endregion Methods
    }
}
