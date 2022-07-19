// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="AppAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
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

        #endregion Methods
    }
}
