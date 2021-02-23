// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-23-2021
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="Subject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Class Subject.
    /// Implements the <see cref="System.Reactive.Subjects.SubjectBase{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="System.Reactive.Subjects.SubjectBase{TMessage}" />
    public class Subject<TMessage> : System.Reactive.Subjects.SubjectBase<TMessage> where TMessage : IMessageBusEvent
    {
        #region Fields

        /// <summary>
        /// The disposables
        /// </summary>
        private List<IDisposable> disposables;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The subject
        /// </summary>
        private System.Reactive.Subjects.Subject<TMessage> subject;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subject{TMessage}" /> class.
        /// </summary>
        public Subject()
        {
            subject = new System.Reactive.Subjects.Subject<TMessage>();
            disposables = new List<IDisposable>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Indicates whether the subject has observers subscribed to it.
        /// </summary>
        /// <value><c>true</c> if this instance has observers; otherwise, <c>false</c>.</value>
        public override bool HasObservers => subject != null && subject.HasObservers;

        /// <summary>
        /// Indicates whether the subject has been disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public override bool IsDisposed => subject == null || subject.IsDisposed;

        /// <summary>
        /// Gets the subscribers.
        /// </summary>
        /// <value>The subscribers.</value>
        public int Subscribers => (disposables?.Where(p => p != null).Count()).GetValueOrDefault();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Releases all resources used by the current instance of the subject and unsubscribes all observers.
        /// </summary>
        public override void Dispose()
        {
            if (disposed)
            {
                return;
            }
            GC.SuppressFinalize(this);
            disposed = true;
            if (subject != null)
            {
                subject.Dispose();
                subject = null;
            }
            if (disposables != null)
            {
                disposables.Clear();
                disposables = null;
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            if (subject != null)
            {
                subject.OnCompleted();
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the specified exception.
        /// </summary>
        /// <param name="error">The exception to send to all currently subscribed observers.</param>
        public override void OnError(Exception error)
        {
            if (subject != null)
            {
                subject.OnError(error);
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the arrival of the specified element in the sequence.
        /// </summary>
        /// <param name="value">The value to send to all currently subscribed observers.</param>
        public override void OnNext(TMessage value)
        {
            if (subject != null)
            {
                subject.OnNext(value);
            }
        }

        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">Observer to subscribe to the subject.</param>
        /// <returns>Disposable object that can be used to unsubscribe the observer from the subject.</returns>
        public override IDisposable Subscribe(IObserver<TMessage> observer)
        {
            if (subject != null)
            {
                var subscriber = subject.Subscribe(observer);
                disposables.Add(subscriber);
                return subscriber;
            }
            return null;
        }

        #endregion Methods
    }
}
