// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 09-06-2021
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
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using IronyModManager.DI;
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
        /// The steam apps library folder VDF
        /// </summary>
        private static readonly string SteamAppsLibraryFolderVDF = PathHelper.MergePaths(SteamAppsDirectory, "libraryfolders.vdf");

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
                try
                {
                    var vdf = LoadVDF(path);
                    if (vdf != null)
                    {
                        var model = vdf.Value.ToJson().ToObject<SteamAppManifest>();
                        return model.InstallDir.StandardizeDirectorySeparator();
                    }
                }
                catch
                {
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
                var steamAppsLibraryFolderVDF = Path.Combine(steamInstallDirectory, SteamAppsLibraryFolderVDF);
                var paths = GetVdf(vdfPath, libraryFolderVDF, steamAppsLibraryFolderVDF);
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
                var steamAppsLibraryFolderVDF = Path.Combine(steamInstallDirectory, SteamAppsLibraryFolderVDF);
                var paths = GetVdf(vdfPath, libraryFolderVDF, steamAppsLibraryFolderVDF);
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
        /// <param name="steamAppsLibraryFolderPath">The steam apps library folder path.</param>
        /// <returns>System.Collections.Generic.List&lt;string&gt;.</returns>
        private static List<string> GetVdf(string vdfPath, string libraryFolderPath, string steamAppsLibraryFolderPath)
        {
            static string ParseLibraryFolderData(VObject item)
            {
                try
                {
                    var deserialized = item.Value<VObject>().ToJson().ToObject<SteamLibraryFolderData>();
                    if (deserialized.Mounted)
                    {
                        return deserialized.Path.StandardizeDirectorySeparator();
                    }
                }
                catch
                {
                }
                return string.Empty;
            }

            if (!(cachedPaths.ContainsKey(vdfPath) || cachedPaths.ContainsKey(libraryFolderPath) || cachedPaths.ContainsKey(steamAppsLibraryFolderPath)))
            {
                if (File.Exists(vdfPath))
                {
                    var paths = new List<string>();
                    var model = LoadVDF(vdfPath);
                    if (model != null && model.Value != null)
                    {
                        var softwareKeyKey = model.Value.OfType<VProperty>().FirstOrDefault(p => p.Key == "Software");
                        if (softwareKeyKey != null)
                        {
                            var valveKey = softwareKeyKey.Value.OfType<VProperty>().FirstOrDefault(p => p.Key == "Valve");
                            if (valveKey != null)
                            {
                                var steamKey = valveKey.Value.OfType<VProperty>().FirstOrDefault(p => p.Key == "Steam");
                                if (steamKey != null)
                                {
                                    paths.AddRange(steamKey.Value.OfType<VProperty>().Where(p => p.Key.StartsWith(SteamVDFBaseInstallFolderId) && p.Value.Type == VTokenType.Value).Select(p => (p.Value as VValue).Value<string>()).Where(p => !string.IsNullOrWhiteSpace(p)));
                                }
                            }
                        }
                    }
                    cachedPaths.Add(vdfPath, paths);
                }
                var loadAppsLibraryFolder = true;
                if (File.Exists(libraryFolderPath))
                {
                    // In newer steam versions and libraries this file is present in config folder
                    loadAppsLibraryFolder = false;
                    var paths = new List<string>();
                    var model = LoadVDF(libraryFolderPath);
                    if (model != null && model.Value != null && model.Value.Children().Any())
                    {
                        paths.AddRange(model.Value.Children().OfType<VProperty>().Where(p => p.Value.Type == VTokenType.Object && (p.Value is VObject) && (p.Value as VObject).Properties().Any(k => k.Key.Equals("path"))).Select(p => ParseLibraryFolderData(p.Value as VObject)).Where(p => !string.IsNullOrWhiteSpace(p)));
                    }
                    cachedPaths.Add(libraryFolderPath, paths);
                }
                if (File.Exists(steamAppsLibraryFolderPath) && loadAppsLibraryFolder)
                {
                    var paths = new List<string>();
                    var model = LoadVDF(steamAppsLibraryFolderPath);
                    if (model != null && model.Value != null && model.Value.Children().Any())
                    {
                        paths.AddRange(model.Value.Children().OfType<VProperty>().Where(p => p.Value.Type == VTokenType.Value && p is VProperty && int.TryParse(p.Key, out var _)).Select(p => (p.Value as VValue).Value<string>()).Where(p => !string.IsNullOrWhiteSpace(p)));
                    }
                    cachedPaths.Add(steamAppsLibraryFolderPath, paths);
                }
            }
            if (cachedPaths.ContainsKey(vdfPath) || cachedPaths.ContainsKey(libraryFolderPath) || cachedPaths.ContainsKey(steamAppsLibraryFolderPath))
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
                if (cachedPaths.ContainsKey(steamAppsLibraryFolderPath))
                {
                    result.AddRange(cachedPaths[steamAppsLibraryFolderPath]);
                }
                return result.Distinct().ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// Loads the VDF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>VProperty.</returns>
        private static VProperty LoadVDF(string path)
        {
            var content = File.ReadAllText(path);
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    return VdfConvert.Deserialize(content);
                }
                catch (Exception ex)
                {
                    var logger = DIResolver.Get<ILogger>();
                    logger.Error(ex);
                }
            }
            return null;
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
