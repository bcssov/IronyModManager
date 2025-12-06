// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 12-06-2025
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="LinuxFlatPakResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronyModManager.Services.Resolver
{
    /// <summary>
    /// Class LinuxFlatPakResolver.
    /// </summary>
    internal class LinuxFlatPakResolver
    {
        #region Fields

        /// <summary>
        /// The flatpak binary path
        /// </summary>
        private const string FlatpakPath = "/usr/bin/flatpak";

        /// <summary>
        /// The steam flatpak path
        /// </summary>
        private const string SteamPath = ".var/app/com.valvesoftware.Steam/.local/share/Steam";

        /// <summary>
        /// The flatpak root
        /// </summary>
        private string flatpakSteamRoot;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the flatpak steam path.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetFlatpakSteamPath()
        {
            if (string.IsNullOrWhiteSpace(flatpakSteamRoot))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                flatpakSteamRoot = Path.Combine(home, SteamPath);
            }

            return flatpakSteamRoot;
        }

        /// <summary>
        /// Determines whether flatpak binaries exist.
        /// </summary>
        /// <returns><c>true</c> if flatpak binaries exist; otherwise, <c>false</c>.</returns>
        public bool HasFlatpak()
        {
            return File.Exists(FlatpakPath);
        }

        /// <summary>
        /// Determines whether provided path is a steam flatpak install.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if path is located under flatpak steam; otherwise, <c>false</c>.</returns>
        public bool IsFlatpakSteamInstall(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && path.StartsWith(GetFlatpakSteamPath(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether flatpak has steam installation.
        /// </summary>
        /// <returns><c>true</c> if flatpak steam installation is present; otherwise, <c>false</c>.</returns>
        public bool IsSteamInstalled()
        {
            return Directory.Exists(GetFlatpakSteamPath());
        }

        #endregion Methods
    }
}
