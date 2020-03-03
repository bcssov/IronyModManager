// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-03-2020
// ***********************************************************************
// <copyright file="AppStateService.cs" company="Mario">
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
    /// Class AppStateService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IAppStateService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IAppStateService" />
    public class AppStateService : BaseService, IAppStateService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppStateService"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        public AppStateService(IStorageProvider storage, IMapper mapper) : base(storage, mapper)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IAppState.</returns>
        public IAppState Get()
        {
            return StorageProvider.GetAppState();
        }

        /// <summary>
        /// Saves the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Save(IAppState state)
        {
            return StorageProvider.SetAppState(state);
        }

        #endregion Methods
    }
}
