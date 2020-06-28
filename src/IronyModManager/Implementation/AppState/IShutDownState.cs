// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-28-2020
//
// Last Modified By : Mario
// Last Modified On : 06-28-2020
// ***********************************************************************
// <copyright file="IShutDownState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.AppState
{
    /// <summary>
    /// Interface IShutDownState
    /// </summary>
    public interface IShutDownState
    {
        #region Methods

        /// <summary>
        /// Toggles the specified can shutdown.
        /// </summary>
        /// <param name="canShutdown">if set to <c>true</c> [can shutdown].</param>
        void Toggle(bool canShutdown);

        /// <summary>
        /// Waits the until free.
        /// </summary>
        /// <returns>Task.</returns>
        Task WaitUntilFree();

        #endregion Methods
    }
}
