// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-14-2020
//
// Last Modified By : Mario
// Last Modified On : 10-13-2020
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Avalonia.Collections;
using DynamicData;
using IronyModManager.Shared;

namespace IronyModManager.Common
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    [ExcludeFromCoverage("Excluding extension methods.")]
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservable<T>(this IObservable<T> source)
        {
            return ObservableExtensions.Subscribe(source);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext)
        {
            return ObservableExtensions.Subscribe(source, onNext);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onError">The on error.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            // Seriously annoyed with conflicting namespaces
            return ObservableExtensions.Subscribe(source, onNext, onError);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onCompleted">The on completed.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            // Seriously annoyed with conflicting namespaces
            return ObservableExtensions.Subscribe(source, onNext, onCompleted);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onError">The on error.</param>
        /// <param name="onCompleted">The on completed.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return ObservableExtensions.Subscribe(source, onNext, onError, onCompleted);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="observer">The observer.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, IObserver<T> observer, CancellationToken token)
        {
            ObservableExtensions.Subscribe(source, observer, token);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, CancellationToken token)
        {
            ObservableExtensions.Subscribe(source, token);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, CancellationToken token)
        {
            ObservableExtensions.Subscribe(source, onNext, token);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onError">The on error.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, CancellationToken token)
        {
            ObservableExtensions.Subscribe(source, onNext, onError, token);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onCompleted">The on completed.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted, CancellationToken token)
        {
            ObservableExtensions.Subscribe(source, onNext, onCompleted, token);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="onNext">The on next.</param>
        /// <param name="onError">The on error.</param>
        /// <param name="onCompleted">The on completed.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted, CancellationToken token)
        {
            ObservableExtensions.Subscribe(source, onNext, onError, onCompleted, token);
        }

        /// <summary>
        /// Subscribes the observable safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="observer">The observer.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservableSafe<T>(this IObservable<T> source, IObserver<T> observer)
        {
            return ObservableExtensions.SubscribeSafe(source, observer);
        }

        /// <summary>
        /// Converts to avalonialist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <returns>AvaloniaList&lt;T&gt;.</returns>
        public static AvaloniaList<T> ToAvaloniaList<T>(this IEnumerable<T> col)
        {
            return new AvaloniaList<T>(col);
        }

        /// <summary>
        /// Converts to localizedpercentage.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string ToLocalizedPercentage(this int number)
        {
            return (number / 100).ToString("P");
        }

        /// <summary>
        /// Converts to localizedpercentage.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string ToLocalizedPercentage(this double number)
        {
            return (number / 100).ToString("P");
        }

        /// <summary>
        /// Converts to observablecollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <returns>ObservableCollection&lt;T&gt;.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }

        /// <summary>
        /// Converts to sourcelist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <returns>SourceList&lt;T&gt;.</returns>
        public static SourceList<T> ToSourceList<T>(this IEnumerable<T> col)
        {
            return new SourceList<T>(col.AsObservableChangeSet());
        }

        #endregion Methods
    }
}
