// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2021
//
// Last Modified By : Mario
// Last Modified On : 09-18-2022
// ***********************************************************************
// <copyright file="IScrollState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Implementation.AppState
{
    /// <summary>
    /// Interface IScrollState
    /// </summary>
    public interface IScrollState
    {
        #region Properties

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        IObservable<bool> State { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the state of the current.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool GetCurrentState();

        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="allowScroll">if set to <c>true</c> [allow scroll].</param>
        void SetState(bool allowScroll);

        #endregion Methods
    }
}
