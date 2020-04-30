// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-30-2020
// ***********************************************************************
// <copyright file="ScriptError.cs" company="Mario">
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
    /// Class ScriptError.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptError" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptError" />
    public class ScriptError : IScriptError
    {
        #region Properties

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        public long? Column { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        /// <value>The line.</value>
        public long? Line { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        #endregion Properties
    }
}
