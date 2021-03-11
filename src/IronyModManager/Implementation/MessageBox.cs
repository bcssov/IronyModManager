// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-22-2020
//
// Last Modified By : Mario
// Last Modified On : 03-11-2021
// ***********************************************************************
// <copyright file="MessageBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using IronyModManager.DI;
using IronyModManager.Fonts;
using IronyModManager.Implementation.Actions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.ViewModels;

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
            var font = ResolveFont();
            var parameters = new MessageBoxCustomParams
            {
                CanResize = false,
                ShowInCenter = true,
                ContentTitle = title,
                ContentHeader = header,
                ContentMessage = message,
                Icon = Icon.Error,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                FontFamily = font.GetFontFamily(),
                WindowIcon = StaticResources.GetAppIcon()
            };
            if (Dispatcher.UIThread.CheckAccess())
            {
                var window = new Controls.Themes.CustomMessageBox(parameters.Style);
                window.DataContext = new MsBoxCustomViewModel(parameters, window);
                return new FatalErrorMessageBox(window);
            }
            else
            {
                var task = Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var window = new Controls.Themes.CustomMessageBox(parameters.Style);
                    window.DataContext = new MsBoxCustomViewModel(parameters, window);
                    return new FatalErrorMessageBox(window);
                });
                Task.WaitAll(task);
                return task.Result;
            }
        }

        /// <summary>
        /// Gets the confirm cancel window.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="promptyType">Type of the prompty.</param>
        /// <returns>MsgBox.BaseWindows.IMsBoxWindow&lt;MsgBox.Enums.ButtonResult&gt;.</returns>
        public static IMsBoxWindow<ButtonResult> GetPromptWindow(string title, string header, string message, Icon icon, PromptType promptyType)
        {
            var buttonEnum = promptyType switch
            {
                PromptType.ConfirmCancel => ButtonEnum.OkCancel,
                PromptType.OK => ButtonEnum.Ok,
                _ => ButtonEnum.YesNo,
            };
            var font = ResolveFont();
            var parameters = new MessageBoxStandardParams()
            {
                CanResize = false,
                ShowInCenter = true,
                ContentTitle = title,
                ContentHeader = header,
                ContentMessage = message,
                Icon = icon,
                ButtonDefinitions = buttonEnum,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                FontFamily = font.GetFontFamily(),
                WindowIcon = StaticResources.GetAppIcon()
            };
            var window = new Controls.Themes.StandardMessageBox(parameters.Style, buttonEnum);
            window.DataContext = new MsBoxStandardViewModel(parameters, window);
            return new StandardMessageBox(window);
        }

        /// <summary>
        /// Resolves the font.
        /// </summary>
        /// <returns>IFontFamily.</returns>
        private static IFontFamily ResolveFont()
        {
            var langService = DIResolver.Get<ILanguagesService>();
            var language = langService.GetSelected();
            var fontResolver = DIResolver.Get<IFontFamilyManager>();
            var font = fontResolver.ResolveFontFamily(language.Font);
            return font;
        }

        #endregion Methods
    }
}
