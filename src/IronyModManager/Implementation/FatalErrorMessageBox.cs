// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-17-2020
//
// Last Modified By : Mario
// Last Modified On : 04-17-2020
// ***********************************************************************
// <copyright file="FatalErrorMessageBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using IronyModManager.Shared;

namespace IronyModManager.Implementation
{
    /// <summary>
    /// Class FatalErrorMessageBox.
    /// </summary>
    [ExcludeFromCoverage("Won't test external GUI component.")]
    public class FatalErrorMessageBox
    {
        #region Fields

        /// <summary>
        /// The window
        /// </summary>
        private readonly MessageBox.Avalonia.Views.MsBoxCustomWindow window;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FatalErrorMessageBox" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public FatalErrorMessageBox(MessageBox.Avalonia.Views.MsBoxCustomWindow window)
        {
            this.window = window;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// show as an asynchronous operation.
        /// </summary>
        /// <returns>System.String.</returns>
        public async Task ShowAsync()
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                var mainWindow = GetMainWindow();
                if (mainWindow != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    await window.ShowDialog(mainWindow);
                }
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var mainWindow = GetMainWindow();
                    if (mainWindow != null)
                    {
                        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        await window.ShowDialog(mainWindow);
                    }
                });
            }
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <returns>Window.</returns>
        private Window GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime != null)
            {
                return ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            }
            return null;
        }

        #endregion Methods
    }
}
