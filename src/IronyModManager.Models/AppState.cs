// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 02-17-2024
// ***********************************************************************
// <copyright file="AppState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class AppState.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IAppState" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IAppState" />
    public class AppState : BaseModel, IAppState
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [collection jump on position change].
        /// </summary>
        /// <value><c>true</c> if [collection jump on position change]; otherwise, <c>false</c>.</value>
        public virtual bool CollectionJumpOnPositionChange { get; set; }

        /// <summary>
        /// Gets or sets the collection mods search term.
        /// </summary>
        /// <value>The collection mods search term.</value>
        public virtual string CollectionModsSearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the collection mods selected mod.
        /// </summary>
        /// <value>The collection mods selected mod.</value>
        public virtual string CollectionModsSelectedMod { get; set; }

        /// <summary>
        /// Gets or sets the collection mods sort column.
        /// </summary>
        /// <value>The collection mods sort column.</value>
        public virtual string CollectionModsSortColumn { get; set; }

        /// <summary>
        /// Gets or sets the installed mods search term.
        /// </summary>
        /// <value>The installed mods search term.</value>
        public virtual string InstalledModsSearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the installed mods sort column.
        /// </summary>
        /// <value>The installed mods sort column.</value>
        public virtual string InstalledModsSortColumn { get; set; }

        /// <summary>
        /// Gets or sets the installed mods sort mode.
        /// </summary>
        /// <value>The installed mods sort mode.</value>
        public virtual int InstalledModsSortMode { get; set; }

        /// <summary>
        /// Gets or sets a value representing the last prank check.<see cref="System.DateTime?" />
        /// </summary>
        /// <value>The last prank check.</value>
        public DateTime? LastPrankCheck { get; set; }

        /// <summary>
        /// Gets or sets the last writable check.
        /// </summary>
        /// <value>The last writable check.</value>
        public virtual DateTime? LastWritableCheck { get; set; }

        #endregion Properties
    }
}
