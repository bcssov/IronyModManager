
// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 06-13-2023
// ***********************************************************************
// <copyright file="IModPatchCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared.Models;

namespace IronyModManager.Services.Common
{

    /// <summary>
    /// Interface IModPatchCollectionService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IModPatchCollectionService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Adds the custom mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> AddCustomModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Adds the mods to ignore list.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="mods">The mods.</param>
        void AddModsToIgnoreList(IConflictResult conflictResult, IEnumerable<string> mods);

        /// <summary>
        /// Applies the mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ApplyModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Cleans the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CleanPatchCollectionAsync(string collectionName);

        /// <summary>
        /// Copies the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="newCollectionName">New name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CopyPatchCollectionAsync(string collectionName, string newCollectionName);

        /// <summary>
        /// Creates the patch definition asynchronous.
        /// </summary>
        /// <param name="copy">The copy.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IDefinition&gt;.</returns>
        Task<IDefinition> CreatePatchDefinitionAsync(IDefinition copy, string collectionName);

        /// <summary>
        /// Evals the definition priority.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IPriorityDefinitionResult.</returns>
        IPriorityDefinitionResult EvalDefinitionPriority(IEnumerable<IDefinition> definitions);

        /// <summary>
        /// Finds the conflicts asynchronous.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="patchStateMode">The patch state mode.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        Task<IConflictResult> FindConflictsAsync(IIndexedDefinitions indexedDefinitions, IList<string> modOrder, PatchStateMode patchStateMode);

        /// <summary>
        /// Gets the bracket count.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns>IBracketValidateResult.</returns>
        public IBracketValidateResult GetBracketCount(string file, string text);

        /// <summary>
        /// Gets the ignored mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        IReadOnlyList<string> GetIgnoredMods(IConflictResult conflictResult);

        /// <summary>
        /// Gets the mod objects asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Task&lt;IIndexedDefinitions&gt;.</returns>
        Task<IIndexedDefinitions> GetModObjectsAsync(IGame game, IEnumerable<IMod> mods, string collectionName, PatchStateMode mode);

        /// <summary>
        /// Gets the patch state mode asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;PatchStateMode&gt;.</returns>
        Task<PatchStateMode> GetPatchStateModeAsync(string collectionName);

        /// <summary>
        /// Ignores the mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> IgnoreModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Initializes the patch state asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        Task<IConflictResult> InitializePatchStateAsync(IConflictResult conflictResult, string collectionName);

        /// <summary>
        /// Invalidates the state of the patch mod.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool InvalidatePatchModState(string collectionName);

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod].
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod]; otherwise, <c>false</c>.</returns>
        bool IsPatchMod(IMod mod);

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod name].
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod name]; otherwise, <c>false</c>.</returns>
        bool IsPatchMod(string modName);

        /// <summary>
        /// Loads the definition contents asynchronous.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> LoadDefinitionContentsAsync(IDefinition definition, string collectionName);

        /// <summary>
        /// Patches the has game definitions asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> PatchHasGameDefinitionsAsync(string collectionName);

        /// <summary>
        /// Patches the mod needs update asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadOrder">The load order.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> PatchModNeedsUpdateAsync(string collectionName, IReadOnlyCollection<string> loadOrder);

        /// <summary>
        /// Renames the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="newCollectionName">New name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> RenamePatchCollectionAsync(string collectionName, string newCollectionName);

        /// <summary>
        /// Resets the custom conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ResetCustomConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName);

        /// <summary>
        /// Resets the ignored conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ResetIgnoredConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName);

        /// <summary>
        /// Resets the patch state cache.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ResetPatchStateCache();

        /// <summary>
        /// Resets the resolved conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ResetResolvedConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName);

        /// <summary>
        /// Resolves the full definition path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        string ResolveFullDefinitionPath(IDefinition definition);

        /// <summary>
        /// Saves the ignored paths asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SaveIgnoredPathsAsync(IConflictResult conflictResult, string collectionName);

        /// <summary>
        /// Shoulds the ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ShouldIgnoreGameMods(IConflictResult conflictResult);

        /// <summary>
        /// Shoulds the show reset conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ShouldShowResetConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Shoulds the hide self conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ShouldShowSelfConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Toggles the ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ToggleIgnoreGameMods(IConflictResult conflictResult);

        /// <summary>
        /// Toggles the self mod conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ToggleSelfModConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Toggles the show reset conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool? ToggleShowResetConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Validates the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>IValidateResult.</returns>
        IValidateResult Validate(IDefinition definition);

        #endregion Methods
    }
}
