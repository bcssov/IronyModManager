// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-24-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="WindowStateService.cs" company="Mario">
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
    /// Class WindowStateService.
    /// Implements the <see cref="IronyModManager.Services.Common.IWindowStateService" />
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IWindowStateService" />
    public class WindowStateService : BaseService, IWindowStateService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowStateService" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="mapper">The mapper.</param>
        public WindowStateService(IStorageProvider storage, IMapper mapper) : base(storage, mapper)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IWindowState.</returns>
        public virtual IWindowState Get()
        {
            return StorageProvider.GetWindowState();
        }

        /// <summary>
        /// Determines whether this instance is defined.
        /// </summary>
        /// <returns><c>true</c> if this instance is defined; otherwise, <c>false</c>.</returns>
        public virtual bool IsDefined()
        {
            var state = Get();
            if (state != null)
            {
                return state.Height.HasValue && state.Width.HasValue && state.LocationX.HasValue && state.LocationY.HasValue;
            }
            return false;
        }

        /// <summary>
        /// Determines whether this instance is maximized.
        /// </summary>
        /// <returns><c>true</c> if this instance is maximized; otherwise, <c>false</c>.</returns>
        public virtual bool IsMaximized()
        {
            var state = Get();
            if (state != null)
            {
                return state.IsMaximized.GetValueOrDefault();
            }
            return false;
        }

        /// <summary>
        /// Saves the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Save(IWindowState state)
        {
            return StorageProvider.SetWindowState(state);
        }

        #endregion Methods
    }
}
