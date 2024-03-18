// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="IPreferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

/// <summary>
/// The Models namespace.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IPreferences
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IPreferences : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [automatic updates].
        /// </summary>
        /// <value><c>null</c> if [automatic updates] contains no value, <c>true</c> if [automatic updates]; otherwise, <c>false</c>.</value>
        bool? AutoUpdates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [check for prerelease].
        /// </summary>
        /// <value><c>true</c> if [check for prerelease]; otherwise, <c>false</c>.</value>
        bool CheckForPrerelease { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver deleted line.
        /// </summary>
        /// <value>The color of the conflict solver deleted line.</value>
        string ConflictSolverDeletedLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver imaginary line.
        /// </summary>
        /// <value>The color of the conflict solver imaginary line.</value>
        string ConflictSolverImaginaryLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver inserted line.
        /// </summary>
        /// <value>The color of the conflict solver inserted line.</value>
        string ConflictSolverInsertedLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value representing the conflict solver languages.<see cref="System.Collections.Generic.List{string}" />
        /// </summary>
        /// <value>The conflict solver languages.</value>
        List<string> ConflictSolverLanguages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the conflict solver languages set.
        /// </summary>
        /// <value><c>true</c> if conflict solver languages set; otherwise, <c>false</c>.</value>
        bool ConflictSolverLanguagesSet { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver modified line.
        /// </summary>
        /// <value>The color of the conflict solver modified line.</value>
        string ConflictSolverModifiedLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [conflict solver prompt shown].
        /// </summary>
        /// <value><c>true</c> if [conflict solver prompt shown]; otherwise, <c>false</c>.</value>
        bool ConflictSolverPromptShown { get; set; }

        /// <summary>
        /// Gets or sets the external editor location.
        /// </summary>
        /// <value>The external editor location.</value>
        string ExternalEditorLocation { get; set; }

        /// <summary>
        /// Gets or sets the external editor parameters.
        /// </summary>
        /// <value>The external editor parameters.</value>
        string ExternalEditorParameters { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        string Game { get; set; }

        /// <summary>
        /// Gets or sets the last skipped version.
        /// </summary>
        /// <value>The last skipped version.</value>
        string LastSkippedVersion { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        string Locale { get; set; }

        /// <summary>
        /// Gets or sets the notification position.
        /// </summary>
        /// <value>The notification position.</value>
        NotificationPosition NotificationPosition { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        string Theme { get; set; }

        #endregion Properties
    }
}
