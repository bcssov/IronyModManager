// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 12-08-2025
// ***********************************************************************
// <copyright file="MessageBus.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.DI.MessageBus
{
    /// <summary>
    /// Class MessageBus.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBus" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBus" />
    internal class MessageBus : IMessageBus
    {
        #region Fields

        /// <summary>
        /// The message bus
        /// </summary>
        private SlimMessageBus.IMessageBus messageBus;

        /// <summary>
        /// The registered types
        /// </summary>
        private HashSet<Type> registeredTypes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="registeredTypes">The registered types.</param>
        public MessageBus(SlimMessageBus.IMessageBus messageBus, HashSet<Type> registeredTypes)
        {
            this.messageBus = messageBus;

            // For crying out loud this is something that SlimMessageBus should handle internally
            this.registeredTypes = registeredTypes;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            messageBus = null;
            registeredTypes = null;
        }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the t message.</typeparam>
        /// <param name="message">The message.</param>
        public void Publish<TMessage>(TMessage message) where TMessage : IMessageBusEvent
        {
            PublishAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <typeparam name="TMessage">The type of the t message.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessageBusEvent
        {
            if (!registeredTypes.Contains(typeof(TMessage)))
            {
                return Task.CompletedTask;
            }

            return messageBus.Publish(message);
        }

        #endregion Methods
    }
}
