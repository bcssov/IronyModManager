// ***********************************************************************
// Assembly         : IronyModManager.Storage.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-04-2020
// ***********************************************************************
// <copyright file="IDatabase.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Interface IDatabase
    /// Implements the <see cref="IronyModManager.Shared.IPropertyChangedModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.IPropertyChangedModel" />
    public interface IDatabase : IPropertyChangedModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the state of the application.
        /// </summary>
        /// <value>The state of the application.</value>
        IAppState AppState { get; set; }

        /// <summary>
        /// Gets or sets the games.
        /// </summary>
        /// <value>The games.</value>
        IList<IGameType> Games { get; set; }

        /// <summary>
        /// Gets or sets the mod collection.
        /// </summary>
        /// <value>The mod collection.</value>
        IEnumerable<IModCollection> ModCollection { get; set; }

        /// <summary>
        /// Gets or sets the preferences.
        /// </summary>
        /// <value>The preferences.</value>
        IPreferences Preferences { get; set; }

        /// <summary>
        /// Gets or sets the themes.
        /// </summary>
        /// <value>The themes.</value>
        IList<IThemeType> Themes { get; set; }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        IWindowState WindowState { get; set; }

        #endregion Properties
    }
}
