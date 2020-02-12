// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="PreferencesService.cs" company="IronyModManager.Services">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using AutoMapper;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class PreferencesService.
    /// Implements the <see cref="IronyModManager.Services.Common.IPreferencesService" />
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IPreferencesService" />
    public class PreferencesService : BaseService, IPreferencesService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreferencesService" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        public PreferencesService(IStorageProvider storage, IMapper mapper) : base(storage, mapper)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IPreferences.</returns>
        public virtual IPreferences Get()
        {
            return StorageProvider.GetPreferences();
        }

        /// <summary>
        /// Saves the specified preferences.
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Save(IPreferences preferences)
        {
            return StorageProvider.SetPreferences(preferences);
        }

        #endregion Methods
    }
}
