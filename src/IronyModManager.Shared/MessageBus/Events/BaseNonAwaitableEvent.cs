// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-22-2021
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="BaseNonAwaitableEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.MessageBus.Events
{
    /// <summary>
    /// Class BaseNonAwaitableEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public abstract class BaseNonAwaitableEvent : IMessageBusEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [end await].
        /// </summary>
        /// <value><c>true</c> if [end await]; otherwise, <c>false</c>.</value>
        public bool EndAwait { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsAwaitable => false;

        #endregion Properties
    }
}
