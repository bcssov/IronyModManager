// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : scrangos
// Created          : 09-02-2021
//
// Last Modified By : Mario
// Last Modified On : 09-02-2021
// ***********************************************************************
// <copyright file="IBracketValidateResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Parsers.Models
{
    /// <summary>
    /// Interface IBracketValidateResult
    /// </summary>
    public interface IBracketValidateResult
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
