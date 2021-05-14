// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 05-25-2020
//
// Last Modified By : Mario
// Last Modified On : 03-10-2021
// ***********************************************************************
// <copyright file="OverwrittenParser.cs" company="Mario">
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
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class OverwrittenParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class OverwrittenParser : BaseParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] startsWithChecks = new string[]
        {
            Common.Constants.Stellaris.PopJobs, Common.Constants.Stellaris.Traits,
            Common.Constants.Stellaris.Districts, Common.Constants.Stellaris.PlanetClasses,
            Common.Constants.Stellaris.PrescriptedCountries, Common.Constants.Stellaris.SpeciesArchetypes,
            Common.Constants.Stellaris.Buildings
        };

        /// <summary>
        /// The key type
        /// </summary>
        private bool keyType = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public OverwrittenParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(OverwrittenParser);

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
            return args.IsStellaris() && CanParseStartsWith(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            keyType = false;
            if (args.File.StartsWith(Common.Constants.Stellaris.PlanetClasses))
            {
                keyType = true;
            }
            var results = ParseRoot(args);
            if (results?.Count() > 0)
            {
                foreach (var item in results)
                {
                    if (item.ValueType == ValueType.Object)
                    {
                        item.ValueType = ValueType.OverwrittenObject;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Determines whether this instance [can parse starts with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse starts with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseStartsWith(CanParseArgs args)
        {
            return startsWithChecks.Any(s => args.File.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (keyType)
            {
                if (Common.Constants.Scripts.GenericKeys.Any(s => s.Equals(value.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    return value.Value;
                }
            }
            return base.EvalElementForId(value);
        }

        #endregion Methods
    }
}
