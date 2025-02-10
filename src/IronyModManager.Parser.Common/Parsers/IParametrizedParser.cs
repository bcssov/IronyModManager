// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 02-10-2025
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
        /// Gets the script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        string GetScriptPath(string parameters);

        /// <summary>
        /// Processes the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="forceProcessPath">if set to <c>true</c> [force process path].</param>
        /// <returns>System.String.</returns>
        string Process(string code, string parameters, bool forceProcessPath = false);

        #endregion Methods
    }
}
