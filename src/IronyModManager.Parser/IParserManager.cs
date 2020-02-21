// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-19-2020
//
// Last Modified By : Mario
// Last Modified On : 02-19-2020
// ***********************************************************************
// <copyright file="IParserManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Interface IParserManager
    /// </summary>
    public interface IParserManager
    {
        #region Methods

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IIndexedDefinitions.</returns>
        IEnumerable<IDefinition> Parse(ParserManagerArgs args);

        #endregion Methods
    }
}
