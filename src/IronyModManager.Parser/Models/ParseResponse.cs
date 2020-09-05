// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="ParseResponse.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Models
{
    /// <summary>
    /// Class ParseResponse.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IParseResponse" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IParseResponse" />
    public class ParseResponse : IParseResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public IScriptError Error { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public IEnumerable<IScriptElement> Values { get; set; }

        #endregion Properties
    }
}
