// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 09-02-2021
//
// Last Modified By : Mario
// Last Modified On : 01-28-2022
// ***********************************************************************
// <copyright file="IValidateParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Interface IValidateParser
    /// </summary>
    public interface IValidateParser
    {
        #region Methods

        /// <summary>
        /// Gets the bracket count.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns>IBracketValidateResult.</returns>
        public IBracketValidateResult GetBracketCount(string file, string text);

        /// <summary>
        /// Determines whether the specified text has code.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if the specified text has code; otherwise, <c>false</c>.</returns>
        public bool HasCode(string file, string text);

        /// <summary>
        /// Validates the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> Validate(ParserArgs args);

        #endregion Methods
    }
}
