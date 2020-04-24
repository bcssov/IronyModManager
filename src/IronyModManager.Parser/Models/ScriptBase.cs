// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
// ***********************************************************************
// <copyright file="ScriptBase.cs" company="Mario">
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
    /// Class ScriptBase.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptBase" />
    public abstract class ScriptBase : IScriptBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public virtual string Code { get; set; }

        #endregion Properties
    }
}
