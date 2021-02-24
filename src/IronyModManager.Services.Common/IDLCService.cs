// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-15-2021
// ***********************************************************************
// <copyright file="IDLCService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IDLCService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IDLCService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportAsync(IGame game, IReadOnlyCollection<IDLC> dlc);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IReadOnlyCollection&lt;IDLC&gt;&gt;.</returns>
        Task<IReadOnlyCollection<IDLC>> GetAsync(IGame game);

        /// <summary>
        /// Synchronizes the state asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SyncStateAsync(IGame game, IReadOnlyCollection<IDLC> dlc);

        #endregion Methods
    }
}
