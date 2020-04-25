// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
// ***********************************************************************
// <copyright file="ScriptValue.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Based on CWTools samples.</summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Models
{
    /// <summary>
    /// Class ScriptValue.
    /// Implements the <see cref="IronyModManager.Parser.Models.ScriptBase" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptValue" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Models.ScriptBase" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptValue" />
    public class ScriptValue : ScriptBase, IScriptValue
    {
        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public virtual string Value { get; set; }

        #endregion Properties
    }
}
