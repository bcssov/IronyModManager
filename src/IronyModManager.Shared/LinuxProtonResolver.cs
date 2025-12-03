// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 12-03-2025
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
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

                bool isExperimental = name.Contains(ProtonExperimental, StringComparison.OrdinalIgnoreCase);
                bool isHotfix = name.Contains(ProtonHotfix, StringComparison.OrdinalIgnoreCase);

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
                    int major = digits[0] - '0';
                    return (major, 0);
                }
                case 2 when digits[0] == '1':
                {
                    int major = 10 + (digits[1] - '0');
                    return (major, 0);
                }
                case 2:
                {
                    int majorSimple = digits[0] - '0';
                    int minorSimple = digits[1] - '0';
                    return (majorSimple, minorSimple);
                }
                case 3:
                {
                    if (digits.StartsWith("10", StringComparison.Ordinal) ||
                        digits.StartsWith("11", StringComparison.Ordinal))
                    {
                        var majorPart = digits.Substring(0, 2);
                        var minorPart = digits.Substring(2);

                        if (int.TryParse(majorPart, out var maj) &&
                            int.TryParse(minorPart, out var min))
                        {
                            return (maj, min);
                        }
                    }

                    int major = digits[0] - '0';
                    if (int.TryParse(digits.AsSpan(1), out var minor))
                    {
                        return (major, minor);
                    }

                    return (major, null);
                }
            }

            var majorStr = digits.Substring(0, digits.Length - 2);
            var minorStr = digits.Substring(digits.Length - 2);

            if (int.TryParse(majorStr, out var majorN) &&
                int.TryParse(minorStr, out var minorN))
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
        private class ProtonInfo(
            string protonScriptPath,
            int? major,
            int? minor,
            bool isExperimental,
            bool isHotfix)
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
