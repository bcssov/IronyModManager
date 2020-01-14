// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
// ***********************************************************************
// <copyright file="PreferencesService.cs" company="IronyModManager.Services">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models;
using IronyModManager.Storage;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class PreferencesService.
    /// Implements the <see cref="IronyModManager.Services.IPreferencesService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.IPreferencesService" />
    public class PreferencesService : IPreferencesService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreferencesService" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public PreferencesService(IStorageProvider storage)
        {
            Storage = storage;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        protected IStorageProvider Storage { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IPreferences.</returns>
        public virtual IPreferences Get()
        {
            return Storage.GetPreferences();
        }

        /// <summary>
        /// Saves the specified preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        public virtual void Save(IPreferences preferences)
        {
            Storage.SetPreferences(preferences);
        }

        #endregion Methods
    }
}
