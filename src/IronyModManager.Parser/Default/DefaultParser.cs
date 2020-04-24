// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="DefaultParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser.Default
{
    /// <summary>
    /// Class DefaultParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    public class DefaultParser : BaseParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public DefaultParser(ICodeParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => nameof(DefaultParser);

        #endregion Properties
    }
}
