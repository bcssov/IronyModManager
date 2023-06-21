
// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 06-21-2023
// ***********************************************************************
// <copyright file="ParseResponse.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
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
        /// Gets or sets a value indicating whether [use simple validation].
        /// </summary>
        /// <value><c>true</c> if [use simple validation]; otherwise, <c>false</c>.</value>
        public bool? UseSimpleValidation { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public IEnumerable<IScriptElement> Values { get; set; }

        #endregion Properties
    }
}
