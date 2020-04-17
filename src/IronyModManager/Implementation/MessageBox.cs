// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-22-2020
//
// Last Modified By : Mario
// Last Modified On : 04-17-2020
// ***********************************************************************
// <copyright file="MessageBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using Avalonia.Media.Imaging;
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
        #region Fields

        /// <summary>
        /// The icon
        /// </summary>
        private static Bitmap icon;

        #endregion Fields

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
                WindowIcon = GetIcon(),
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            var window = new MsgBox.Views.MsBoxCustomWindow(parameters.Style);
            parameters.Window = window;
            window.DataContext = new MsgBox.ViewModels.MsBoxCustomViewModel(parameters);
            return new FatalErrorMessageBox(window);
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
            // Can this messagebox not break something in an update?
            var msgBox = MsgBox.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams()
            {
                CanResize = false,
                ShowInCenter = true,
                ContentTitle = title,
                ContentHeader = header,
                ContentMessage = message,
                Icon = icon,
                ButtonDefinitions = MsgBox.Enums.ButtonEnum.YesNo,
                WindowIcon = GetIcon(),
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            });
            return msgBox;
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <returns>Bitmap.</returns>
        private static Bitmap GetIcon()
        {
            if (icon == null)
            {
                using var ms = new MemoryStream(Shared.ResourceReader.GetEmbeddedResource(Constants.Resources.LogoIco));
                // TODO: Check in a future package if this is is finally implemented? Why have a property in a parameter if it does nothing?
                icon = new Bitmap(ms);
            }
            return icon;
        }

        #endregion Methods
    }
}
