// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-17-2020
// ***********************************************************************
// <copyright file="GenericScriptParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class GenericScriptParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IDefaultParser" />
    public class GenericScriptParser : BaseParser, IDefaultParser
    {
        #region Properties

        /// <summary>
        /// Gets the type of the parser.
        /// </summary>
        /// <value>The type of the parser.</value>
        public override string ParserType => Constants.GenericParserFlag;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            // Default parser needs to be invoked manually
            return false;
        }

        #endregion Methods
    }
}
