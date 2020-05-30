// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 05-30-2020
// ***********************************************************************
// <copyright file="AppAction.cs" company="Mario">
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
using Avalonia.Controls.ApplicationLifetimes;
using IronyModManager.Shared;
using Avalonia.Threading;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Class AppAction.
    /// Implements the <see cref="IronyModManager.Implementation.Actions.IAppAction" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Actions.IAppAction" />
    [ExcludeFromCoverage("UI Actions are tested via functional testing.")]
    public class AppAction : IAppAction
    {
        #region Methods

        /// <summary>
        /// copy as an asynchronous operation.
        /// </summary>
        /// <param name="text">The URL.</param>
        /// <returns>Task.</returns>
        public async Task CopyAsync(string text)
        {
            await Application.Current.Clipboard.SetTextAsync(text);
        }

        /// <summary>
        /// Exits the application asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task ExitAppAsync()
        {
            await Task.Delay(50);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Shutdown();
            });
        }

        // Borrowed logic from here: https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Dialogs/AboutAvaloniaDialog.xaml.cs
        /// <summary>
        /// Opens the specified URL.
        /// Borrowed logic from here: https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Dialogs/AboutAvaloniaDialog.xaml.cs
        /// </summary>
        /// <param name="command">The URL.</param>
        /// <returns>Task.</returns>
        public Task OpenAsync(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ShellExec($"xdg-open {command}");
            }
            else
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? command : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"-e {command}" : "",
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
