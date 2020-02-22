// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-21-2020
// ***********************************************************************
// <copyright file="IGenericParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Parser.Default;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Interface IGenericParser
    /// Implements the <see cref="IronyModManager.Parser.Default.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.IDefaultParser" />
    public interface IGenericParser : IDefaultParser
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        bool CanParse(CanParseArgs args);

        #endregion Methods
    }
}
