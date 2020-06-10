// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="BaseIronyMessageBusConsumer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Class BaseIronyMessageBusConsumer.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IIronyMessageBusConsumer{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IIronyMessageBusConsumer{TMessage}" />
    public class BaseIronyMessageBusConsumer<TMessage> : IIronyMessageBusConsumer<TMessage>
    {
        #region Events

        /// <summary>
        /// Occurs when [message processed].
        /// </summary>
        public event EventHandler<TMessage> OnMessageReceived;

        #endregion Events

        #region Methods

        /// <summary>
        /// Called when [handle].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        public virtual Task OnHandle(TMessage message, string name)
        {
            RaiseEvent(message);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Invokes the event.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void RaiseEvent(TMessage message)
        {
            OnMessageReceived?.Invoke(this, message);
        }

        #endregion Methods
    }
}
