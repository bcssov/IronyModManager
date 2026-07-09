// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 07-08-2026
// ***********************************************************************
// <copyright file="IParametrizedParser.cs" company="Mario">
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
    /// Interface IParametrizedParser
    /// </summary>
    public interface IParametrizedParser
    {
        #region Methods

        /// <summary>
        /// Gets the first optimized script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        string GetFirstOptimizedScriptPath(string parameters);

        /// <summary>
        /// Gets the optimized script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        string GetOptimizedScriptPath(string parameters);

        /// <summary>
        /// Processes the first optimized.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="logicProcessed">if set to <c>true</c> [logic processed].</param>
        /// <param name="forceProcessPath">if set to <c>true</c> [force process path].</param>
        /// <returns>System.String.</returns>
        string ProcessFirstOptimized(string code, string parameters, out bool logicProcessed, bool forceProcessPath = false);

        /// <summary>
        /// Optimized processes logic.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="logicProcessed">if set to <c>true</c> [logic processed].</param>
        /// <param name="forceProcessPath">if set to <c>true</c> [force process path].</param>
        /// <returns>System.String.</returns>
        string ProcessOptimized(string code, string parameters, out bool logicProcessed, bool forceProcessPath = false);

        #endregion Methods
    }
}
