// ***********************************************************************
// Assembly         : IronyModManager.Shared.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="IMessageBus.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Interface IIronyMessageBus
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMessageBus : IDisposable
    {
        #region Methods

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <typeparam name="TMessage">The type of the t message.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessageBusEvent;

        #endregion Methods
    }
}
