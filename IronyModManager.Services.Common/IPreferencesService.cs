// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="IPreferencesService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using IronyModManager.Models.Common;

/// <summary>
/// The Common namespace.
/// </summary>
namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IPreferencesService
    /// </summary>
    public interface IPreferencesService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IPreferences.</returns>
        IPreferences Get();

        /// <summary>
        /// Saves the specified preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        void Save(IPreferences preferences);

        #endregion Methods
    }
}
