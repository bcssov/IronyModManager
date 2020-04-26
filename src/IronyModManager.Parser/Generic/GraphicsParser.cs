// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-26-2020
// ***********************************************************************
// <copyright file="GraphicsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class GraphicsParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.BaseGraphicsParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.BaseGraphicsParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class GraphicsParser : BaseGraphicsParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public GraphicsParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(GraphicsParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 2;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return !HasPassedComplexThreshold(args.Lines) && (args.File.EndsWith(Common.Constants.GuiExtension, StringComparison.OrdinalIgnoreCase) || args.File.EndsWith(Common.Constants.GfxExtension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            return ParseComplexFirstLevel(args);
        }

        /// <summary>
        /// Evals the complex parse key value for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalComplexParseKeyValueForId(IScriptKeyValue value)
        {
            if (Common.Constants.Scripts.GraphicsTypeName.Equals(value.Key, StringComparison.OrdinalIgnoreCase))
            {
                return value.Value;
            }
            return base.EvalComplexParseKeyValueForId(value);
        }

        /// <summary>
        /// Parses the simple.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected override IEnumerable<IDefinition> ParseSimple(ParserArgs args)
        {
            // Called as a part of a fallback strategy
            if (args.File.EndsWith(Common.Constants.GuiExtension, StringComparison.OrdinalIgnoreCase))
            {
                return ParseGUI(args);
            }
            else
            {
                return ParseGFX(args);
            }
        }

        #endregion Methods
    }
}
