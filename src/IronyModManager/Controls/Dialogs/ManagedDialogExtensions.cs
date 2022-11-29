// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 11-29-2022
// ***********************************************************************
// <copyright file="ManagedDialogExtensions.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileDialogExtensions. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using IronyModManager.Common;
using IronyModManager.Controls.Themes;
using IronyModManager.DI;
using IronyModManager.Platform.Fonts;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedDialogExtensions.
    /// </summary>
    [ExcludeFromCoverage("External logic.")]
    public static class ManagedDialogExtensions
    {
        #region Methods

        /// <summary>
        /// Uses the managed dialogs.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>TAppBuilder.</returns>
        public static TAppBuilder UseIronyManagedDialogs<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup(_ =>
                AvaloniaLocator.CurrentMutable.Bind<ISystemDialogImpl>().ToSingleton<ManagedDialogImpl<Window>>());
            return builder;
        }

        /// <summary>
        /// Uses the managed dialogs.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>TAppBuilder.</returns>
        public static TAppBuilder UseIronyManagedDialogs<TAppBuilder, TWindow>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new() where TWindow : Window, new()
        {
            builder.AfterSetup(_ =>
                AvaloniaLocator.CurrentMutable.Bind<ISystemDialogImpl>().ToSingleton<ManagedDialogImpl<TWindow>>());
            return builder;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ManagedDialogImpl.
        /// Implements the <see cref="Avalonia.Controls.Platform.ISystemDialogImpl" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <seealso cref="Avalonia.Controls.Platform.ISystemDialogImpl" />
        private class ManagedDialogImpl<T> : ISystemDialogImpl where T : Window, new()
        {
            #region Methods

            /// <summary>
            /// show file dialog as an asynchronous operation.
            /// </summary>
            /// <param name="dialog">The dialog.</param>
            /// <param name="parent">The parent.</param>
            /// <returns>System.String[].</returns>
            public async Task<string[]> ShowFileDialogAsync(FileDialog dialog, Window parent)
            {
                return await ShowAsync(dialog, parent);
            }

            /// <summary>
            /// show folder dialog as an asynchronous operation.
            /// </summary>
            /// <param name="dialog">The dialog.</param>
            /// <param name="parent">The parent.</param>
            /// <returns>System.String.</returns>
            public async Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, Window parent)
            {
                return (await ShowAsync(dialog, parent, new ManagedFileDialogOptions() { AllowDirectorySelection = true }))?.FirstOrDefault();
            }

            /// <summary>
            /// show as an asynchronous operation.
            /// </summary>
            /// <param name="d">The d.</param>
            /// <param name="parent">The parent.</param>
            /// <param name="options">The options.</param>
            /// <returns>System.String[].</returns>
            private async Task<string[]> ShowAsync(SystemDialog d, Window parent, ManagedFileDialogOptions options = null)
            {
                var model = new ManagedDialogViewModel((FileSystemDialog)d,
                    options ?? new ManagedFileDialogOptions());

                var langService = DIResolver.Get<ILanguagesService>();
                var language = langService.GetSelected();
                var fontResolver = DIResolver.Get<IFontFamilyManager>();
                var font = fontResolver.ResolveFontFamily(language.Font);

                var dialog = new T
                {
                    Icon = StaticResources.GetAppIcon(),
                    Content = new ManagedDialog(),
                    Title = d.Title,
                    DataContext = model,
                    SizeToContent = SizeToContent.Width,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    FontFamily = font.GetFontFamily(),
                    Height = 700
                };
                dialog.EnsureTitlebarSpacing();

                dialog.Closed += delegate { model.Cancel(); };

                string[] result = null;

                model.CompleteRequested += items =>
                {
                    result = items;
                    dialog.Close();
                };

                model.CancelRequested += dialog.Close;

                await dialog.ShowDialog<object>(parent);
                return result;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
