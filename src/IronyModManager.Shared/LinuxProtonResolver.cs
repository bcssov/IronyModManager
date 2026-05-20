// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 12-03-2025
//
// Last Modified By : Mario
// Last Modified On : 05-20-2026
// ***********************************************************************
// <copyright file="LinuxProtonResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class LinuxProtonResolver.
    /// </summary>
    public static class LinuxProtonResolver
    {
        #region Fields

        /// <summary>
        /// The common
        /// </summary>
        private const string Common = "common";

        /// <summary>
        /// The dos devices
        /// </summary>
        private const string DosDevices = "dosdevices";

        /// <summary>
        /// The proton binary
        /// </summary>
        private const string ProtonBinary = "proton";

        /// <summary>
        /// The proton experimental
        /// </summary>
        private const string ProtonExperimental = "Experimental";

        /// <summary>
        /// The proton hotfix
        /// </summary>
        private const string ProtonHotfix = "Hotfix";

        /// <summary>
        /// The proton search pattern
        /// </summary>
        private const string ProtonSearchPattern = "Proton*";

        /// <summary>
        /// The steam apps
        /// </summary>
        private const string SteamApps = "steamapps";

        /// <summary>
        /// The proton folder match regex
        /// </summary>
        private static readonly Regex protonFolderMatchRegex = new(@"Proton\s+(\d+)(?:\.(\d+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The proton version match regex
        /// </summary>
        private static readonly Regex protonVersionMatchRegex = new(@"proton_(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion Fields

        #region Methods

        /// <summary>
        /// From proton path to native path.
        /// </summary>
        /// <param name="protonPath">The proton path.</param>
        /// <param name="protonPrefixPath">The proton prefix path.</param>
        /// <returns>System.String.</returns>
        public static string FromProtonPath(string protonPath, string protonPrefixPath)
        {
            if (!IsProtonDrivePath(protonPath) || !IsProtonPrefix(protonPrefixPath))
            {
                return protonPath;
            }

            var normalized = protonPath.Replace('\\', '/');

            var drive = char.ToLowerInvariant(normalized[0]) + ":";
            var rest = normalized[2..].TrimStart('/');

            var dosDevice = Path.Combine(protonPrefixPath, DosDevices, drive);

            FileSystemInfo info = Directory.Exists(dosDevice) ? new DirectoryInfo(dosDevice) : new FileInfo(dosDevice);

            if (!info.Exists)
            {
                return protonPath;
            }

            var driveTarget = info.ResolveLinkTarget(true);

            return driveTarget != null ? Path.GetFullPath(Path.Combine(driveTarget.FullName, rest)) : protonPath;
        }

        /// <summary>
        /// Gets the proton prefix.
        /// </summary>
        /// <param name="gamePath">The game path.</param>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.String.</returns>
        public static string GetProtonPrefix(string gamePath, long appId)
        {
            // ReSharper disable StringLiteralTypo
            var libraryPath = GetSteamLibraryRootFromGamePath(gamePath);
            return string.IsNullOrWhiteSpace(libraryPath) ? string.Empty : Path.Combine(libraryPath, "steamapps", "compatdata", appId.ToString(), "pfx");

            // ReSharper restore StringLiteralTypo
        }

        /// <summary>
        /// Gets the steam library root from game path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public static string GetSteamLibraryRootFromGamePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var fullPath = Path.GetFullPath(path);

            var index = fullPath.IndexOf(Path.DirectorySeparatorChar + SteamApps + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            return index < 0 ? null : fullPath[..index].TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Determines whether [is proton drive path] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if [is proton drive path] [the specified path]; otherwise, <c>false</c>.</returns>
        public static bool IsProtonDrivePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && path.Length >= 3 && char.IsLetter(path[0]) && path[1] == ':' && (path[2] == '/' || path[2] == '\\');
        }

        /// <summary>
        /// Resolves the proton path.
        /// </summary>
        /// <param name="steamRoot">The steam root.</param>
        /// <param name="compatValue">The compat value.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">steamRoot</exception>
        public static string ResolveProtonPath(string steamRoot, string compatValue)
        {
            if (string.IsNullOrWhiteSpace(steamRoot))
            {
                throw new ArgumentNullException(nameof(steamRoot));
            }

            var protons = FindInstalledProtons(steamRoot);
            if (protons.Count == 0)
            {
                return null;
            }

            var requestedVersion = ParseProtonVersionFromToolName(compatValue);
            if (requestedVersion != null)
            {
                var (reqMajor, reqMinor) = requestedVersion.Value;
                var matching = protons.Where(p => p.Major == reqMajor).ToList();

                if (matching.Count > 0)
                {
                    ProtonInfo chosen;
                    if (reqMinor.HasValue)
                    {
                        chosen = matching.Where(p => p.Minor == reqMinor).OrderByDescending(p => p.Minor ?? 0).FirstOrDefault() ?? matching.OrderByDescending(p => p.Minor ?? 0).FirstOrDefault();
                    }
                    else
                    {
                        chosen = matching.OrderByDescending(p => p.Minor ?? 0).FirstOrDefault();
                    }

                    if (chosen != null)
                    {
                        return chosen.ProtonScriptPath;
                    }
                }
            }

            var experimental = protons.FirstOrDefault(p => p.IsExperimental);
            if (experimental != null)
            {
                return experimental.ProtonScriptPath;
            }

            var versioned = protons.Where(p => p.Major.HasValue).OrderByDescending(p => p.Major!.Value).ThenByDescending(p => p.Minor ?? 0).FirstOrDefault();

            if (versioned != null)
            {
                return versioned.ProtonScriptPath;
            }

            var hotfix = protons.FirstOrDefault(p => p.IsHotfix);
            if (hotfix != null)
            {
                return hotfix.ProtonScriptPath;
            }

            return protons[0].ProtonScriptPath;
        }

        /// <summary>
        /// Converts the native path to proton path.
        /// </summary>
        /// <param name="nativePath">The native path.</param>
        /// <param name="protonPrefixPath">The proton prefix path.</param>
        /// <returns>System.String.</returns>
        public static string ToProtonPath(string nativePath, string protonPrefixPath)
        {
            if (string.IsNullOrWhiteSpace(nativePath) || !IsProtonPrefix(protonPrefixPath))
            {
                return nativePath;
            }

            var mappings = GetDriveMappings(protonPrefixPath);

            var normalizedNative = NormalizeNativePath(nativePath);

            foreach (var mapping in mappings)
            {
                if (normalizedNative.Equals(mapping.Target, StringComparison.Ordinal) || normalizedNative.StartsWith(mapping.Target + "/", StringComparison.Ordinal))
                {
                    var rest = normalizedNative.Length == mapping.Target.Length ? string.Empty : normalizedNative[mapping.Target.Length..].TrimStart('/');

                    return string.IsNullOrEmpty(rest) ? mapping.Drive + "/" : mapping.Drive + "/" + rest.Replace('\\', '/');
                }
            }

            return nativePath;
        }

        /// <summary>
        /// Finds the installed protons.
        /// </summary>
        /// <param name="steamRoot">The steam root.</param>
        /// <returns>List&lt;ProtonInfo&gt;.</returns>
        private static List<ProtonInfo> FindInstalledProtons(string steamRoot)
        {
            var result = new List<ProtonInfo>();

            var common = Path.Combine(steamRoot, SteamApps, Common);
            if (!Directory.Exists(common))
                return result;

            foreach (var dir in Directory.GetDirectories(common, ProtonSearchPattern))
            {
                var script = Path.Combine(dir, ProtonBinary);
                if (!File.Exists(script))
                    continue;

                var name = Path.GetFileName(dir);

                var isExperimental = name.Contains(ProtonExperimental, StringComparison.OrdinalIgnoreCase);
                var isHotfix = name.Contains(ProtonHotfix, StringComparison.OrdinalIgnoreCase);

                int? major = null;
                int? minor = null;

                var m = protonFolderMatchRegex.Match(name);

                if (m.Success)
                {
                    if (int.TryParse(m.Groups[1].Value, out var maj))
                    {
                        major = maj;
                    }

                    if (m.Groups[2].Success && int.TryParse(m.Groups[2].Value, out var min))
                    {
                        minor = min;
                    }
                }

                result.Add(new ProtonInfo(script, major, minor, isExperimental, isHotfix));
            }

            return result;
        }

        /// <summary>
        /// Gets the drive mappings.
        /// </summary>
        /// <param name="protonPrefixPath">The proton prefix path.</param>
        /// <returns>IReadOnlyList{DriveMapping}.</returns>
        private static IReadOnlyList<DriveMapping> GetDriveMappings(string protonPrefixPath)
        {
            var dosDevices = Path.Combine(protonPrefixPath, DosDevices);

            return Directory.EnumerateFileSystemEntries(dosDevices)
                .Select(entry =>
                {
                    var name = Path.GetFileName(entry);

                    if (name.Length != 2 || name[1] != ':' || !char.IsLetter(name[0]))
                    {
                        return null;
                    }

                    FileSystemInfo info = Directory.Exists(entry) ? new DirectoryInfo(entry) : new FileInfo(entry);

                    if (!info.Exists)
                    {
                        return null;
                    }

                    var target = info.ResolveLinkTarget(true);
                    if (target == null)
                    {
                        return null;
                    }

                    return new DriveMapping(char.ToUpperInvariant(name[0]) + ":", NormalizeNativePath(target.FullName));
                })
                .Where(x => x != null)
                .OrderByDescending(x => x!.Target.Length)
                .ToList()!;
        }

        /// <summary>
        /// Determines whether [is proton prefix] [the specified proton prefix path].
        /// </summary>
        /// <param name="protonPrefixPath">The proton prefix path.</param>
        /// <returns><c>true</c> if [is proton prefix] [the specified proton prefix path]; otherwise, <c>false</c>.</returns>
        private static bool IsProtonPrefix(string protonPrefixPath)
        {
            if (string.IsNullOrWhiteSpace(protonPrefixPath))
            {
                return false;
            }

            var dosDevices = Path.Combine(protonPrefixPath, DosDevices);

            return Directory.Exists(dosDevices);
        }

        /// <summary>
        /// Normalizes the native path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string NormalizeNativePath(string path)
        {
            return Path.GetFullPath(path).Replace('\\', '/').TrimEnd('/');
        }

        /// <summary>
        /// Class DriveMapping. This class cannot be inherited.
        /// </summary>
        private sealed record DriveMapping(string Drive, string Target);

        /// <summary>
        /// Parses the name of the proton version from tool.
        /// </summary>
        /// <param name="toolName">Name of the tool.</param>
        /// <returns>System.Nullable&lt;System.ValueTuple&lt;System.Int32, System.Nullable&lt;System.Int32&gt;&gt;&gt;.</returns>
        private static (int Major, int? Minor)? ParseProtonVersionFromToolName(string toolName)
        {
            if (string.IsNullOrWhiteSpace(toolName))
            {
                return null;
            }

            var m = protonVersionMatchRegex.Match(toolName);
            if (!m.Success)
            {
                return null;
            }

            var digits = m.Groups[1].Value;

            if (!int.TryParse(digits, out _))
            {
                return null;
            }

            switch (digits.Length)
            {
                case 1:
                {
                    var major = digits[0] - '0';
                    return (major, 0);
                }
                case 2 when digits[0] == '1':
                {
                    var major = 10 + (digits[1] - '0');
                    return (major, 0);
                }
                case 2:
                {
                    var majorSimple = digits[0] - '0';
                    var minorSimple = digits[1] - '0';
                    return (majorSimple, minorSimple);
                }
                case 3:
                {
                    if (digits.StartsWith("10", StringComparison.Ordinal) || digits.StartsWith("11", StringComparison.Ordinal))
                    {
                        var majorPart = digits[..2];
                        var minorPart = digits[2..];

                        if (int.TryParse(majorPart, out var maj) && int.TryParse(minorPart, out var min))
                        {
                            return (maj, min);
                        }
                    }

                    var major = digits[0] - '0';
                    if (int.TryParse(digits.AsSpan(1), out var minor))
                    {
                        return (major, minor);
                    }

                    return (major, null);
                }
            }

            var majorStr = digits[..^2];
            var minorStr = digits[^2..];

            if (int.TryParse(majorStr, out var majorN) && int.TryParse(minorStr, out var minorN))
            {
                return (majorN, minorN);
            }

            return null;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ProtonInfo.
        /// </summary>
        private class ProtonInfo(string protonScriptPath, int? major, int? minor, bool isExperimental, bool isHotfix)
        {
            #region Properties

            /// <summary>
            /// Gets a value indicating whether this instance is experimental.
            /// </summary>
            /// <value><c>true</c> if this instance is experimental; otherwise, <c>false</c>.</value>
            public bool IsExperimental { get; } = isExperimental;

            /// <summary>
            /// Gets a value indicating whether this instance is hotfix.
            /// </summary>
            /// <value><c>true</c> if this instance is hotfix; otherwise, <c>false</c>.</value>
            public bool IsHotfix { get; } = isHotfix;

            /// <summary>
            /// Gets the major.
            /// </summary>
            /// <value>The major.</value>
            public int? Major { get; } = major;

            /// <summary>
            /// Gets the minor.
            /// </summary>
            /// <value>The minor.</value>
            public int? Minor { get; } = minor;

            /// <summary>
            /// Gets the proton script path.
            /// </summary>
            /// <value>The proton script path.</value>
            public string ProtonScriptPath { get; } = protonScriptPath;

            #endregion Properties
        }

        #endregion Classes
    }
}
