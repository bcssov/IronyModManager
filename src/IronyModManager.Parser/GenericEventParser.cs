// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2020
// ***********************************************************************
// <copyright file="GenericEventParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class GenericEventParser.
    /// Implements the <see cref="IronyModManager.Parser.GenericScriptParser" />
    /// Implements the <see cref="IronyModManager.Parser.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.GenericScriptParser" />
    /// <seealso cref="IronyModManager.Parser.IDefaultParser" />
    public class GenericEventParser : GenericScriptParser, IDefaultParser
    {
        #region Fields

        /// <summary>
        /// The event identifier
        /// </summary>
        private string eventId = string.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the type of the parser.
        /// </summary>
        /// <value>The type of the parser.</value>
        public override string ParserType => Constants.GenericEventFlag;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.Type.Equals(ParserType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            return base.Parse(args);
        }

        /// <summary>
        /// Finalizes the object definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="line">The line.</param>
        /// <returns>IDefinition.</returns>
        protected override IDefinition FinalizeObjectDefinition(IDefinition definition, string line)
        {
            if (!string.IsNullOrWhiteSpace(eventId))
            {
                definition.Id = eventId;
                eventId = string.Empty;
            }
            return base.FinalizeObjectDefinition(definition, line);
        }

        /// <summary>
        /// Called when [read object line].
        /// </summary>
        /// <param name="line">The line.</param>
        protected override void OnReadObjectLine(string line)
        {
            if (ClearWhitespace(line).Contains(Constants.Scripts.EventId))
            {
                eventId = GetOperationValue(line, Constants.Scripts.SeparatorOperators);
            }
            base.OnReadObjectLine(line);
        }

        #endregion Methods
    }
}
