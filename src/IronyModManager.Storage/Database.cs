// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-04-2020
// ***********************************************************************
// <copyright file="Database.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using Jot.Configuration.Attributes;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Database.
    /// Implements the <see cref="IronyModManager.Storage.Common.IDatabase" />
    /// Implements the <see cref="IronyModManager.Shared.PropertyChangedModelBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PropertyChangedModelBase" />
    /// <seealso cref="IronyModManager.Storage.Common.IDatabase" />
    public class Database : PropertyChangedModelBase, IDatabase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Database" /> class.
        /// </summary>
        public Database()
        {
            Themes = new List<IThemeType>();
            Games = new List<IGameType>();
            ModCollection = new List<IModCollection>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the state of the application.
        /// </summary>
        /// <value>The state of the application.</value>
        [Trackable]
        public virtual IAppState AppState { get; set; } = DIResolver.Get<IAppState>();

        /// <summary>
        /// Gets or sets the games.
        /// </summary>
        /// <value>The games.</value>
        public virtual IList<IGameType> Games { get; set; }

        /// <summary>
        /// Gets or sets the mod collection.
        /// </summary>
        /// <value>The mod collection.</value>
        [Trackable]
        public virtual IEnumerable<IModCollection> ModCollection { get; set; }

        /// <summary>
        /// Gets or sets the preferences.
        /// </summary>
        /// <value>The preferences.</value>
        [Trackable]
        public virtual IPreferences Preferences { get; set; } = DIResolver.Get<IPreferences>();

        /// <summary>
        /// Gets or sets the themes.
        /// </summary>
        /// <value>The themes.</value>
        public virtual IList<IThemeType> Themes { get; set; }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        [Trackable]
        public virtual IWindowState WindowState { get; set; } = DIResolver.Get<IWindowState>();

        #endregion Properties
    }
}
