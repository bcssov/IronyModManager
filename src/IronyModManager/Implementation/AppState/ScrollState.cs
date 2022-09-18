// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2021
//
// Last Modified By : Mario
// Last Modified On : 09-18-2022
// ***********************************************************************
// <copyright file="ScrollState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace IronyModManager.Implementation.AppState
{
    /// <summary>
    /// Class ScrollState.
    /// Implements the <see cref="IronyModManager.Implementation.AppState.IScrollState" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.AppState.IScrollState" />
    public class ScrollState : IScrollState
    {
        #region Fields

        /// <summary>
        /// The scroll allowed
        /// </summary>
        private readonly Subject<bool> scrollAllowed;

        /// <summary>
        /// The current state
        /// </summary>
        private bool currentState;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollState" /> class.
        /// </summary>
        public ScrollState()
        {
            scrollAllowed = new Subject<bool>();
            currentState = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public IObservable<bool> State => scrollAllowed;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the state of the current.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="allowScroll">if set to <c>true</c> [allow scroll].</param>
        public void SetState(bool allowScroll)
        {
            currentState = allowScroll;
            scrollAllowed.OnNext(allowScroll);
        }

        #endregion Methods
    }
}
