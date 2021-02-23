// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-22-2021
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="BaseAwaitableEvent.cs" company="Mario">
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
    /// Class BaseAwaitableEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public abstract class BaseAwaitableEvent : IMessageBusEvent
    {
        #region Fields

        /// <summary>
        /// The object lock
        /// </summary>
        private readonly object objectLock = new { };

        /// <summary>
        /// The tasks completed
        /// </summary>
        private int tasksCompleted;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsAwaitable => true;

        /// <summary>
        /// Gets the tasks completed.
        /// </summary>
        /// <value>The tasks completed.</value>
        public int TasksCompleted => tasksCompleted;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Finalizes the await.
        /// </summary>
        public void FinalizeAwait()
        {
            lock (objectLock)
            {
                tasksCompleted++;
            }
        }

        #endregion Methods
    }
}
