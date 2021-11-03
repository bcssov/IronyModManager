// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 11-03-2021
// ***********************************************************************
// <copyright file="GameRootPathResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Resolver
{
    /// <summary>
    /// Class DLCPathResolver.
    /// Implements the <see cref="IronyModManager.Services.Resolver.IGameRootPathResolver" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Resolver.IGameRootPathResolver" />
    internal class GameRootPathResolver : IGameRootPathResolver
    {
        #region Fields

        /// <summary>
        /// The DLC folder
        /// </summary>
        public const string DLCFolder = "dlc";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the DLC path.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        public string GetPath(IGame game)
        {
            var path = Path.GetDirectoryName(game.ExecutableLocation);
            while (!string.IsNullOrWhiteSpace(path))
            {
                var directory = Path.Combine(path, ResolveDLCDirectory(game.DLCContainer, DLCFolder));
                if (Directory.Exists(directory))
                {
                    break;
                }
                path = Path.GetDirectoryName(path);
            }
            return path;
        }

        /// <summary>
        /// Resolves the DLC directory.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="dlc">The DLC.</param>
        /// <returns>System.String.</returns>
        public string ResolveDLCDirectory(string basePath, string dlc)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return dlc;
            }
            return Path.Combine(basePath, dlc);
        }

        #endregion Methods
    }
}
