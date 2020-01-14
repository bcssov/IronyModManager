// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
// ***********************************************************************
// <copyright file="IThemeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using IronyModManager.Models;

namespace IronyModManager.Services
{
    /// <summary>
    /// Interface IThemeService
    /// </summary>
    public interface IThemeService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ITheme&gt;.</returns>
        IEnumerable<ITheme> Get();

        /// <summary>
        /// Saves the specified themes.
        /// </summary>
        /// <param name="themes">The themes.</param>
        void Save(IEnumerable<ITheme> themes);

        #endregion Methods
    }
}
