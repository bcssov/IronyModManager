// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="IDLCService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IReadOnlyCollection&lt;IDLC&gt;&gt;.</returns>
        Task<IReadOnlyCollection<IDLC>> GetAsync(IGame game);

        #endregion Methods
    }
}
