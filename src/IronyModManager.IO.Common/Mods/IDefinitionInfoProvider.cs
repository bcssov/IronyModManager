// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 10-30-2024
// ***********************************************************************
// <copyright file="IDefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets the inline scripts path.
        /// </summary>
        /// <value>The inline scripts path.</value>
        string InlineScriptsPath { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fully implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is fully implemented; otherwise, <c>false</c>.</value>
        bool IsFullyImplemented { get; }

        /// <summary>
        /// Gets the merge types.
        /// </summary>
        /// <value>The merge types.</value>
        IDictionary<MergeType, List<string>> MergeTypes { get; }

        /// <summary>
        /// Gets a value indicating whether [supports inline scripts].
        /// </summary>
        /// <value><c>true</c> if [supports inline scripts]; otherwise, <c>false</c>.</value>
        bool SupportsInlineScripts { get; }

        /// <summary>
        /// Gets a value indicating whether [supports script merge].
        /// </summary>
        /// <value><c>true</c> if [supports script merge]; otherwise, <c>false</c>.</value>
        bool SupportsScriptMerge { get; }

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

        /// <summary>
        /// Determines whether [is valid encoding] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encodingInfo">The encoding information.</param>
        /// <returns><c>true</c> if [is valid encoding] [the specified path]; otherwise, <c>false</c>.</returns>
        bool IsValidEncoding(string path, Shared.EncodingInfo encodingInfo);

        #endregion Methods
    }
}
