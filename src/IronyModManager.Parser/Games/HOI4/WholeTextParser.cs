// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
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
using IronyModManager.Shared;

namespace IronyModManager.Parser.Games.HOI4
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
            Common.Constants.HOI4.GraphicalCultureType
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeTextParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public WholeTextParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "HOI4" + nameof(WholeTextParser);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsHOI4() && IsValidType(args);
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
        /// Gets the file tag code.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>System.String.</returns>
        protected override string GetFileTagCode(string file, IEnumerable<string> lines)
        {
            var cleaned = codeParser.CleanCode(file, lines);
            return base.GetFileTagCode(file, cleaned);
        }

        /// <summary>
        /// Determines whether [is file name tag] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is file name tag] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected override bool IsFileNameTag(ParserArgs args)
        {
            return args.File.Equals(Common.Constants.HOI4.GraphicalCultureType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is valid type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is valid type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected override bool IsValidType(CanParseArgs args)
        {
            return CanParseEquals(args);
        }

        #endregion Methods
    }
}
