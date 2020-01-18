// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
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
    /// Implements the <see cref="IronyModManager.Models.IModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    /// <seealso cref="IronyModManager.Models.IModel" />
    public interface IPreferences : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        Enums.Theme Theme { get; set; }

        #endregion Properties
    }
}
