// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-28-2020
//
// Last Modified By : Mario
// Last Modified On : 07-01-2020
// ***********************************************************************
// <copyright file="ShutdownState.cs" company="Mario">
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
    /// Class ShutdownState.
    /// Implements the <see cref="IronyModManager.Implementation.AppState.IShutDownState" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.AppState.IShutDownState" />
    public class ShutdownState : IShutDownState
    {
        #region Fields

        /// <summary>
        /// The can shutdown
        /// </summary>
        private volatile bool canShutdown;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShutdownState" /> class.
        /// </summary>
        public ShutdownState()
        {
            canShutdown = true;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Toggles the specified new state.
        /// </summary>
        /// <param name="newState">if set to <c>true</c> [new state].</param>
        public void Toggle(bool newState)
        {
            canShutdown = newState;
        }

        /// <summary>
        /// Waits the until free.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task WaitUntilFree()
        {
            await Task.Run(() =>
            {
                while (!canShutdown)
                {
                }
            });
        }

        #endregion Methods
    }
}
