// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 02-12-2025
// ***********************************************************************
// <copyright file="InnerLayerParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Games.HOI4
{
    /// <summary>
    /// Class InnerLayerParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class InnerLayerParser : BaseParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The exact match
        /// </summary>
        private static readonly string[] exactMatch = [Common.Constants.HOI4.Decisions];

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] startsWithChecks =
        [
            Common.Constants.HOI4.Abilities, Common.Constants.HOI4.Characters, Common.Constants.HOI4.OpinionModifiers, Common.Constants.HOI4.StateCategories, Common.Constants.HOI4.Technologies, Common.Constants.HOI4.UnitLeader,
            Common.Constants.HOI4.CountryLeader, Common.Constants.HOI4.Aces, Common.Constants.HOI4.AIAreas, Common.Constants.HOI4.Buildings, Common.Constants.HOI4.Ideologies, Common.Constants.HOI4.Resources,
            Common.Constants.HOI4.Wargoals, Common.Constants.HOI4.ScriptedDiplomaticActions, Common.Constants.HOI4.Medals, Common.Constants.HOI4.Ribbons, Common.Constants.HOI4.UnitMedals, Common.Constants.HOI4.MapModes
        ];

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerLayerParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        /// <seealso cref="T:IronyModManager.Parser.Common.Parsers.IDefaultParser" />
        /// <remarks>Initializes a new instance of the <see cref="T:IronyModManager.Parser.Common.Parsers.BaseParser" /> class.</remarks>
        public InnerLayerParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "HOI4" + nameof(InnerLayerParser);

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
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsHOI4() && (EvalStartsWith(args) || EvalEquals(args));
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = ParseSecondLevel(args);
            if (Path.GetDirectoryName(args.File)!.Equals(Common.Constants.HOI4.Decisions, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in result)
                {
                    item.Type = args.File.FormatDefinitionType($"{item.CodeTag}-{Common.Constants.TxtType}");
                }
            }
            else if (args.File.StartsWith(Common.Constants.HOI4.UnitLeader, StringComparison.OrdinalIgnoreCase))
            {
                if (result.Any() && result.All(p => int.TryParse(p.Id, out _)))
                {
                    result = ParseRoot(args);
                }
            }

            return result;
        }

        /// <summary>
        /// Evals the equals.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if it can evaluate direct match, <c>false</c> otherwise.</returns>
        protected virtual bool EvalEquals(CanParseArgs args)
        {
            return exactMatch.Any(s => Path.GetDirectoryName(args.File)!.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Evals the starts with.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if it can evaluate starts with match, <c>false</c> otherwise.</returns>
        protected virtual bool EvalStartsWith(CanParseArgs args)
        {
            return startsWithChecks.Any(s => args.File.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
