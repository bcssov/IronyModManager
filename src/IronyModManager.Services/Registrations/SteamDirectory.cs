// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="SteamDirectory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using IronyModManager.Shared;
using Microsoft.Win32;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class SteamDirectory.
    /// </summary>
    [ExcludeFromCoverage("Helper setup static class.")]
    public static class SteamDirectory
    {
        #region Fields

        /// <summary>
        /// The acf format
        /// </summary>
        private const string ACFFormat = "appmanifest_{0}.acf";

        /// <summary>
        /// The install dir identifier
        /// </summary>
        private const string InstallDirId = "installdir";

        /// <summary>
        /// The steam apps directory
        /// </summary>
        private const string SteamAppsDirectory = "steamapps";

        /// <summary>
        /// The steam registry path
        /// </summary>
        private const string SteamRegistryPath = "Software\\Valve\\Steam";

        /// <summary>
        /// The steam registry sub key
        /// </summary>
        private const string SteamRegistrySubKey = "SteamPath";

        /// <summary>
        /// The steam VDF base install folder identifier
        /// </summary>
        private const string SteamVDFBaseInstallFolderId = "BaseInstallFolder_";

        /// <summary>
        /// The steam common directory
        /// </summary>
        private static readonly string SteamCommonDirectory = PathHelper.MergePaths(SteamAppsDirectory, "common");

        /// <summary>
        /// The steam configuration VDF
        /// </summary>
        private static readonly string SteamConfigVDF = PathHelper.MergePaths("config", "config.vdf");

        /// <summary>
        /// The steam workshop directory
        /// </summary>
        private static readonly string SteamWorkshopDirectory = PathHelper.MergePaths(SteamAppsDirectory, "workshop", "content");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the game directory.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetGameDirectory(int appId)
        {
            string findInstallDirectory(string path)
            {
                var lines = File.ReadAllLines(path);
                foreach (var item in lines)
                {
                    if (item.Contains(InstallDirId))
                    {
                        return item.Replace("\t", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries)[1].Replace("\"", string.Empty);
                    }
                }
                return string.Empty;
            }
            var steamInstallDirectory = GetSteamRootPath();
            if (Directory.Exists(steamInstallDirectory))
            {
                var acfFile = string.Format(ACFFormat, appId);
                if (File.Exists(Path.Combine(steamInstallDirectory, SteamAppsDirectory, acfFile)))
                {
                    var path = findInstallDirectory(Path.Combine(steamInstallDirectory, SteamAppsDirectory, acfFile));
                    if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(Path.Combine(steamInstallDirectory, SteamCommonDirectory, path)))
                    {
                        return Path.Combine(steamInstallDirectory, SteamCommonDirectory, path);
                    }
                }
                var vdfPath = Path.Combine(steamInstallDirectory, SteamConfigVDF);
                if (File.Exists(vdfPath))
                {
                    static string CleanPath(string path)
                    {
                        path = ReplaceMultipleCharacters(path, '\\');
                        path = ReplaceMultipleCharacters(path, '/');
                        return path;
                    }
                    static string ReplaceMultipleCharacters(string text, char delimiter)
                    {
                        return string.Join(delimiter, text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                    }
                    // I know there's a vdf parser out there but I see no need to add another dependency to look for a simple string
                    var lines = File.ReadAllLines(vdfPath);
                    foreach (var line in lines)
                    {
                        if (line.Contains(SteamVDFBaseInstallFolderId, StringComparison.OrdinalIgnoreCase))
                        {
                            var directory = CleanPath(line.Replace("\t", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries)[1].Replace("\"", string.Empty));
                            if (File.Exists(Path.Combine(steamInstallDirectory, SteamAppsDirectory, acfFile)))
                            {
                                var path = findInstallDirectory(Path.Combine(steamInstallDirectory, SteamAppsDirectory, acfFile));
                                if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(Path.Combine(steamInstallDirectory, SteamCommonDirectory, path)))
                                {
                                    return Path.Combine(steamInstallDirectory, SteamCommonDirectory, path);
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the workshop directory.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetWorkshopDirectory(int appId)
        {
            var steamInstallDirectory = GetSteamRootPath();
            if (Directory.Exists(steamInstallDirectory))
            {
                if (Directory.Exists(Path.Combine(steamInstallDirectory, SteamWorkshopDirectory, appId.ToString())))
                {
                    return Path.Combine(steamInstallDirectory, SteamWorkshopDirectory, appId.ToString());
                }
                var vdfPath = Path.Combine(steamInstallDirectory, SteamConfigVDF);
                if (File.Exists(vdfPath))
                {
                    static string CleanPath(string path)
                    {
                        path = ReplaceMultipleCharacters(path, '\\');
                        path = ReplaceMultipleCharacters(path, '/');
                        return path;
                    }
                    static string ReplaceMultipleCharacters(string text, char delimiter)
                    {
                        return string.Join(delimiter, text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));
                    }
                    // I know there's a vdf parser out there but I see no need to add another dependency to look for a simple string
                    var lines = File.ReadAllLines(vdfPath);
                    foreach (var line in lines)
                    {
                        if (line.Contains(SteamVDFBaseInstallFolderId, StringComparison.OrdinalIgnoreCase))
                        {
                            var directory = CleanPath(line.Replace("\t", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries)[1].Replace("\"", string.Empty));
                            if (Directory.Exists(Path.Combine(directory, SteamWorkshopDirectory, appId.ToString())))
                            {
                                return Path.Combine(directory, SteamWorkshopDirectory, appId.ToString());
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the steam linux root path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetSteamLinuxRootPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam");
        }

        /// <summary>
        /// Gets the steam osx root path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetSteamOSXRootPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), PathHelper.MergePaths("Library", "Application Support", "Steam"));
        }

        /// <summary>
        /// Gets the steam root path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetSteamRootPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return GetSteamOSXRootPath();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetSteamLinuxRootPath();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetSteamWindowsRootPath();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the steam windows root path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetSteamWindowsRootPath()
        {
            var key = ReadSteamWindowsRegistryKey(Registry.CurrentUser);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = ReadSteamWindowsRegistryKey(Registry.LocalMachine);
            }
            return key;
        }

        /// <summary>
        /// Reads the steam windows registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        /// <returns>System.String.</returns>
        private static string ReadSteamWindowsRegistryKey(RegistryKey registryKey)
        {
            using var key = registryKey.OpenSubKey(SteamRegistryPath);
            if (key != null)
            {
                var value = key.GetValue(SteamRegistrySubKey);
                if (value != null)
                    return value.ToString();
            }
            return string.Empty;
        }

        #endregion Methods
    }
}
