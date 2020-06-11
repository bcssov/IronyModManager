// ***********************************************************************
// Assembly         : IronyModManager.Shared.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="IMessageBusConsumer.cs" company="Mario">
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
    public interface IMessageBusConsumer<TMessage> : IConsumer<TMessage> where TMessage : IMessageBusEvent
    {
        #region Properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        IObservable<TMessage> Message { get; }

        #endregion Properties
    }
}
