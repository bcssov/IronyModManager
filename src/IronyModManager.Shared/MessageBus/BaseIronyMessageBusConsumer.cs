// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="BaseIronyMessageBusConsumer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Class BaseIronyMessageBusConsumer.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IIronyMessageBusConsumer{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IIronyMessageBusConsumer{TMessage}" />
    public abstract class BaseIronyMessageBusConsumer<TMessage> : IIronyMessageBusConsumer<TMessage> where TMessage : IMessageBusEvent
    {
        #region Fields

        /// <summary>
        /// The message subject
        /// </summary>
        private readonly Subject<TMessage> messageSubject;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIronyMessageBusConsumer{TMessage}" /> class.
        /// </summary>
        public BaseIronyMessageBusConsumer()
        {
            messageSubject = new Subject<TMessage>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public virtual IObservable<TMessage> Message => messageSubject;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [handle].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="name">The name.</param>
        /// <returns>Task.</returns>
        public virtual Task OnHandle(TMessage message, string name)
        {
            messageSubject.OnNext(message);
            return Task.FromResult(true);
        }

        #endregion Methods
    }
}
