// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 02-12-2025
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
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Games.HOI4
{
    /// <summary>
    /// Class KeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.KeyParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.KeyParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class KeyParser : Generic.KeyParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The parsing bookmark
        /// </summary>
        private bool parsingBookmark;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public KeyParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "HOI4" + nameof(KeyParser);

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
            return args.IsHOI4() && EvalStartsWith(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            parsingBookmark = args.File.StartsWith(Common.Constants.HOI4.Bookmark, StringComparison.OrdinalIgnoreCase);
            return ParseSecondLevel(args);
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (parsingBookmark && value.Key.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                return value.Value;
            }
            else if (value.Key.Equals("key", StringComparison.OrdinalIgnoreCase))
            {
                return value.Value;
            }

            return base.EvalElementForId(value);
        }

        /// <summary>
        /// Evals the starts with.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if it can evaluate, <c>false</c> otherwise.</returns>
        protected virtual bool EvalStartsWith(CanParseArgs args)
        {
            return args.File.StartsWith(Common.Constants.HOI4.Bookmark, StringComparison.OrdinalIgnoreCase)
                   || args.File.StartsWith(Common.Constants.HOI4.DifficultySettings, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
