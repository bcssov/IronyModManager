// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
// ***********************************************************************
// <copyright file="ScriptKeyValue.cs" company="Mario">
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
    /// Class ScriptKeyValue.
    /// Implements the <see cref="IronyModManager.Parser.Models.ScriptBase" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptKeyValue" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Models.ScriptBase" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptKeyValue" />
    public class ScriptKeyValue : ScriptBase, IScriptKeyValue
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public virtual string Value { get; set; }

        #endregion Properties
    }
}
