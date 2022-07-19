// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
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
        public static void RunExternalProcess(string path, string args = Constants.EmptyParam)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
            }
            else
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    Arguments = args,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
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
