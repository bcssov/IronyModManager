// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="ILanguagesService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using IronyModManager.Models.Common;

/// <summary>
/// The Common namespace.
/// </summary>
namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface ILanguagesService
    /// </summary>
    public interface ILanguagesService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ILanguage&gt;.</returns>
        IEnumerable<ILanguage> Get();

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>ILanguage.</returns>
        ILanguage GetSelected();

        /// <summary>
        /// Saves the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        void Save(ILanguage language);

        /// <summary>
        /// Toggles the selected.
        /// </summary>
        void ToggleSelected();

        #endregion Methods
    }
}
