// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-22-2020
//
// Last Modified By : Mario
// Last Modified On : 01-22-2020
// ***********************************************************************
// <copyright file="MessageBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using MessageBox.Avalonia.DTO;
using MsgBox = MessageBox.Avalonia;

namespace IronyModManager
{
    /// <summary>
    /// Class MessageBoxes.
    /// </summary>
    public static class MessageBoxes
    {
        #region Methods

        /// <summary>
        /// Gets the fatal error window.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns>MsBoxCustomWindow.</returns>
        public static MsgBox.BaseWindows.MsBoxCustomWindow GetFatalErrorWindow(string title, string message)
        {
            var messageBox = MsgBox.MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
            {
                CanResize = false,
                ShowInCenter = true,
                ContentTitle = title,
                ContentHeader = title,
                ContentMessage = message,
                Icon = MsgBox.Enums.Icon.Error
            });
            return messageBox;
        }

        /// <summary>
        /// Gets the yes no window.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="icon">The icon.</param>
        /// <returns>MessageBox.Avalonia.BaseWindows.MsBoxStandardWindow.</returns>
        public static MsgBox.BaseWindows.MsBoxStandardWindow GetYesNoWindow(string title, string message, MsgBox.Enums.Icon icon)
        {
            var msgBox = MsgBox.MessageBoxManager.GetMessageBoxStandardWindow(title, message, MsgBox.Enums.ButtonEnum.YesNo, icon);
            return msgBox;
        }

        #endregion Methods
    }
}
