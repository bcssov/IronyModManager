// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="IronyMessageBus.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IronyModManager.Shared;
using SlimMessageBus;

namespace IronyModManager.DI.MessageBus
{
    /// <summary>
    /// Class IronyMessageBus.
    /// Implements the <see cref="IronyModManager.Shared.IIronyMessageBus" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.IIronyMessageBus" />
    public class IronyMessageBus : IIronyMessageBus
    {
        #region Fields

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IronyMessageBus"/> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public IronyMessageBus(IMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            messageBus?.Dispose();
        }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the t message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        public Task Publish<TMessage>(TMessage message, string name = null)
        {
            return messageBus.Publish(message, name);
        }

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <typeparam name="TResponseMessage">The type of the t response message.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TResponseMessage&gt;.</returns>
        public Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, CancellationToken cancellationToken)
        {
            return messageBus.Send(request, cancellationToken);
        }

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <typeparam name="TResponseMessage">The type of the t response message.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TResponseMessage&gt;.</returns>
        public Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, string name = null, CancellationToken cancellationToken = default)
        {
            return messageBus.Send(request, name, cancellationToken);
        }

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <typeparam name="TResponseMessage">The type of the t response message.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TResponseMessage&gt;.</returns>
        public Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, TimeSpan timeout, string name = null, CancellationToken cancellationToken = default)
        {
            return messageBus.Send(request, timeout, name, cancellationToken);
        }

        #endregion Methods
    }
}
