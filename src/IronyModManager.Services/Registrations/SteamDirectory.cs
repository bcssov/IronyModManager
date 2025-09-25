
// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 09-25-2025
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
using Microsoft.Win32;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using IronyModManager.DI;
using IronyModManager.Services.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Configuration;

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
        private static readonly string libraryFolderVDF = PathHelper.MergePaths("config", "libraryfolders.vdf");

        /// <summary>
        /// The steam apps library folder VDF
        /// </summary>
        private static readonly string steamAppsLibraryFolderVDF = PathHelper.MergePaths(SteamAppsDirectory, "libraryfolders.vdf");

        /// <summary>
        /// The steam common directory
        /// </summary>
        private static readonly string steamCommonDirectory = PathHelper.MergePaths(SteamAppsDirectory, "common");

        /// <summary>
        /// The steam configuration VDF
        /// </summary>
        private static readonly string steamConfigVDF = PathHelper.MergePaths("config", "config.vdf");

        /// <summary>
        /// The steam user data directory
        /// </summary>
        private static readonly string steamUserDataDirectory = "userdata";

        /// <summary>
        /// The steam workshop directory
        /// </summary>
        private static readonly string steamWorkshopDirectory = PathHelper.MergePaths(SteamAppsDirectory, "workshop", "content");

        /// <summary>
        /// The VDF serializer settings
        /// </summary>
        private static readonly VdfSerializerSettings vdfSerializerSettings = new() { MaximumTokenSize = 8192 * 8, UsesEscapeSequences = true, UsesConditionals = true };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the game directory.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="rootPathOnly">if set to <c>true</c> returns root path only.</param>
        /// <returns>System.String.</returns>
        public static string GetGameDirectory(int appId, bool rootPathOnly = false)
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
                    if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(Path.Combine(steamInstallDirectory, steamCommonDirectory, path)))
                    {
                        if (rootPathOnly)
                        {
                            return Path.Combine(steamInstallDirectory, SteamAppsDirectory);
                        }

                        return Path.Combine(steamInstallDirectory, steamCommonDirectory, path);
                    }
                }

                var vdfPath = Path.Combine(steamInstallDirectory, steamConfigVDF);
                var libraryFolderVDF = Path.Combine(steamInstallDirectory, SteamDirectory.libraryFolderVDF);
                var steamAppsLibraryFolderVDF = Path.Combine(steamInstallDirectory, SteamDirectory.steamAppsLibraryFolderVDF);
                var paths = GetVdf(vdfPath, libraryFolderVDF, steamAppsLibraryFolderVDF);
                foreach (var path in paths)
                {
                    if (File.Exists(Path.Combine(path, SteamAppsDirectory, acfFile)))
                    {
                        var installDir = findInstallDirectory(Path.Combine(path, SteamAppsDirectory, acfFile));
                        if (!string.IsNullOrWhiteSpace(installDir) && Directory.Exists(Path.Combine(path, steamCommonDirectory, installDir)))
                        {
                            if (rootPathOnly)
                            {
                                return Path.Combine(path, SteamAppsDirectory);
                            }

                            return Path.Combine(path, steamCommonDirectory, installDir);
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
            var path = Path.Combine(steamInstallDirectory, steamUserDataDirectory);
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
        /// <returns>System.Collections.Generic.IReadOnlyCollection&lt;string&gt;.</returns>
        public static IReadOnlyCollection<string> GetWorkshopDirectory(int appId)
        {
            var gameDirectory = GetGameDirectory(appId, true).ToLowerInvariant().StandardizeDirectorySeparator();
            var steamInstallDirectory = GetSteamRootPath().StandardizeDirectorySeparator();
            var result = new List<WorkshopDirectoryContainer>();
            if (Directory.Exists(steamInstallDirectory))
            {
                if (Directory.Exists(Path.Combine(steamInstallDirectory, steamWorkshopDirectory, appId.ToString())))
                {
                    result.Add(new WorkshopDirectoryContainer(Path.Combine(steamInstallDirectory, steamWorkshopDirectory, appId.ToString()).StandardizeDirectorySeparator()));
                }

                var vdfPath = Path.Combine(steamInstallDirectory, steamConfigVDF);
                var libraryFolderVDF = Path.Combine(steamInstallDirectory, SteamDirectory.libraryFolderVDF);
                var steamAppsLibraryFolderVDF = Path.Combine(steamInstallDirectory, SteamDirectory.steamAppsLibraryFolderVDF);
                var paths = GetVdf(vdfPath, libraryFolderVDF, steamAppsLibraryFolderVDF);
                foreach (var path in paths)
                {
                    if (Directory.Exists(Path.Combine(path, steamWorkshopDirectory, appId.ToString())))
                    {
                        result.Add(new WorkshopDirectoryContainer(Path.Combine(path, steamWorkshopDirectory, appId.ToString()).StandardizeDirectorySeparator()));
                    }
                }
            }

            return result.GroupBy(p => p.PathCI).Select(p => p.FirstOrDefault()).OrderBy(p => p!.PathCI.StartsWith(gameDirectory) ? 0 : 1).Select(p => p.Path).ToList();
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
            // We're going to use an override if set
            var options = DIResolver.Get<IDomainConfiguration>().GetOptions();
            if (!string.IsNullOrWhiteSpace(options.Steam.InstallLocationOverride) && Directory.Exists(options.Steam.InstallLocationOverride))
            {
                return options.Steam.InstallLocationOverride;
            }

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
            var key = ReadSteamWindowsRegistryKey(RegistryHive.CurrentUser);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = ReadSteamWindowsRegistryKey(RegistryHive.LocalMachine);
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
                    if (model is { Value: not null })
                    {
                        var softwareKeyKey = model.Value.OfType<VProperty>().FirstOrDefault(p => p.Key == "Software");
                        var valveKey = softwareKeyKey?.Value.OfType<VProperty>().FirstOrDefault(p => p.Key == "Valve");
                        var steamKey = valveKey?.Value.OfType<VProperty>().FirstOrDefault(p => p.Key == "Steam");
                        if (steamKey != null)
                        {
                            paths.AddRange(steamKey.Value.OfType<VProperty>().Where(p => p.Key.StartsWith(SteamVDFBaseInstallFolderId) && p.Value.Type == VTokenType.Value).Select(p => (p.Value as VValue)!.Value<string>())
                                .Where(p => !string.IsNullOrWhiteSpace(p)));
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
                    if (model is { Value: not null } && model.Value.Children().Any())
                    {
                        paths.AddRange(model.Value.Children().OfType<VProperty>().Where(p => p.Value.Type == VTokenType.Object && p.Value is VObject vObject && vObject.Properties().Any(k => k.Key.Equals("path")))
                            .Select(p => ParseLibraryFolderData(p.Value as VObject)).Where(p => !string.IsNullOrWhiteSpace(p)));
                    }

                    cachedPaths.Add(libraryFolderPath, paths);
                }

                if (File.Exists(steamAppsLibraryFolderPath) && loadAppsLibraryFolder)
                {
                    var paths = new List<string>();
                    var model = LoadVDF(steamAppsLibraryFolderPath);
                    if (model is { Value: not null } && model.Value.Children().Any())
                    {
                        paths.AddRange(model.Value.Children().OfType<VProperty>().Where(p => p.Value.Type == VTokenType.Value && p is not null && int.TryParse(p.Key, out _)).Select(p => (p.Value as VValue)!.Value<string>())
                            .Where(p => !string.IsNullOrWhiteSpace(p)));
                    }

                    cachedPaths.Add(steamAppsLibraryFolderPath, paths);
                }
            }

            if (cachedPaths.ContainsKey(vdfPath) || cachedPaths.ContainsKey(libraryFolderPath) || cachedPaths.ContainsKey(steamAppsLibraryFolderPath))
            {
                var result = new List<string>();
                if (cachedPaths.TryGetValue(vdfPath, out var cachedPath))
                {
                    result.AddRange(cachedPath);
                }

                if (cachedPaths.TryGetValue(libraryFolderPath, out var cachedLibPath))
                {
                    result.AddRange(cachedLibPath);
                }

                if (cachedPaths.TryGetValue(steamAppsLibraryFolderPath, out var cachedFolderPath))
                {
                    result.AddRange(cachedFolderPath);
                }

                return result.Distinct().ToList();
            }

            return [];
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
                    return VdfConvert.Deserialize(content, vdfSerializerSettings);
                }
                catch (Exception ex)
                {
                    var logger = DIResolver.Get<ILogger>();
                    logger.Error(new Exception($"Failed parsing {path}", ex));
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the steam windows registry key.
        /// </summary>
        /// <param name="registryHive">The registry hive.</param>
        /// <returns>System.String.</returns>
        private static string ReadSteamWindowsRegistryKey(RegistryHive registryHive)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using var key = registryHive.GetRegistryKey(SteamRegistryPath);
            var value = key?.GetValue(SteamRegistrySubKey);
            if (value != null)
                return value.ToString();
#pragma warning restore CA1416 // Validate platform compatibility
            return string.Empty;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class WorkshopDirectoryContainer.
        /// </summary>
        private class WorkshopDirectoryContainer
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WorkshopDirectoryContainer" /> class.
            /// </summary>
            /// <param name="path">The path.</param>
            public WorkshopDirectoryContainer(string path)
            {
                Path = path;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path
            {
                get;
            }

            /// <summary>
            /// Gets the path ci.
            /// </summary>
            /// <value>The path ci.</value>
            public string PathCI
            {
                get
                {
                    return Path.ToLowerInvariant();
                }
            }

            #endregion Properties
        }

        #endregion Classes
    }
}
