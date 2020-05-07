// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ManagedFileDialogOptions.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileDialogOptions. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedFileDialogOptions.
    /// </summary>
    [ExcludeFromCoverage("External logic.")]
    public class ManagedFileDialogOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [allow directory selection].
        /// </summary>
        /// <value><c>true</c> if [allow directory selection]; otherwise, <c>false</c>.</value>
        public bool AllowDirectorySelection { get; set; }

        #endregion Properties
    }
}
