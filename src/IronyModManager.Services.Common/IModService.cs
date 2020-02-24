// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 02-24-2020
// ***********************************************************************
// <copyright file="IModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Indexer;
using IronyModManager.Parser.Mod;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IModService
    /// </summary>
    public interface IModService
    {
        #region Methods

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IModObject&gt;.</returns>
        IEnumerable<IModObject> GetInstalledMods(IGame game);

        /// <summary>
        /// Gets the mod objects.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <returns>IIndexedDefinitions.</returns>
        IIndexedDefinitions GetModObjects(IGame game, IEnumerable<IModObject> mods);

        #endregion Methods
    }
}
