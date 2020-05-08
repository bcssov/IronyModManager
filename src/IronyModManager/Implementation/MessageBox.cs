// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-22-2020
//
// Last Modified By : Mario
// Last Modified On : 05-08-2020
// ***********************************************************************
// <copyright file="MessageBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using IronyModManager.Shared;
using MessageBox.Avalonia.DTO;
using MsgBox = MessageBox.Avalonia;

namespace IronyModManager.Implementation
{
    /// <summary>
    /// Class MessageBoxes.
    /// </summary>
    [ExcludeFromCoverage("Won't test external GUI component.")]
    public static class MessageBoxes
    {
        #region Methods

        /// <summary>
        /// Gets the fatal error window.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <returns>MsgBox.BaseWindows.IMsBoxWindow&lt;System.String&gt;.</returns>
        public static FatalErrorMessageBox GetFatalErrorWindow(string title, string header, string message)
        {
            var parameters = new MessageBoxCustomParams
            {
                CanResize = false,
                ShowInCenter = true,
                ContentTitle = title,
                ContentHeader = header,
                ContentMessage = message,
                Icon = MsgBox.Enums.Icon.Error,
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            if (Dispatcher.UIThread.CheckAccess())
            {
                var window = new MsgBox.Views.MsBoxCustomWindow(parameters.Style);
                window.Icon = StaticResources.GetAppIcon();
                parameters.Window = window;
                window.DataContext = new MsgBox.ViewModels.MsBoxCustomViewModel(parameters);
                return new FatalErrorMessageBox(window);
            }
            else
            {
                var task = Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var window = new MsgBox.Views.MsBoxCustomWindow(parameters.Style);
                    window.Icon = StaticResources.GetAppIcon();
                    parameters.Window = window;
                    window.DataContext = new MsgBox.ViewModels.MsBoxCustomViewModel(parameters);
                    return new FatalErrorMessageBox(window);
                });
                Task.WaitAll(task);
                return task.Result;
            }
        }

        /// <summary>
        /// Gets the yes no window.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>MessageBox.Avalonia.BaseWindows.MsBoxStandardWindow.</returns>
        public static MsgBox.BaseWindows.IMsBoxWindow<MsgBox.Enums.ButtonResult> GetYesNoWindow(string title, string header, string message, MsgBox.Enums.Icon icon)
        {
            var parameters = new MessageBoxStandardParams()
            {
                CanResize = false,
                ShowInCenter = true,
                ContentTitle = title,
                ContentHeader = header,
                ContentMessage = message,
                Icon = icon,
                ButtonDefinitions = MsgBox.Enums.ButtonEnum.YesNo,
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            var window = new MsgBox.Views.MsBoxStandardWindow(parameters.Style);
            parameters.Window = window;
            window.Icon = StaticResources.GetAppIcon();
            window.DataContext = new MsgBox.ViewModels.MsBoxStandardViewModel(parameters);
            return new MsgBox.BaseWindows.MsBoxStandardWindow(window);
        }

        #endregion Methods
    }
}
