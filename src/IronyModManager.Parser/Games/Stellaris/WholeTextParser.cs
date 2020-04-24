// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="WholeTextParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class WholeTextParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.WholeTextParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.WholeTextParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class WholeTextParser : Generic.WholeTextParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The equals checks
        /// </summary>
        private static readonly string[] equalsChecks = new string[]
        {
            Common.Constants.Stellaris.WeaponComponents
        };

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] startsWithChecks = new string[]
        {
            Common.Constants.Stellaris.DiploPhrases, Common.Constants.Stellaris.MapGalaxy, Common.Constants.Stellaris.NameLists,
            Common.Constants.Stellaris.SpeciesNames, Common.Constants.Stellaris.Terraform, Common.Constants.Stellaris.Portraits,
            Common.Constants.Stellaris.ComponentTags, Common.Constants.Stellaris.DiplomaticActions
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeTextParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public WholeTextParser(ICodeParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(WholeTextParser);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsStellaris() && IsValidType(args);
        }

        /// <summary>
        /// Determines whether this instance [can parse equals] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse equals] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseEquals(CanParseArgs args)
        {
            return equalsChecks.Any(s => args.File.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse starts with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse starts with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected override bool CanParseStartsWith(CanParseArgs args)
        {
            return startsWithChecks.Any(s => args.File.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is valid type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is valid type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected override bool IsValidType(CanParseArgs args)
        {
            return CanParseStartsWith(args) || CanParseEquals(args);
        }

        #endregion Methods
    }
}
