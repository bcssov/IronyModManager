// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-30-2020
// ***********************************************************************
// <copyright file="IScriptError.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Parsers.Models
{
    /// <summary>
    /// Interface IScriptError
    /// </summary>
    public interface IScriptError
    {
        #region Properties

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        long? Column { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>The line.</value>
        long? Line { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        string Message { get; set; }

        #endregion Properties
    }
}
