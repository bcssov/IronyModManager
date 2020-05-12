// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-12-2020
//
// Last Modified By : Mario
// Last Modified On : 05-12-2020
// ***********************************************************************
// <copyright file="StandardMessageBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia.BaseWindows;
using MessageBox.Avalonia.Enums;

namespace IronyModManager.Implementation
{
    /// <summary>
    /// Class StandardMessageBox.
    /// Implements the <see cref="MessageBox.Avalonia.BaseWindows.IMsBoxWindow{MessageBox.Avalonia.Enums.ButtonResult}" />
    /// </summary>
    /// <seealso cref="MessageBox.Avalonia.BaseWindows.IMsBoxWindow{MessageBox.Avalonia.Enums.ButtonResult}" />
    public class StandardMessageBox : IMsBoxWindow<ButtonResult>
    {
        #region Fields

        /// <summary>
        /// The window
        /// </summary>
        private readonly Controls.Themes.StandardMessageBox window;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMessageBox" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public StandardMessageBox(Controls.Themes.StandardMessageBox window)
        {
            this.window = window;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Shows this instance.
        /// </summary>
        /// <returns>Task&lt;ButtonResult&gt;.</returns>
        public Task<ButtonResult> Show()
        {
            var tcs = new TaskCompletionSource<ButtonResult>();
            window.Closed += delegate { tcs.TrySetResult(window.ButtonResult); };
            window.Show();
            return tcs.Task;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="ownerWindow">The owner window.</param>
        /// <returns>Task&lt;ButtonResult&gt;.</returns>
        public Task<ButtonResult> ShowDialog(Window ownerWindow)
        {
            var tcs = new TaskCompletionSource<ButtonResult>();
            window.Closed += delegate { tcs.TrySetResult(window.ButtonResult); };
            window.ShowDialog(ownerWindow);
            return tcs.Task;
        }

        #endregion Methods
    }
}
