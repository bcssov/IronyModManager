// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ManagedDialogNavigationItem.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileChooserNavigationItem. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Dialogs;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedDialogNavigationItem.
    /// </summary>
    [ExcludeFromCoverage("External logic.")]
    public class ManagedDialogNavigationItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        public ManagedFileChooserItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        #endregion Properties
    }
}
