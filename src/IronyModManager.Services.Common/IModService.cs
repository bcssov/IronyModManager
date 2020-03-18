// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2020
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

        #endregion Events

        #region Methods

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        string BuildModUrl(IMod mod);

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

        #endregion Methods
    }
}
