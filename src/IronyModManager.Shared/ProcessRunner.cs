// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 10-26-2022
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var sanitizedCmd = path.Replace("\"", "\\\"");
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
        /// Runs the external process.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="waitForExit">if set to <c>true</c> [wait for exit].</param>
        public static void RunExternalProcess(string path, string args = Constants.EmptyParam, bool waitForExit = false)
        {
            if (!File.Exists(path))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(args))
            {
                var process = Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
                if (waitForExit)
                {
                    process.WaitForExit(120 * 1000);
                }
            }
            else
            {
                var process = Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    Arguments = args,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
                if (waitForExit)
                {
                    process.WaitForExit(120 * 1000);
                }
            }
        }

        /// <summary>
        /// Shells the execute.
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

        #endregion Methods
    }
}
