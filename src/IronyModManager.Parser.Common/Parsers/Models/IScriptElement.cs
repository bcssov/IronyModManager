// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 08-31-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="IScriptElement.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Parsers.Models
{
    /// <summary>
    /// Interface IScriptElement
    /// </summary>
    public interface IScriptElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is simple type.
        /// </summary>
        /// <value><c>true</c> if this instance is simple type; otherwise, <c>false</c>.</value>
        bool IsSimpleType { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>The operator.</value>
        string Operator { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        IEnumerable<IScriptElement> Values { get; set; }

        #endregion Properties
    }
}
