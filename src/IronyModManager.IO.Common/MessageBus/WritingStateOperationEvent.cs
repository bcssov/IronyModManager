// ***********************************************************************
// Assembly         : IronyModManager.IO.Common.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="WritingStateOperationEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class WritingStateOperationEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public class WritingStateOperationEvent : IMessageBusEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance can shutdown.
        /// </summary>
        /// <value><c>true</c> if this instance can shutdown; otherwise, <c>false</c>.</value>
        public bool CanShutdown { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsFireAndForget => true;

        #endregion Properties
    }
}
