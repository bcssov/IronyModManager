// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Mod
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="IModParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Common.Mod
{
    /// <summary>
    /// Interface IModParser
    /// </summary>
    public interface IModParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IModObject.</returns>
        IModObject Parse(IEnumerable<string> lines);

        #endregion Methods
    }
}
