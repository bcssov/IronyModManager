// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="IDefaultParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Interface IDefaultParser
    /// </summary>
    public interface IDefaultParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        IEnumerable<IDefinition> Parse(ParserArgs args);

        #endregion Methods
    }
}
