// ***********************************************************************
// Assembly         : IronyModManager.Shared.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="IMessageBusConsumer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlimMessageBus;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Interface IIronyMessageBusConsumer
    /// Implements the <see cref="SlimMessageBus.IConsumer{TMessage}" />
    /// Implements the <see cref="SlimMessageBus.IConsumer`1" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="SlimMessageBus.IConsumer`1" />
    /// <seealso cref="SlimMessageBus.IConsumer{TMessage}" />
    public interface IMessageBusConsumer<TMessage> : IConsumer<TMessage> where TMessage : IMessageBusEvent
    {
        #region Methods

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        IDisposable Subscribe(Action<TMessage> action);

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        IDisposable Subscribe(Func<TMessage, Task> action);

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        IDisposable Subscribe<T>(Func<TMessage, Task<T>> action);

        #endregion Methods
    }
}
