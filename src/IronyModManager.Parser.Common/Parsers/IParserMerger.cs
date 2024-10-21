// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-18-2024
//
// Last Modified By : Mario
// Last Modified On : 10-18-2024
// ***********************************************************************
// <copyright file="IParserMerger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Interface IParserMerger
    /// </summary>
    public interface IParserMerger
    {
        #region Methods

        /// <summary>
        /// Merges the top level.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetCode">The target code.</param>
        /// <param name="addToEnd">if set to <c>true</c> [add to end].</param>
        /// <returns>System.String.</returns>
        string MergeTopLevel(IEnumerable<string> code, string fileName, string target, IEnumerable<string> targetCode, bool addToEnd);

        #endregion Methods
    }
}
