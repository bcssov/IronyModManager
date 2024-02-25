// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-25-2024
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="IGameLanguageService.cs" company="Mario">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// An game language service interface.
    /// </summary>
    public interface IGameLanguageService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Get.
        /// </summary>
        /// <returns>A list of IGameLanguages.<see cref="IEnumerable{IGameLanguage}" /></returns>
        IEnumerable<IGameLanguage> Get();

        /// <summary>
        /// Get selected.
        /// </summary>
        /// <returns>A read only collection of IGameLanguages.<see cref="IReadOnlyCollection{IGameLanguage}"/></returns>
        IReadOnlyCollection<IGameLanguage> GetSelected();

        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="languages">The languages.</param>
        void Save(IEnumerable<IGameLanguage> languages);

        #endregion Methods
    }
}
