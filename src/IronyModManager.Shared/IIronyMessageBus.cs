// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="IIronyMessageBus.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using SlimMessageBus;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Interface IIronyMessageBus
    /// Implements the <see cref="SlimMessageBus.IMessageBus" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="SlimMessageBus.IMessageBus" />
    public interface IIronyMessageBus : IDisposable
    {
        #region Methods

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the t message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        Task Publish<TMessage>(TMessage message, string name = null);

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <typeparam name="TResponseMessage">The type of the t response message.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TResponseMessage&gt;.</returns>
        Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <typeparam name="TResponseMessage">The type of the t response message.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TResponseMessage&gt;.</returns>
        Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, string name = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <typeparam name="TResponseMessage">The type of the t response message.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="name">The name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;TResponseMessage&gt;.</returns>
        Task<TResponseMessage> Send<TResponseMessage>(IRequestMessage<TResponseMessage> request, TimeSpan timeout, string name = null, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
