// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="IPreferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using IronyModManager.Models.Enums;

/// <summary>
/// The Models namespace.
/// </summary>
namespace IronyModManager.Models
{
    /// <summary>
    /// Interface IPreferences
    /// Implements the <see cref="IronyModManager.Models.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.IModel" />
    public interface IPreferences : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        Theme Theme { get; set; }

        #endregion Properties
    }
}
