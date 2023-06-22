
// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 06-21-2023
// ***********************************************************************
// <copyright file="IParseResponse.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Parsers.Models
{

    /// <summary>
    /// Interface IParseResponse
    /// </summary>
    public interface IParseResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        IScriptError Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use simple validation].
        /// </summary>
        /// <value><c>true</c> if [use simple validation]; otherwise, <c>false</c>.</value>
        bool? UseSimpleValidation { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        IEnumerable<IScriptElement> Values { get; set; }

        #endregion Properties
    }
}
