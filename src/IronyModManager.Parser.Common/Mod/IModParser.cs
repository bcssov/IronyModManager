// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Mod
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
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
        /// <param name="descriptorModType">Type of the descriptor mod.</param>
        /// <returns>IModObject.</returns>
        IModObject Parse(IEnumerable<string> lines, DescriptorModType descriptorModType);

        #endregion Methods
    }
}
