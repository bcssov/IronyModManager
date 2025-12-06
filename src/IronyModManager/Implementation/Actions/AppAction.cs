// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="AppAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// <remarks>Initializes a new instance of the <see cref="AppAction" /> class.</remarks>
    [ExcludeFromCoverage("UI Actions are tested via functional testing.")]
    public class AppAction(ILogger logger) : IAppAction
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        #endregion Fields

        #region Methods

        /// <summary>
        /// copy as an asynchronous operation.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> CopyAsync(string text)
        {
            await Application.Current!.Clipboard!.SetTextAsync(text);
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
                ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime)!.Shutdown();
            });
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public Task<string> GetAsync()
        {
            return Application.Current!.Clipboard!.GetTextAsync();
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
                ProcessRunner.LaunchExternalCommand(command);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Executes flatpak command.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> OpenFlatpakAsync(params string[] commands)
        {
            try
            {
                ProcessRunner.LaunchFlatpakCommand(commands);
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
                ProcessRunner.RunExternalProcess(path, args);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Runs the game asynchronous.
        /// </summary>
        /// <param name="createSteamFile">if set to <c>true</c> [create steam file].</param>
        /// <param name="path">The path.</param>
        /// <param name="steamRoot">The steam root.</param>
        /// <param name="steamProtonVersion">The steam proton version.</param>
        /// <param name="appId">The appid.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> RunGameAsync(bool createSteamFile, string path, string steamRoot, string steamProtonVersion, int appId, string args = Shared.Constants.EmptyParam)
        {
            try
            {
                TryCreateSteamAppIdFile(path, appId);

                // Proton test
                if (path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    ProcessRunner.RunProtonProcess(path, steamRoot, steamProtonVersion, appId, args);
                    return Task.FromResult(true);
                }
                else
                {
                    return RunAsync(path, args);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Tries to create steam application identifier file.
        /// </summary>
        /// <param name="gameBinary">The game binary.</param>
        /// <param name="appId">The application identifier.</param>
        private void TryCreateSteamAppIdFile(string gameBinary, long appId)
        {
            try
            {
                var path = Path.Combine(Path.GetDirectoryName(gameBinary)!, "steam_appid.txt");

                if (File.Exists(path))
                {
                    return;
                }

                var contents = appId + Environment.NewLine;
                File.WriteAllText(path, contents);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to create steam_appid.txt in game directory");
            }
        }

        #endregion Methods
    }
}
