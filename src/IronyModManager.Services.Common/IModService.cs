// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 05-26-2020
// ***********************************************************************
// <copyright file="IModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IModService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IModService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        string BuildModUrl(IMod mod);

        /// <summary>
        /// Builds the steam URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        string BuildSteamUrl(IMod mod);

        /// <summary>
        /// Deletes the descriptors asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteDescriptorsAsync(IEnumerable<IMod> mods);

        /// <summary>
        /// Exports the mods asynchronous.
        /// </summary>
        /// <param name="enabledMods">The enabled mods.</param>
        /// <param name="regularMods">The regular mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportModsAsync(IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, string collectionName);

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        IEnumerable<IMod> GetInstalledMods(IGame game);

        /// <summary>
        /// Installs the mods asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> InstallModsAsync();

        /// <summary>
        /// Locks the descriptors asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> LockDescriptorsAsync(IEnumerable<IMod> mods, bool isLocked);

        #endregion Methods
    }
}
