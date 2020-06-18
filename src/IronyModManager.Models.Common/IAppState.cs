// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 06-18-2020
// ***********************************************************************
// <copyright file="IAppState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IAppState
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IAppState : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [collection jump on position change].
        /// </summary>
        /// <value><c>true</c> if [collection jump on position change]; otherwise, <c>false</c>.</value>
        bool CollectionJumpOnPositionChange { get; set; }

        /// <summary>
        /// Gets or sets the collection mods search term.
        /// </summary>
        /// <value>The collection mods search term.</value>
        string CollectionModsSearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the collection mods selected mod.
        /// </summary>
        /// <value>The collection mods selected mod.</value>
        string CollectionModsSelectedMod { get; set; }

        /// <summary>
        /// Gets or sets the collection mods sort column.
        /// </summary>
        /// <value>The collection mods sort column.</value>
        string CollectionModsSortColumn { get; set; }

        /// <summary>
        /// Gets or sets the installed mods search term.
        /// </summary>
        /// <value>The installed mods search term.</value>
        string InstalledModsSearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the installed mods sort column.
        /// </summary>
        /// <value>The installed mods sort column.</value>
        string InstalledModsSortColumn { get; set; }

        /// <summary>
        /// Gets or sets the installed mods sort mode.
        /// </summary>
        /// <value>The installed mods sort mode.</value>
        int InstalledModsSortMode { get; set; }

        #endregion Properties
    }
}
