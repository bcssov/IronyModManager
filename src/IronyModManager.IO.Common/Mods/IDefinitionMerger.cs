// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 04-03-2020
// ***********************************************************************
// <copyright file="IDefinitionMerger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IDefinitionMerger
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IDefaultDefinitionMerger" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IDefaultDefinitionMerger" />
    public interface IDefinitionMerger
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
        /// Gets the name of the file.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>System.String.</returns>
        string GetFileName(IEnumerable<IDefinition> definitions);

        /// <summary>
        /// Merges the content.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>System.String.</returns>
        string MergeContent(IEnumerable<IDefinition> definitions);

        #endregion Methods
    }
}
