// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-24-2020
//
// Last Modified By : Mario
// Last Modified On : 01-24-2020
// ***********************************************************************
// <copyright file="WindowStateService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class WindowStateService.
    /// Implements the <see cref="IronyModManager.Services.Common.IWindowStateService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IWindowStateService" />
    public class WindowStateService : IWindowStateService
    {
        #region Fields

        /// <summary>
        /// The storage
        /// </summary>
        private readonly IStorageProvider storage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowStateService" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public WindowStateService(IStorageProvider storage)
        {
            this.storage = storage;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IWindowState.</returns>
        public IWindowState Get()
        {
            return storage.GetWindowState();
        }

        /// <summary>
        /// Determines whether this instance is defined.
        /// </summary>
        /// <returns><c>true</c> if this instance is defined; otherwise, <c>false</c>.</returns>
        public bool IsDefined()
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
        public bool IsMaximized()
        {
            var state = Get();
            if (state != null)
            {
                return state.IsMaximized.GetValueOrDefault();
            }
            return false;
        }

        /// <summary>
        /// Sets the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Set(IWindowState state)
        {
            return storage.SetWindowState(state);
        }

        #endregion Methods
    }
}
