// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-06-2020
// ***********************************************************************
// <copyright file="IModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IModService
    /// </summary>
    public interface IModService
    {
        #region Events

        /// <summary>
        /// Occurs when [mod definition analyze].
        /// </summary>
        event ModDefinitionAnalyzeDelegate ModDefinitionAnalyze;

        /// <summary>
        /// Occurs when [mod analyze].
        /// </summary>
        event ModDefinitionLoadDelegate ModDefinitionLoad;

        /// <summary>
        /// Occurs when [mod definition patch load].
        /// </summary>
        event ModDefinitionPatchLoadDelegate ModDefinitionPatchLoad;

        #endregion Events

        #region Methods

        /// <summary>
        /// Applies the mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ApplyModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        string BuildModUrl(IMod mod);

        /// <summary>
        /// Creates the patch definition.
        /// </summary>
        /// <param name="copy">The copy.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>IDefinition.</returns>
        IDefinition CreatePatchDefinition(IDefinition copy, string collectionName);

        /// <summary>
        /// Exports the mods asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportModsAsync(IReadOnlyCollection<IMod> mods);

        /// <summary>
        /// Finds the conflicts.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <returns>IConflictResult.</returns>
        IConflictResult FindConflicts(IIndexedDefinitions indexedDefinitions);

        /// <summary>
        /// Gets the definitions to write asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        IEnumerable<IDefinition> GetDefinitionsToWrite(IConflictResult conflictResult, IDefinition definition);

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        IEnumerable<IMod> GetInstalledMods(IGame game);

        /// <summary>
        /// Gets the mod objects.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <returns>IIndexedDefinitions.</returns>
        IIndexedDefinitions GetModObjects(IGame game, IEnumerable<IMod> mods);

        /// <summary>
        /// Loads the patch state asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        Task<IConflictResult> LoadPatchStateAsync(IConflictResult conflictResult, string collectionName);

        #endregion Methods
    }
}
