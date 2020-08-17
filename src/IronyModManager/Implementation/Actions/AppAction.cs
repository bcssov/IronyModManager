// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 08-17-2020
// ***********************************************************************
// <copyright file="AppAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using IronyModManager.Shared;

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
        /// <param name="text">The text.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> CopyAsync(string text)
        {
            await Application.Current.Clipboard.SetTextAsync(text);
            return true;
        }

        /// <summary>
        /// exit application as an asynchronous operation.
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
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> OpenAsync(string command)
        {
            try
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
            catch
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> RunAsync(string path, string args = Shared.Constants.EmptyParam)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                {
                    Process.Start(path);
                }
                else
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = path,
                        Arguments = args,
                        WorkingDirectory = Path.GetDirectoryName(path)
                    });
                }
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
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
