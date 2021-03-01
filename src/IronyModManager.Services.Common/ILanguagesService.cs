// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2021
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
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface ILanguagesService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Applies the selected.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ApplySelected();

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ILanguage&gt;.</returns>
        IEnumerable<ILanguage> Get();

        /// <summary>
        /// Gets the language by supported name block.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>ILanguage.</returns>
        ILanguage GetLanguageBySupportedNameBlock(string text);

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>ILanguage.</returns>
        ILanguage GetSelected();

        /// <summary>
        /// Saves the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(ILanguage language);

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="selectedLanguage">The selected language.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetSelected(IEnumerable<ILanguage> languages, ILanguage selectedLanguage);

        #endregion Methods
    }
}
