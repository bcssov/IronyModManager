// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-08-2020
// ***********************************************************************
// <copyright file="AppState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        /// Gets or sets the collection mods search term.
        /// </summary>
        /// <value>The collection mods search term.</value>
        public virtual string CollectionModsSearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the collection mods sort column.
        /// </summary>
        /// <value>The collection mods sort column.</value>
        public virtual string CollectionModsSortColumn { get; set; }

        /// <summary>
        /// Gets or sets the collection mods sort mode.
        /// </summary>
        /// <value>The collection mods sort mode.</value>
        public virtual int CollectionModsSortMode { get; set; }

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

        #endregion Properties
    }
}
