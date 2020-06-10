// ***********************************************************************
// Assembly         : IronyModManager.Shared.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="IIronyMessageBusConsumer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using SlimMessageBus;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Interface IIronyMessageBusConsumer
    /// Implements the <see cref="SlimMessageBus.IConsumer{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="SlimMessageBus.IConsumer{TMessage}" />
    public interface IIronyMessageBusConsumer<TMessage> : IConsumer<TMessage>
    {
        #region Events

        /// <summary>
        /// Occurs when [on message received].
        /// </summary>
        event EventHandler<TMessage> OnMessageReceived;

        #endregion Events
    }
}
