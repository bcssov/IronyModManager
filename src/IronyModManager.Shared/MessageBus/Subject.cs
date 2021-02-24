// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : reactive
// Created          : 02-23-2021
//
// Last Modified By : Mario
// Last Modified On : 02-24-2021
// ***********************************************************************
// <copyright file="Subject.cs" company="reactive">
//     reactive
// </copyright>
// <summary>Taken from system reactive: https://github.com/dotnet/reactive/blob/main/Rx.NET/Source/src/System.Reactive/Subjects/Subject.cs</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

// Source: https://github.com/dotnet/reactive/blob/main/Rx.NET/Source/src/System.Reactive/Subjects/Subject.cs

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Represents an object that is both an observable sequence as well as an observer.
    /// Each notification is broadcasted to all subscribed observers.
    /// Implements the <see cref="System.Reactive.Subjects.SubjectBase{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="System.Reactive.Subjects.SubjectBase{TMessage}" />
    public sealed class Subject<TMessage> : SubjectBase<TMessage> where TMessage : IMessageBusEvent
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        #region Fields

        /// <summary>
        /// The disposed
        /// </summary>
        private static readonly SubjectDisposable[] Disposed = new SubjectDisposable[0];

        /// <summary>
        /// The terminated
        /// </summary>
        private static readonly SubjectDisposable[] Terminated = new SubjectDisposable[0];

        /// <summary>
        /// The exception
        /// </summary>
        private Exception? _exception;

        /// <summary>
        /// The observers
        /// </summary>
        private SubjectDisposable[] _observers;

        #endregion Fields

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        #region Constructors

        /// <summary>
        /// Creates a subject.
        /// </summary>
        public Subject() => _observers = Array.Empty<SubjectDisposable>();

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Indicates whether the subject has observers subscribed to it.
        /// </summary>
        /// <value><c>true</c> if this instance has observers; otherwise, <c>false</c>.</value>
        public override bool HasObservers => Volatile.Read(ref _observers).Length != 0;

        /// <summary>
        /// Indicates whether the subject has been disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public override bool IsDisposed => Volatile.Read(ref _observers) == Disposed;

        /// <summary>
        /// Gets the subscribers.
        /// </summary>
        /// <value>The subscribers.</value>
        public int Subscribers => !IsDisposed ? Volatile.Read(ref _observers).Length : 0;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Subject{T}" /> class and unsubscribes all observers.
        /// </summary>
        public override void Dispose()
        {
            Interlocked.Exchange(ref _observers, Disposed);
            _exception = null;
        }

        /// <summary>
        /// Notifies all subscribed observers about the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            for (; ; )
            {
                var observers = Volatile.Read(ref _observers);

                if (observers == Disposed)
                {
                    _exception = null;
                    ThrowDisposed();
                    break;
                }

                if (observers == Terminated)
                {
                    break;
                }

                if (Interlocked.CompareExchange(ref _observers, Terminated, observers) == observers)
                {
                    foreach (var observer in observers)
                    {
                        observer.Observer?.OnCompleted();
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the specified exception.
        /// </summary>
        /// <param name="error">The exception to send to all currently subscribed observers.</param>
        /// <exception cref="ArgumentNullException">error</exception>
        public override void OnError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            for (; ; )
            {
                var observers = Volatile.Read(ref _observers);

                if (observers == Disposed)
                {
                    _exception = null;
                    ThrowDisposed();
                    break;
                }

                if (observers == Terminated)
                {
                    break;
                }

                _exception = error;

                if (Interlocked.CompareExchange(ref _observers, Terminated, observers) == observers)
                {
                    foreach (var observer in observers)
                    {
                        observer.Observer?.OnError(error);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the arrival of the specified element in the sequence.
        /// </summary>
        /// <param name="value">The value to send to all currently subscribed observers.</param>
        public override void OnNext(TMessage value)
        {
            var observers = Volatile.Read(ref _observers);

            if (observers == Disposed)
            {
                _exception = null;
                ThrowDisposed();
                return;
            }

            foreach (var observer in observers)
            {
                observer.Observer?.OnNext(value);
            }
        }

        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">Observer to subscribe to the subject.</param>
        /// <returns>Disposable object that can be used to unsubscribe the observer from the subject.</returns>
        /// <exception cref="ArgumentNullException">observer</exception>
        public override IDisposable Subscribe(IObserver<TMessage> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            var disposable = default(SubjectDisposable);
            for (; ; )
            {
                var observers = Volatile.Read(ref _observers);

                if (observers == Disposed)
                {
                    _exception = null;
                    ThrowDisposed();

                    break;
                }

                if (observers == Terminated)
                {
                    var ex = _exception;

                    if (ex != null)
                    {
                        observer.OnError(ex);
                    }
                    else
                    {
                        observer.OnCompleted();
                    }

                    break;
                }

                disposable ??= new SubjectDisposable(this, observer);

                var n = observers.Length;
                var b = new SubjectDisposable[n + 1];

                Array.Copy(observers, 0, b, 0, n);

                b[n] = disposable;

                if (Interlocked.CompareExchange(ref _observers, b, observers) == observers)
                {
                    return disposable;
                }
            }

            return Disposable.Empty;
        }

        /// <summary>
        /// Throws the disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private static void ThrowDisposed() => throw new ObjectDisposedException(string.Empty);

        /// <summary>
        /// Unsubscribes the specified observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        private void Unsubscribe(SubjectDisposable observer)
        {
            for (; ; )
            {
                var a = Volatile.Read(ref _observers);
                var n = a.Length;

                if (n == 0)
                {
                    break;
                }

                var j = Array.IndexOf(a, observer);

                if (j < 0)
                {
                    break;
                }

                SubjectDisposable[] b;

                if (n == 1)
                {
                    b = Array.Empty<SubjectDisposable>();
                }
                else
                {
                    b = new SubjectDisposable[n - 1];

                    Array.Copy(a, 0, b, 0, j);
                    Array.Copy(a, j + 1, b, j, n - j - 1);
                }

                if (Interlocked.CompareExchange(ref _observers, b, a) == a)
                {
                    break;
                }
            }
        }

        #endregion Methods

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        #region Classes

        /// <summary>
        /// Class SubjectDisposable. This class cannot be inherited.
        /// Implements the <see cref="System.IDisposable" />
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private sealed class SubjectDisposable : IDisposable
        {
            /// <summary>
            /// The observer
            /// </summary>

            #region Fields

            private volatile IObserver<TMessage>? _observer;

            /// <summary>
            /// The subject
            /// </summary>
            private Subject<TMessage> _subject;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SubjectDisposable" /> class.
            /// </summary>
            /// <param name="subject">The subject.</param>
            /// <param name="observer">The observer.</param>
            public SubjectDisposable(Subject<TMessage> subject, IObserver<TMessage> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the observer.
            /// </summary>
            /// <value>The observer.</value>
            public IObserver<TMessage>? Observer => _observer;

            #endregion Properties

            #region Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                var observer = Interlocked.Exchange(ref _observer, null);
                if (observer == null)
                {
                    return;
                }

                _subject.Unsubscribe(this);
                _subject = null!;
            }

            #endregion Methods
        }

        #endregion Classes

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
