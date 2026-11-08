// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="ProcessRunner.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class ProcessRunner.
    /// </summary>
    [ExcludeFromCoverage("Process runner is excluded.")]
    public static class ProcessRunner
    {
        #region Methods

        /// <summary>
        /// Ensures the permissions.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void EnsurePermissions(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var wrappedPath = WrapPath(path);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var sanitizedCmd = wrappedPath.Replace("\"", "\\\"");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"chmod +x {sanitizedCmd}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
            }
        }

        /// <summary>
        /// Launches the external command.
        /// </summary>
        /// <param name="command">The command.</param>
        public static void LaunchExternalCommand(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var linuxCommand = $"\"{command}\"";
                ShellExec($"xdg-open {linuxCommand}");
            }
            else
            {
                using var newProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? command : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"{command}" : "",
                    CreateNoWindow = true,
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
            }
        }

        /// <summary>
        /// Launches the flatpak command.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public static void LaunchFlatpakCommand(params string[] commands)
        {
            var psi = new ProcessStartInfo { FileName = "flatpak", UseShellExecute = false };

            psi.ArgumentList.Add("run");
            foreach (var command in commands)
            {
                psi.ArgumentList.Add(command);
            }

            Process.Start(psi);
        }

        /// <summary>
        /// Runs the external process.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="waitForExit">if set to <c>true</c> [wait for exit].</param>
        public static void RunExternalProcess(string path, string args = Constants.EmptyParam, bool waitForExit = false)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The configured executable does not exist.", path);
            }

            if (string.IsNullOrWhiteSpace(args))
            {
                var process = Process.Start(new ProcessStartInfo { FileName = path, WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) });
                if (process == null)
                {
                    throw new InvalidOperationException("No process was started for the configured executable.");
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (waitForExit)
                    {
                        process!.WaitForExit(120 * 1000);
                    }
                }
                else
                {
                    if (waitForExit)
                    {
                        var now = DateTime.UtcNow;
                        while (!process!.HasExited)
                        {
                            if (DateTime.UtcNow.Subtract(now).TotalSeconds > 120)
                            {
                                process.Kill(true);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var process = Process.Start(new ProcessStartInfo { FileName = path, Arguments = args, WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) });
                if (process == null)
                {
                    throw new InvalidOperationException("No process was started for the configured executable.");
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (waitForExit)
                    {
                        process!.WaitForExit(120 * 1000);
                    }
                }
                else
                {
                    if (waitForExit)
                    {
                        var now = DateTime.UtcNow;
                        while (!process!.HasExited)
                        {
                            if (DateTime.UtcNow.Subtract(now).TotalSeconds > 120)
                            {
                                process.Kill(true);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Runs the proton process.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="baseSteamPath">The base steam path.</param>
        /// <param name="protonVersion">The proton version.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="args">The arguments.</param>
        public static void RunProtonProcess(string path, string baseSteamPath, string protonVersion, long appId, string args = Constants.EmptyParam)
        {
            RunProtonProcess(path, baseSteamPath, protonVersion, appId, null, args);
        }

        /// <summary>
        /// Runs Proton using Steam library roots supplied by the registered game model.
        /// </summary>
        public static void RunProtonProcess(string path, string baseSteamPath, string protonVersion, long appId, IEnumerable<string> steamLibraryRoots, string args)
        {
            var psi = CreateProtonStartInfo(path, baseSteamPath, protonVersion, appId, steamLibraryRoots, args);
            if (Process.Start(psi) == null)
            {
                throw new InvalidOperationException("No process was started for the resolved Proton executable.");
            }
        }

        /// <summary>
        /// Builds the direct Proton launch configuration without starting a process.
        /// </summary>
        private static ProcessStartInfo CreateProtonStartInfo(string path, string baseSteamPath, string protonVersion, long appId, IEnumerable<string> steamLibraryRoots, string args)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The configured game executable does not exist.", path);
            }

            var libraryPath = LinuxProtonResolver.GetSteamLibraryRootFromGamePath(path) ?? baseSteamPath;

            var protonPath = LinuxProtonResolver.ResolveProtonPath(baseSteamPath, protonVersion, steamLibraryRoots);
            if (string.IsNullOrWhiteSpace(protonPath))
            {
                throw new InvalidOperationException("No Proton executable was found in the Steam client or registered Steam libraries.");
            }

            // ReSharper disable StringLiteralTypo -- shut the hell up
            var compatDataPath = Path.Combine(libraryPath, "steamapps", "compatdata", appId.ToString());

            // ReSharper restore StringLiteralTypo

            var psi = new ProcessStartInfo { FileName = protonPath, WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = false };
            psi.ArgumentList.Add("run");
            psi.ArgumentList.Add(path);

            if (!string.IsNullOrEmpty(args))
            {
                foreach (var part in args.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    psi.ArgumentList.Add(part);
                }
            }

            psi.Environment["STEAM_COMPAT_DATA_PATH"] = compatDataPath;
            psi.Environment["STEAM_COMPAT_CLIENT_INSTALL_PATH"] = baseSteamPath;

            return psi;
        }

        /// <summary>
        /// Shells execute command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        private static void ShellExec(string cmd)
        {
            // Bad idea copying this from Avalonia
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = $"-c \"{escapedArgs}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        /// <summary>
        /// Wraps the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string WrapPath(string path)
        {
            return $"\"{path}\"";
        }

        #endregion Methods
    }
}
