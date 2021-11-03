// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 11-03-2021
// ***********************************************************************
// <copyright file="IGameRootPathResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using IronyModManager.Models.Common;

namespace IronyModManager.Services.Resolver
{
    /// <summary>
    /// Interface IGameRootPathResolver
    /// </summary>
    public interface IGameRootPathResolver
    {
        #region Methods

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        string GetPath(IGame game);

        /// <summary>
        /// Resolves the DLC directory.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>System.String.</returns>
        string ResolveDLCDirectory(string basePath, string dlc);

        #endregion Methods
    }
}
