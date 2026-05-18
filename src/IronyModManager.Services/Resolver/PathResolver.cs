// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 09-22-2020
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="PathResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;

namespace IronyModManager.Services.Resolver
{
    /// <summary>
    /// Class PathResolver.
    /// </summary>
    internal class PathResolver
    {
        #region Fields

        /// <summary>
        /// The map
        /// </summary>
        private static readonly Dictionary<string, Environment.SpecialFolder> map = new() { { "%USER_DOCUMENTS%", Environment.SpecialFolder.MyDocuments }, { "$LINUX_DATA_HOME", Environment.SpecialFolder.LocalApplicationData } };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the proton paradox user dir.
        /// </summary>
        /// <param name="gamePath">The game path.</param>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public string GetProtonParadoxUserDir(string gamePath, long appId)
        {
            // ReSharper disable StringLiteralTypo
            var prefix = LinuxProtonResolver.GetProtonPrefix(gamePath, appId);
            return string.IsNullOrWhiteSpace(prefix) ? string.Empty : Path.Combine(prefix, "drive_c", "users", "steamuser", "Documents", "Paradox Interactive");

            // ReSharper restore StringLiteralTypo
        }

        /// <summary>
        /// Gets the user directory.
        /// </summary>
        /// <param name="protonVersion">The proton version.</param>
        /// <param name="gameDataPath">The game data path.</param>
        /// <param name="baseSteamGameDir">The base steam game dir.</param>
        /// <param name="steamAppId">The steam application identifier.</param>
        /// <returns>System.String.</returns>
        public string GetUserDirectory(string protonVersion, string gameDataPath, string baseSteamGameDir, long steamAppId)
        {
            var protonPrefix = string.Empty;
            Dictionary<string, string> overrides = null;
            if (!string.IsNullOrWhiteSpace(protonVersion))
            {
                protonPrefix = LinuxProtonResolver.GetProtonPrefix(baseSteamGameDir, steamAppId);
                overrides = GetProtonOverride(baseSteamGameDir, steamAppId);
            }

            var userDir = Parse(gameDataPath, overrides);

            if (!string.IsNullOrWhiteSpace(protonPrefix) && LinuxProtonResolver.IsProtonDrivePath(userDir))
            {
                userDir = LinuxProtonResolver.FromProtonPath(userDir, protonPrefix);
            }

            return userDir;
        }

        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="overrides">The overrides.</param>
        /// <returns>System.String.</returns>
        public string Parse(string path, IDictionary<string, string> overrides = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            var segments = path.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var newPath = new List<string>();
            foreach (var item in segments)
            {
                if (overrides != null && overrides.TryGetValue(item, out var overridePath))
                {
                    newPath.Add(overridePath);
                }
                else if (map.TryGetValue(item.ToUpperInvariant(), out var specialFolder))
                {
                    newPath.Add(Environment.GetFolderPath(specialFolder));
                }
                else
                {
                    newPath.Add(ResolveEnvironmentVariable(item));
                }
            }

            return Path.Combine(newPath.ToArray());
        }

        /// <summary>
        /// Gets the proton override.
        /// </summary>
        /// <param name="baseSteamGameDir">The base steam game dir.</param>
        /// <param name="steamAppId">The steam application identifier.</param>
        /// <returns>Dictionary{System.String, System.String}.</returns>
        private Dictionary<string, string> GetProtonOverride(string baseSteamGameDir, long steamAppId)
        {
            // ReSharper disable once StringLiteralTypo
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "%USER_DOCUMENTS%", Path.Combine(LinuxProtonResolver.GetProtonPrefix(baseSteamGameDir, steamAppId), "drive_c", "users", "steamuser", "Documents") } };
        }

        /// <summary>
        /// Resolves the environment variable.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>System.String.</returns>
        private string ResolveEnvironmentVariable(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                return variable;
            }

            if (variable.StartsWith('$'))
            {
                var name = variable.TrimStart('$');
                var value = Environment.GetEnvironmentVariable(name);
                return value ?? variable;
            }

            if (variable.StartsWith('%') && variable.EndsWith('%'))
            {
                var name = variable.Substring(1, variable.Length - 2);
                var value = Environment.GetEnvironmentVariable(name);
                return value ?? variable;
            }

            return variable;
        }

        #endregion Methods
    }
}
