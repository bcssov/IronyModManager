// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="IronyMessageBus.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using IronyModManager.Shared.MessageBus;
using SlimMessageBus;

namespace IronyModManager.DI.MessageBus
{
    /// <summary>
    /// Class IronyMessageBus.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IIronyMessageBus" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IIronyMessageBus" />
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
        /// Initializes a new instance of the <see cref="IronyMessageBus" /> class.
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
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public Task Publish<T>(T message) where T : IMessageBusEvent
        {
            return messageBus.Publish(message);
        }

        #endregion Methods
    }
}
