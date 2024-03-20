// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
// ***********************************************************************
// <copyright file="Preferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Gets or sets the color of the conflict solver deleted line.
        /// </summary>
        /// <value>The color of the conflict solver deleted line.</value>
        public virtual string ConflictSolverDeletedLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver imaginary line.
        /// </summary>
        /// <value>The color of the conflict solver imaginary line.</value>
        public virtual string ConflictSolverImaginaryLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver inserted line.
        /// </summary>
        /// <value>The color of the conflict solver inserted line.</value>
        public virtual string ConflictSolverInsertedLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value representing the conflict solver languages.<see cref="System.Collections.Generic.List{string}" />
        /// </summary>
        /// <value>The conflict solver languages.</value>
        public virtual List<string> ConflictSolverLanguages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [conflict solver languages set].
        /// </summary>
        /// <value><c>true</c> if [conflict solver languages set]; otherwise, <c>false</c>.</value>
        public virtual bool ConflictSolverLanguagesSet { get; set; }

        /// <summary>
        /// Gets or sets a value representing the conflict solver modified line color.
        /// </summary>
        /// <value>The conflict solver modified line color.</value>
        public virtual string ConflictSolverModifiedLineColor { get; set; }

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
        /// Gets or sets the last skipped version.
        /// </summary>
        /// <value>The last skipped version.</value>
        public virtual string LastSkippedVersion { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        public virtual string Locale { get; set; }

        /// <summary>
        /// Gets or sets a value representing the merge collection mod name template.
        /// </summary>
        /// <value>The merge collection mod name template.</value>
        public virtual string MergeCollectionModNameTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value representing the merged collection name template.
        /// </summary>
        /// <value>The merged collection name template.</value>
        public virtual string MergedCollectionNameTemplate { get; set; }

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
