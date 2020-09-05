// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 08-31-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="ScriptElement.cs" company="Mario">
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
    /// Class ScriptElement.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptElement" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptElement" />
    public class ScriptElement : IScriptElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is simple type.
        /// </summary>
        /// <value><c>true</c> if this instance is simple type; otherwise, <c>false</c>.</value>
        public virtual bool IsSimpleType { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public virtual string Operator { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        public virtual IEnumerable<IScriptElement> Values { get; set; }

        #endregion Properties
    }
}
