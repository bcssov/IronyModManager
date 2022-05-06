// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 02-15-2022
// ***********************************************************************
// <copyright file="AppAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AppAction(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

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

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public Task<string> GetAsync()
        {
            return Application.Current.Clipboard.GetTextAsync();
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
                    var linuxCommand = $"\"{command}\"";
                    ShellExec($"xdg-open {linuxCommand}");
                }
                else
                {
                    using var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? command : "open",
                        Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"{command}" : "",
                        CreateNoWindow = true,
                        UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    });
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Shells the execute.
        /// </summary>
        /// <param name="cmd">The command.</param>
        private void ShellExec(string cmd)
        {
            // Bad idea copying this from Avalonia
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            );
        }

        #endregion Methods
    }
}
