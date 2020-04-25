// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="IGenericParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Interface IGenericParser
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
    public interface IGenericParser : IDefaultParser
    {
        #region Properties

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        int Priority { get; }

        #endregion Properties
    }
}
