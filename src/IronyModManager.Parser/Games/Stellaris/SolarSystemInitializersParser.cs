// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-20-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="SolarSystemInitializersParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class SolarSystemInitializersParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class SolarSystemInitializersParser : BaseParser, IGameParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SolarSystemInitializersParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public SolarSystemInitializersParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(SolarSystemInitializersParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            return args.IsStellaris() && args.File.StartsWith(Common.Constants.Stellaris.SolarSystemInitializers, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
