// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="IDefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IDefinitionInfoProvider
    /// </summary>
    public interface IDefinitionInfoProvider
    {
        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        IReadOnlyCollection<string> FIOSPaths { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can process the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns><c>true</c> if this instance can process the specified game; otherwise, <c>false</c>.</returns>
        bool CanProcess(string game);

        /// <summary>
        /// Definitions the uses fios rules.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool DefinitionUsesFIOSRules(IDefinition definition);

        /// <summary>
        /// Gets the name of the disk file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        string GetDiskFileName(IDefinition definition);

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Encoding.</returns>
        Encoding GetEncoding(IDefinition definition);

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        string GetFileName(IDefinition definition);

        #endregion Methods
    }
}
