// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="UrlAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using IronyModManager.Shared;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Class UrlActions.
    /// Implements the <see cref="IronyModManager.Implementation.Actions.IUrlAction" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Actions.IUrlAction" />
    [ExcludeFromCoverage("UI Actions are tested via functional testing.")]
    public class UrlAction : IUrlAction
    {
        #region Methods

        /// <summary>
        /// Copies the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task.</returns>
        public async Task CopyAsync(string url)
        {
            await Application.Current.Clipboard.SetTextAsync(url);
        }

        // Borrowed logic from here: https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Dialogs/AboutAvaloniaDialog.xaml.cs
        /// <summary>
        /// Opens the specified URL.
        /// Borrowed logic from here: https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Dialogs/AboutAvaloniaDialog.xaml.cs
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task.</returns>
        public Task OpenAsync(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ShellExec($"xdg-open {url}");
            }
            else
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? url : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"-e {url}" : "",
                    CreateNoWindow = true,
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shells the execute.
        /// </summary>
        /// <param name="cmd">The command.</param>
        private void ShellExec(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            );
        }

        #endregion Methods
    }
}
