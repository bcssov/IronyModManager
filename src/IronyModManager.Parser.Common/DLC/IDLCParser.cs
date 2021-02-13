// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="IDLCParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Common.DLC
{
    /// <summary>
    /// Interface IDLCParser
    /// </summary>
    public interface IDLCParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>IDLCObject.</returns>
        IDLCObject Parse(string path, IEnumerable<string> lines);

        #endregion Methods
    }
}
