// ***********************************************************************
// Assembly         : IronyModManager.IO.Common.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="WritingStateOperationEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class WritingStateOperationEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseNonAwaitableEvent" />
    public class WritingStateOperationEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WritingStateOperationEvent" /> class.
        /// </summary>
        /// <param name="canShutdown">if set to <c>true</c> [can shutdown].</param>
        public WritingStateOperationEvent(bool canShutdown)
        {
            CanShutdown = canShutdown;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance can shutdown.
        /// </summary>
        /// <value><c>true</c> if this instance can shutdown; otherwise, <c>false</c>.</value>
        public bool CanShutdown { get; }

        #endregion Properties
    }
}
