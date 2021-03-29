// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-26-2021
// ***********************************************************************
// <copyright file="Preferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

/// <summary>
/// The Models namespace.
/// </summary>
namespace IronyModManager.Models
{
    /// <summary>
    /// Class Preferences.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IPreferences" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IPreferences" />
    public class Preferences : BaseModel, IPreferences
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [automatic updates].
        /// </summary>
        /// <value><c>null</c> if [automatic updates] contains no value, <c>true</c> if [automatic updates]; otherwise, <c>false</c>.</value>
        public virtual bool? AutoUpdates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [check for prerelease].
        /// </summary>
        /// <value><c>true</c> if [check for prerelease]; otherwise, <c>false</c>.</value>
        public virtual bool CheckForPrerelease { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [conflict solver prompt shown].
        /// </summary>
        /// <value><c>true</c> if [conflict solver prompt shown]; otherwise, <c>false</c>.</value>
        public virtual bool ConflictSolverPromptShown { get; set; }

        /// <summary>
        /// Gets or sets the external editor location.
        /// </summary>
        /// <value>The external editor location.</value>
        public virtual string ExternalEditorLocation { get; set; }

        /// <summary>
        /// Gets or sets the external editor parameters.
        /// </summary>
        /// <value>The external editor parameters.</value>
        public virtual string ExternalEditorParameters { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public virtual string Game { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        public virtual string Locale { get; set; }

        /// <summary>
        /// Gets or sets the notification position.
        /// </summary>
        /// <value>The notification position.</value>
        public virtual Common.NotificationPosition NotificationPosition { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public virtual string Theme { get; set; }

        #endregion Properties
    }
}
