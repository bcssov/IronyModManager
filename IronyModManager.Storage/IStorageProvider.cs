// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="IStorageProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Interface IStorageProvider
    /// </summary>
    public interface IStorageProvider
    {
        #region Methods

        /// <summary>
        /// Gets the preferences.
        /// </summary>
        /// <returns>IPreferences.</returns>
        IPreferences GetPreferences();

        /// <summary>
        /// Sets the preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        void SetPreferences(IPreferences preferences);

        #endregion Methods
    }
}
