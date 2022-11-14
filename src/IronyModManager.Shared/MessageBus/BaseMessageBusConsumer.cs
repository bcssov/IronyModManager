// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 11-14-2022
// ***********************************************************************
// <copyright file="BaseMessageBusConsumer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Class BaseMessageBusConsumer.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusConsumer{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusConsumer{TMessage}" />
    public abstract class BaseMessageBusConsumer<TMessage> : IMessageBusConsumer<TMessage> where TMessage : IMessageBusEvent
    {
        #region Fields

        /// <summary>
        /// The message subject
        /// </summary>
        private readonly Subject<TMessage> messageSubject;

        /// <summary>
        /// The task cache
        /// </summary>
        private readonly ConcurrentDictionary<BaseAwaitableEvent, int> taskCache;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMessageBusConsumer{TMessage}" /> class.
        /// </summary>
        public BaseMessageBusConsumer()
        {
            messageSubject = new Subject<TMessage>();
            taskCache = new ConcurrentDictionary<BaseAwaitableEvent, int>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [handle].
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual async Task OnHandle(TMessage message)
        {
            messageSubject.OnNext(message);
            if (message is BaseAwaitableEvent awaitableEvent)
            {
                taskCache.TryGetValue(awaitableEvent, out var tasksCompleted);
                while (tasksCompleted < messageSubject.Subscribers)
                {
                    await Task.Delay(25);
                    taskCache.TryGetValue(awaitableEvent, out tasksCompleted);
                }
                taskCache.Remove(awaitableEvent, out _);
            }
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        public virtual IDisposable Subscribe(Action<TMessage> action)
        {
            return messageSubject.Subscribe(s =>
            {
                action(s);
                if (s is BaseAwaitableEvent awaitableEvent)
                {
                    IncrementTaskCounter(awaitableEvent);
                }
            });
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        public virtual IDisposable Subscribe(Func<TMessage, Task> action)
        {
            return messageSubject.Subscribe(async s =>
            {
                await action(s);
                if (s is BaseAwaitableEvent awaitableEvent)
                {
                    IncrementTaskCounter(awaitableEvent);
                }
            });
        }

        /// <summary>
        /// Subscribes the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IDisposable.</returns>
        public virtual IDisposable Subscribe<T>(Func<TMessage, Task<T>> action)
        {
            return messageSubject.Subscribe(async s =>
            {
                await action(s);
                if (s is BaseAwaitableEvent awaitableEvent)
                {
                    IncrementTaskCounter(awaitableEvent);
                }
            });
        }

        /// <summary>
        /// Increments the task counter.
        /// </summary>
        /// <param name="awaitableEvent">The awaitable event.</param>
        protected virtual void IncrementTaskCounter(BaseAwaitableEvent awaitableEvent)
        {
            taskCache.AddOrUpdate(awaitableEvent, 1, (key, oldValue) =>
            {
                return oldValue + 1;
            });
        }

        #endregion Methods
    }
}
