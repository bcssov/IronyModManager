// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 11-12-2022
// ***********************************************************************
// <copyright file="IModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IModService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    [GameStateSafetyContract]
    public interface IModService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        [GameStateSafetyExempt("Pure metadata operation.")]
        string BuildModUrl(IMod mod);

        /// <summary>
        /// Builds the steam URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        [GameStateSafetyExempt("Pure metadata operation.")]
        string BuildSteamUrl(IMod mod);

        /// <summary>
        /// Customs the mod directory empty asynchronous.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Configuration validation must remain available during controlled recovery.")]
        Task<bool> CustomModDirectoryEmptyAsync(string gameType);

        /// <summary>
        /// Deletes the descriptors asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Delete mod descriptors", GameStateLockReason.WriteAccessFailure)]
        Task<bool> DeleteDescriptorsAsync(IEnumerable<IMod> mods);

        /// <summary>
        /// Evals the achievement compatibility.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("In-memory domain evaluation.")]
        bool EvalAchievementCompatibility(IEnumerable<IMod> mods);

        /// <summary>
        /// Exports the mods asynchronous.
        /// </summary>
        /// <param name="enabledMods">The enabled mods.</param>
        /// <param name="regularMods">The regular mods.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses virtual filtering and a method-specific apply result.")]
        Task<ModApplyResult> ExportModsAsync(IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, IModCollection modCollection);

        /// <summary>
        /// Filters the mods.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="text">The text.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        [GameStateSafetyExempt("In-memory collection operation.")]
        IEnumerable<IMod> FilterMods(IEnumerable<IMod> collection, string text);

        /// <summary>
        /// Finds the mod.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="text">The text.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <param name="skipIndex">Index of the skip.</param>
        /// <returns>IMod.</returns>
        [GameStateSafetyExempt("In-memory collection operation.")]
        IMod FindMod(IEnumerable<IMod> collection, string text, bool reverse, int? skipIndex = null);

        /// <summary>
        /// Sets or clears an Irony-local display-name override for a mod.
        /// </summary>
        [GameStateSafetyExempt("User preference operation.")]
        bool SetModAlias(IMod mod, string nameOverride);

        /// <summary>
        /// Gets the available mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IEnumerable&lt;IMod&gt;&gt;.</returns>
        [GameStateSafetyExempt("Uses a method-specific known-good installed-mod fallback.")]
        Task<IEnumerable<IMod>> GetAvailableModsAsync(IGame game);

        /// <summary>
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <param name="path">The path.</param>
        /// <param name="isFromGame">if set to <c>true</c> [is from game].</param>
        /// <returns>Task&lt;MemoryStream&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.Null, "Read mod image", GameStateLockReason.DiscoveryUnavailable)]
        Task<MemoryStream> GetImageStreamAsync(string modName, string path, bool isFromGame = false);

        /// <summary>
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="path">The path.</param>
        /// <param name="isFromGame">if set to <c>true</c> [is from game].</param>
        /// <returns>Task&lt;MemoryStream&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.Null, "Read mod image", GameStateLockReason.DiscoveryUnavailable)]
        Task<MemoryStream> GetImageStreamAsync(IMod mod, string path, bool isFromGame = false);

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        [GameStateSafetyExempt("Delegates to the authoritative installed-mod refresh state machine.")]
        Task<IEnumerable<IMod>> GetInstalledModsAsync(IGame game);

        /// <summary>
        /// Refreshes installed mods and reports whether the result is authoritative.
        /// </summary>
        [GameStateSafetyExempt("Owns authoritative discovery and known-good cache state.")]
        Task<InstalledModsResult> RefreshInstalledModsAsync(IGame game);

        /// <summary>
        /// Performs an authoritative installed-mod refresh for an explicit filesystem configuration change.
        /// The game remains locked until the coordinating caller installs the result and completes revalidation.
        /// </summary>
        [GameStateSafetyExempt("Owns controlled revalidation generation and unlock ordering.")]
        Task<InstalledModsResult> RevalidateInstalledModsAsync(IGame game, GameStateLockInfo revalidationLock);

        /// <summary>
        /// Resolves a stored collection against an authoritative installed-mod set, creating virtual placeholders for missing entries.
        /// </summary>
        [GameStateSafetyExempt("In-memory collection resolution.")]
        IReadOnlyCollection<IMod> ResolveCollectionMods(IEnumerable<IMod> installedMods, IModCollection collection, IEnumerable<IMod> previousMods = null);

        /// <summary>
        /// Determines whether two mod definitions have equivalent state.
        /// </summary>
        [GameStateSafetyExempt("Pure domain comparison.")]
        bool AreModDefinitionsEquivalent(IMod mod, IMod otherMod);

        /// <summary>
        /// Determines whether two runtime mod models identify the same logical collection member.
        /// </summary>
        [GameStateSafetyExempt("Pure domain identity comparison.")]
        bool AreModIdentitiesEquivalent(IMod mod, IMod otherMod);

        /// <summary>
        /// Installs the mods asynchronous.
        /// </summary>
        /// <param name="statusToRetain">The status to retain.</param>
        /// <returns>Task&lt;IReadOnlyCollection&lt;IModInstallationResult&gt;&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.Null, "Install mod descriptors", GameStateSafetyEnforcement.EntryOnly)]
        Task<IReadOnlyCollection<IModInstallationResult>> InstallModsAsync(IEnumerable<IMod> statusToRetain);

        /// <summary>
        /// Synchronizes mod descriptors while an explicit filesystem configuration revalidation owns the game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="statusToRetain">The status to retain.</param>
        /// <param name="revalidationLock">The current configuration revalidation lock.</param>
        /// <returns><c>true</c> when synchronization completed under the supplied generation; otherwise, <c>false</c>.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False,
            "Synchronize mod descriptors during configuration revalidation",
            GameStateSafetyEnforcement.EntryOnly | GameStateSafetyEnforcement.AllowCurrentRevalidation)]
        Task<bool> InstallModsAsync(IGame game, IEnumerable<IMod> statusToRetain, GameStateLockInfo revalidationLock);

        /// <summary>
        /// Locks the descriptors asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Change descriptor lock state", GameStateLockReason.WriteAccessFailure)]
        Task<bool> LockDescriptorsAsync(IEnumerable<IMod> mods, bool isLocked);

        /// <summary>
        /// Mods the directory exists asynchronous.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Filesystem preflight remains available during controlled recovery.")]
        Task<bool> ModDirectoryExistsAsync(string folder);

        /// <summary>
        /// Patches the mod exists asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Filesystem preflight remains available during controlled recovery.")]
        Task<bool> PatchModExistsAsync(string collectionName);

        /// <summary>
        /// Populates the mod files asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafetyExempt("Uses existing per-mod file availability semantics.")]
        Task<bool> PopulateModFilesAsync(IEnumerable<IMod> mods);

        /// <summary>
        /// Purges the mod directory asynchronous.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Purge mod directory", GameStateLockReason.WriteAccessFailure)]
        Task<bool> PurgeModDirectoryAsync(string folder);

        /// <summary>
        /// Purges the mod patch asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Purge mod directory", GameStateLockReason.WriteAccessFailure)]
        Task<bool> PurgeModPatchAsync(string collectionName);

        /// <summary>
        /// Queries the contains achievements.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [GameStateSafetyExempt("Pure query evaluation.")]
        bool QueryContainsAchievements(string query);

        #endregion Methods
    }
}
