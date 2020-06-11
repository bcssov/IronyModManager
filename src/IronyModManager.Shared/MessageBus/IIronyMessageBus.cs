// ***********************************************************************
// Assembly         : IronyModManager.Shared.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="IIronyMessageBus.cs" company="Mario">
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
    public interface IIronyMessageBus : IDisposable
    {
        #region Methods

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        Task Publish<T>(T message) where T : IMessageBusEvent;

        #endregion Methods
    }
}
