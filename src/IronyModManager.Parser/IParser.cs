// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2020
// ***********************************************************************
// <copyright file="IParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Interface IParser
    /// Implements the <see cref="IronyModManager.Parser.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.IDefaultParser" />
    public interface IParser : IDefaultParser
    {
        #region Properties

        /// <summary>
        /// Gets the type of the game.
        /// </summary>
        /// <value>The type of the game.</value>
        string GameType { get; }

        #endregion Properties
    }
}
