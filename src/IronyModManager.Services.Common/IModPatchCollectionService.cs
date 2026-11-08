// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 02-09-2025
// ***********************************************************************
// <copyright file="IModPatchCollectionService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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
    [GameStateSafetyContract]
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
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Write patch collection", GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> AddCustomModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Adds the mods to ignore list.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="mods">The mods.</param>
        [GameStateSafetyExempt("Mutates in-memory conflict state only.")]
        void AddModsToIgnoreList(IConflictResult conflictResult, IEnumerable<IModIgnoreConfiguration> mods);

        /// <summary>
        /// Adds an exact participating-mod set to the ignore rules.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="modNames">The canonical mod names.</param>
        [GameStateSafetyExempt("Mutates in-memory conflict configuration only.")]
        void AddExactModSetToIgnoreList(IConflictResult conflictResult, IEnumerable<string> modNames);

        /// <summary>
        /// Gets the valid exact participating-mod sets from the ignore rules.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns>The decoded canonical mod-name sets in persisted order.</returns>
        [GameStateSafetyExempt("Reads in-memory conflict configuration only.")]
        IReadOnlyList<IReadOnlyList<string>> GetExactModSetIgnoreRules(IConflictResult conflictResult);

        /// <summary>
        /// Removes a logical exact participating-mod set from the ignore rules.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="modNames">The canonical mod names.</param>
        /// <returns><c>true</c> when at least one equivalent rule was removed; otherwise, <c>false</c>.</returns>
        [GameStateSafetyExempt("Mutates in-memory conflict configuration only.")]
        bool RemoveExactModSetIgnoreRule(IConflictResult conflictResult, IEnumerable<string> modNames);

        /// <summary>
        /// Determines whether an exact participating-mod set is already present in the ignore rules.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="modNames">The canonical mod names.</param>
        /// <returns><c>true</c> when a logically equivalent exact-set rule exists; otherwise, <c>false</c>.</returns>
        [GameStateSafetyExempt("Reads in-memory conflict configuration only.")]
        bool HasExactModSetIgnoreRule(IConflictResult conflictResult, IEnumerable<string> modNames);

        /// <summary>
        /// Applies the mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Write patch collection", GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> ApplyModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Cleans the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Clean patch collection", GameStateLockReason.WriteAccessFailure)]
        Task<bool> CleanPatchCollectionAsync(string collectionName);

        /// <summary>
        /// Copies the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="newCollectionName">New name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Copy patch collection", GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> CopyPatchCollectionAsync(string collectionName, string newCollectionName);

        /// <summary>
        /// Creates the patch definition asynchronous.
        /// </summary>
        /// <param name="copy">The copy.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IDefinition&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.Null, "Read patch state", GameStateLockReason.DiscoveryUnavailable)]
        Task<IDefinition> CreatePatchDefinitionAsync(IDefinition copy, string collectionName);

        /// <summary>
        /// Evals the definition priority.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IPriorityDefinitionResult.</returns>
        [GameStateSafetyExempt("In-memory definition evaluation.")]
        IPriorityDefinitionResult EvalDefinitionPriority(IEnumerable<IDefinition> definitions);

        /// <summary>
        /// Finds the conflicts asynchronous.
        /// </summary>
        /// <param name="indexedDefinitions">The indexed definitions.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="patchStateMode">The patch state mode.</param>
        /// <param name="allowedLanguages">The allowed languages.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        [GameStateSafetyExempt("Conflict analysis uses its existing operation-specific orchestration.")]
        Task<IConflictResult> FindConflictsAsync(IIndexedDefinitions indexedDefinitions, IList<string> modOrder, PatchStateMode patchStateMode, IReadOnlyCollection<IGameLanguage> allowedLanguages);

        /// <summary>
        /// Gets an allowed languages async.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A Task containing IReadOnlyCollection of strings.<see cref="Task{IReadOnlyCollection{string}}" /></returns>
        [GameStateSafetyExempt("Reads persisted patch metadata with method-specific fallback.")]
        Task<IReadOnlyCollection<string>> GetAllowedLanguagesAsync(string collectionName);

        /// <summary>
        /// Gets the bracket count.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns>IBracketValidateResult.</returns>
        [GameStateSafetyExempt("Pure text validation.")]
        public IBracketValidateResult GetBracketCount(string file, string text);

        /// <summary>
        /// Gets the ignored mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
        [GameStateSafetyExempt("Reads in-memory conflict configuration.")]
        IReadOnlyList<IModIgnoreConfiguration> GetIgnoredMods(IConflictResult conflictResult);

        /// <summary>
        /// Gets the mod objects asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="allowedGameLanguages">The allowed game languages.</param>
        /// <returns>Task&lt;IIndexedDefinitions&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.Null, "Read mod content for analysis", GameStateLockReason.DiscoveryUnavailable)]
        Task<IIndexedDefinitions> GetModObjectsAsync(IGame game, IEnumerable<IMod> mods, string collectionName, PatchStateMode mode, IReadOnlyCollection<IGameLanguage> allowedGameLanguages);

        /// <summary>
        /// Gets the patch state mode asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;PatchStateMode&gt;.</returns>
        [GameStateSafetyExempt("Reads persisted patch metadata with method-specific fallback.")]
        Task<PatchStateMode> GetPatchStateModeAsync(string collectionName);

        /// <summary>
        /// Ignores the mod patch asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Write patch collection", GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> IgnoreModPatchAsync(IConflictResult conflictResult, IDefinition definition, string collectionName);

        /// <summary>
        /// Initializes the patch state asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IConflictResult&gt;.</returns>
        [GameStateSafetyExempt("Owns a complex patch-state initialization and persistence sequence.")]
        Task<IConflictResult> InitializePatchStateAsync(IConflictResult conflictResult, string collectionName);

        /// <summary>
        /// Invalidates the state of the patch mod.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns><c>true</c> if invalidated, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("Invalidates application cache state only.")]
        bool InvalidatePatchModState(string collectionName);

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod].
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod]; otherwise, <c>false</c>.</returns>
        [GameStateSafetyExempt("Pure domain identification.")]
        bool IsPatchMod(IMod mod);

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod name].
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod name]; otherwise, <c>false</c>.</returns>
        [GameStateSafetyExempt("Pure domain identification.")]
        bool IsPatchMod(string modName);

        /// <summary>
        /// Loads the definition contents asynchronous.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        [GameStateSafetyExempt("Uses existing definition-loading result semantics.")]
        Task<string> LoadDefinitionContentsAsync(IDefinition definition, string collectionName);

        /// <summary>
        /// Determines if conflict result needs reload.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if conflict result needs reload, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("In-memory conflict-state evaluation.")]
        bool NeedsReload(IConflictResult conflictResult, IDefinition definition);

        /// <summary>
        /// Patches the has game definitions asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses existing patch-state inspection semantics.")]
        Task<bool> PatchHasGameDefinitionsAsync(string collectionName);

        /// <summary>
        /// Patches the mod needs update asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadOrder">The load order.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses a complex read-only patch-state comparison.")]
        Task<bool> PatchModNeedsUpdateAsync(string collectionName, IReadOnlyCollection<string> loadOrder);

        /// <summary>
        /// Renames the patch collection asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="newCollectionName">New name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Rename patch collection", GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> RenamePatchCollectionAsync(string collectionName, string newCollectionName);

        /// <summary>
        /// Resets the custom conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses the existing conflict-reset state machine.")]
        Task<bool> ResetCustomConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName);

        /// <summary>
        /// Resets the ignored conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses the existing conflict-reset state machine.")]
        Task<bool> ResetIgnoredConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName);

        /// <summary>
        /// Resets the patch state cache.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("Invalidates application cache state only.")]
        bool ResetPatchStateCache();

        /// <summary>
        /// Resets the resolved conflict asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="typeAndId">The type and identifier.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses the existing conflict-reset state machine.")]
        Task<bool> ResetResolvedConflictAsync(IConflictResult conflictResult, string typeAndId, string collectionName);

        /// <summary>
        /// Resolves the full definition path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        [GameStateSafetyExempt("Pure path resolution.")]
        string ResolveFullDefinitionPath(IDefinition definition);

        /// <summary>
        /// Saves the ignored paths asynchronous.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Save patch state", GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> SaveIgnoredPathsAsync(IConflictResult conflictResult, string collectionName);

        /// <summary>
        /// Should it ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if ignored, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("In-memory conflict-state evaluation.")]
        bool? ShouldIgnoreGameMods(IConflictResult conflictResult);

        /// <summary>
        /// Should it show reset conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it should show, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("In-memory conflict-state evaluation.")]
        bool? ShouldShowResetConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Should it hide self conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c>if it should show, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("In-memory conflict-state evaluation.")]
        bool? ShouldShowSelfConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Toggles the ignore game mods.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it is toggled, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("Mutates in-memory conflict preferences only.")]
        bool? ToggleIgnoreGameMods(IConflictResult conflictResult);

        /// <summary>
        /// Toggles the self mod conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it is toggled, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("Mutates in-memory conflict preferences only.")]
        bool? ToggleSelfModConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Toggles the show reset conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <returns><c>true</c> if it toggled, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("Mutates in-memory conflict preferences only.")]
        bool? ToggleShowResetConflicts(IConflictResult conflictResult);

        /// <summary>
        /// Validates the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>IValidateResult.</returns>
        [GameStateSafetyExempt("Pure definition validation.")]
        IValidateResult Validate(IDefinition definition);

        #endregion Methods
    }
}
