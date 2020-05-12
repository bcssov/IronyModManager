// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-12-2020
//
// Last Modified By : Mario
// Last Modified On : 05-12-2020
// ***********************************************************************
// <copyright file="Helpers.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace IronyModManager
{
    /// <summary>
    /// Class Helpers.
    /// </summary>
    public static class Helpers
    {
        #region Methods

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <returns>Window.</returns>
        public static Window GetMainWindow()
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
