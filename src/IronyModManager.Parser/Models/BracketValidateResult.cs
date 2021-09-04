// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 09-02-2021
//
// Last Modified By : Mario
// Last Modified On : 09-02-2021
// ***********************************************************************
// <copyright file="BracketValidateResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Models
{
    /// <summary>
    /// Class BracketValidateResult.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IBracketValidateResult" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IBracketValidateResult" />
    public class BracketValidateResult : IBracketValidateResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the close bracket count.
        /// </summary>
        /// <value>The close bracket count.</value>
        public int CloseBracketCount { get; set; }

        /// <summary>
        /// Gets or sets the open bracket count.
        /// </summary>
        /// <value>The open bracket count.</value>
        public int OpenBracketCount { get; set; }

        #endregion Properties
    }
}
