// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 07-08-2026
// ***********************************************************************
// <copyright file="IDefaultParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Interface IDefaultParser
    /// </summary>
    public interface IDefaultParser
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is key type parser.
        /// </summary>
        /// <value><c>true</c> if this instance is key type parser; otherwise, <c>false</c>.</value>
        bool IsKeyTypeParser { get; }

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        string ParserName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        bool CanParse(CanParseArgs args);

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> Parse(ParserArgs args);

        #endregion Methods
    }
}
