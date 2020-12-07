// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="IPreferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

/// <summary>
/// The Models namespace.
/// </summary>
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
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        string Locale { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        string Theme { get; set; }

        #endregion Properties
    }
}
