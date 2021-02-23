// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-22-2021
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
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
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsAwaitable => false;

        /// <summary>
        /// Gets the tasks completed.
        /// </summary>
        /// <value>The tasks completed.</value>
        /// <exception cref="NotSupportedException">Use {nameof(BaseAwaitableEvent)} instead.</exception>
        public int TasksCompleted => throw new NotSupportedException($"Use {nameof(BaseAwaitableEvent)} instead.");

        #endregion Properties

        #region Methods

        /// <summary>
        /// Finalizes the await.
        /// </summary>
        /// <exception cref="NotSupportedException">Use {nameof(BaseAwaitableEvent)} instead.</exception>
        public void FinalizeAwait()
        {
            throw new NotSupportedException($"Use {nameof(BaseAwaitableEvent)} instead.");
        }

        #endregion Methods
    }
}
