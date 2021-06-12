// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 06-12-2021
// ***********************************************************************
// <copyright file="SteamDirectory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using IronyModManager.Services.Models;
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
        /// The cached paths
        /// </summary>
        private static readonly Dictionary<string, List<string>> cachedPaths = new();

        /// <summary>
        /// The library folder VDF
        /// </summary>
        private static readonly string LibraryFolderVDF = PathHelper.MergePaths("config", "libraryfolders.vdf");

        /// <summary>
        /// The quotes regex
        /// </summary>
        private static readonly Regex quotesRegex = new("\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The steam common directory
        /// </summary>
        private static readonly string SteamCommonDirectory = PathHelper.MergePaths(SteamAppsDirectory, "common");

        /// <summary>
        /// The steam configuration VDF
        /// </summary>
        private static readonly string SteamConfigVDF = PathHelper.MergePaths("config", "config.vdf");

        /// <summary>
        /// The steam user data directory
        /// </summary>
        private static readonly string SteamUserDataDirectory = "userdata";

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
            static string findInstallDirectory(string path)
            {
                var lines = File.ReadAllLines(path);
                foreach (var item in lines)
                {
                    if (item.Contains(InstallDirId))
                    {
                        return FindPath(item);
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
                var libraryFolderVDF = Path.Combine(steamInstallDirectory, LibraryFolderVDF);
                var paths = GetVdf(vdfPath, libraryFolderVDF);
                foreach (var path in paths)
                {
                    if (File.Exists(Path.Combine(path, SteamAppsDirectory, acfFile)))
                    {
                        var installDir = findInstallDirectory(Path.Combine(path, SteamAppsDirectory, acfFile));
                        if (!string.IsNullOrWhiteSpace(installDir) && Directory.Exists(Path.Combine(path, SteamCommonDirectory, installDir)))
                        {
                            return Path.Combine(path, SteamCommonDirectory, installDir);
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the user data folders.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> GetUserDataFolders(int appId)
        {
            var userDataFolders = new List<string>();
            var steamInstallDirectory = GetSteamRootPath();
            // I always thought that steam would create this folder during installation
            var path = Path.Combine(steamInstallDirectory, SteamUserDataDirectory);
            if (Directory.Exists(path))
            {
                var folders = Directory.EnumerateDirectories(path).Where(p => Directory.Exists(Path.Combine(p, appId.ToString(), "remote"))).Select(p => Path.Combine(p, appId.ToString(), "remote"));
                if (folders.Any())
                {
                    userDataFolders.AddRange(folders);
                }
            }
            return userDataFolders;
        }

        /// <summary>
        /// Gets the workshop directory.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public static IReadOnlyCollection<string> GetWorkshopDirectory(int appId)
        {
            var steamInstallDirectory = GetSteamRootPath();
            var result = new List<string>();
            if (Directory.Exists(steamInstallDirectory))
            {
                if (Directory.Exists(Path.Combine(steamInstallDirectory, SteamWorkshopDirectory, appId.ToString())))
                {
                    result.Add(Path.Combine(steamInstallDirectory, SteamWorkshopDirectory, appId.ToString()));
                }
                var vdfPath = Path.Combine(steamInstallDirectory, SteamConfigVDF);
                var libraryFolderVDF = Path.Combine(steamInstallDirectory, LibraryFolderVDF);
                var paths = GetVdf(vdfPath, libraryFolderVDF);
                foreach (var path in paths)
                {
                    if (Directory.Exists(Path.Combine(path, SteamWorkshopDirectory, appId.ToString())))
                    {
                        result.Add(Path.Combine(path, SteamWorkshopDirectory, appId.ToString()));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the path.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        private static string FindPath(string line)
        {
            var result = quotesRegex.Matches(line.ReplaceTabs());
            if (result.Count == 2)
            {
                return result[1].Value.Replace("\"", string.Empty);
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
#pragma warning disable CA1416 // Validate platform compatibility
            // I know it's supported only on windows hence the OS detection.
            var key = ReadSteamWindowsRegistryKey(Registry.CurrentUser);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = ReadSteamWindowsRegistryKey(Registry.LocalMachine);
            }
#pragma warning restore CA1416 // Validate platform compatibility
            return key;
        }

        /// <summary>
        /// Gets the VDF.
        /// </summary>
        /// <param name="vdfPath">The VDF path.</param>
        /// <param name="libraryFolderPath">The library folder path.</param>
        /// <returns>System.Collections.Generic.List&lt;string&gt;.</returns>
        private static List<string> GetVdf(string vdfPath, string libraryFolderPath)
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

            if (!(cachedPaths.ContainsKey(vdfPath) || cachedPaths.ContainsKey(libraryFolderPath)))
            {
                if (File.Exists(vdfPath))
                {
                    var paths = new List<string>();
                    // This is now legacy code, so I don't think I need to port this over to vdf parser usage
                    var lines = File.ReadAllLines(vdfPath);
                    foreach (var line in lines)
                    {
                        if (line.Contains(SteamVDFBaseInstallFolderId, StringComparison.OrdinalIgnoreCase))
                        {
                            var directory = CleanPath(FindPath(line));
                            if (Directory.Exists(directory))
                            {
                                if (!paths.Contains(directory))
                                {
                                    paths.Add(directory);
                                }
                            }
                        }
                    }
                    cachedPaths.Add(vdfPath, paths);
                }
                if (File.Exists(libraryFolderPath))
                {
                    var paths = new List<string>();
                    var lines = File.ReadAllLines(libraryFolderPath);
                    var model = VdfConvert.Deserialize(File.ReadAllText(libraryFolderPath));
                    if (model != null && model.Value != null && model.Value.Children().Any())
                    {
                        foreach (var item in model.Value.Children().ToList())
                        {
                            if (item is VProperty property)
                            {
                                if (property.Value is VObject o)
                                {
                                    if (o.Properties().Any(p => p.Key.Contains("path")))
                                    {
                                        try
                                        {
                                            var deserialized = o.ToJson().ToObject<SteamLibraryFolderData>();
                                            if (deserialized.Mounted > 0)
                                            {
                                                if (!paths.Contains(deserialized.Path))
                                                {
                                                    paths.Add(CleanPath(deserialized.Path));
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                    cachedPaths.Add(libraryFolderPath, paths);
                }
            }
            if (cachedPaths.ContainsKey(vdfPath) || cachedPaths.ContainsKey(libraryFolderPath))
            {
                var result = new List<string>();
                if (cachedPaths.ContainsKey(vdfPath))
                {
                    result.AddRange(cachedPaths[vdfPath]);
                }
                if (cachedPaths.ContainsKey(libraryFolderPath))
                {
                    result.AddRange(cachedPaths[libraryFolderPath]);
                }
                return result;
            }
            return new List<string>();
        }

        /// <summary>
        /// Reads the steam windows registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        /// <returns>System.String.</returns>
        private static string ReadSteamWindowsRegistryKey(RegistryKey registryKey)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using var key = registryKey.OpenSubKey(SteamRegistryPath);
            if (key != null)
            {
                var value = key.GetValue(SteamRegistrySubKey);
                if (value != null)
                    return value.ToString();
            }
#pragma warning restore CA1416 // Validate platform compatibility
            return string.Empty;
        }

        #endregion Methods
    }
}
