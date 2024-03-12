// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-14-2020
//
// Last Modified By : Mario
// Last Modified On : 02-20-2024
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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using IronyModManager.DI;
using IronyModManager.Platform.Configuration;
using IronyModManager.Shared;
using ReactiveUI;

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
        /// Ensures the titlebar spacing.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void EnsureTitlebarSpacing(this Window window)
        {
            var config = DIResolver.Get<IPlatformConfiguration>().GetOptions();
            if (!config.TitleBar.Native)
            {
                window.ExtendClientAreaToDecorationsHint = true;
                window.ExtendClientAreaTitleBarHeightHint = 30d;
                window.Padding = new Thickness(0, 30, 0, 0);
            }
        }

        /// <summary>
        /// Observables WhenAnyValue.
        /// </summary>
        /// <typeparam name="TSender">The type of the TSender.</typeparam>
        /// <typeparam name="TRet">The type of the TRet.</typeparam>
        /// <typeparam name="T1">The type of the T1.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="property">The Property.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>an IObservable with  TRets .<see cref="IObservable{TRet}" /></returns>
        public static IObservable<TRet> ObservableWhenAnyValue<TSender, TRet, T1>(this TSender sender, Expression<Func<TSender, T1>> property, Func<T1, TRet> selector)
        {
            var innerSelector = selector;
            return sender.WhenAny(property, c1 => innerSelector(c1.Value));
        }

        /// <summary>
        /// Safe invoke.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        public static void SafeInvoke(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.InvokeAsync(action);
            }
        }

        /// <summary>
        /// Safes invoke asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<T> SafeInvokeAsync<T>(this Dispatcher dispatcher, Func<Task<T>> action)
        {
            if (dispatcher.CheckAccess())
            {
                return action();
            }
            else
            {
                return dispatcher.InvokeAsync(action);
            }
        }

        /// <summary>
        /// Safes invoke asynchronous.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        public static async Task SafeInvokeAsync(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                await dispatcher.InvokeAsync(action);
            }
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable SubscribeObservable<T>(this IObservable<T> source)
        {
            return source.Subscribe();
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
            return source.Subscribe(onNext);
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
            return source.Subscribe(onNext, onError);
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
            return source.Subscribe(onNext, onCompleted);
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
            return source.Subscribe(onNext, onError, onCompleted);
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
            source.Subscribe(observer, token);
        }

        /// <summary>
        /// Subscribes the observable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="token">The token.</param>
        public static void SubscribeObservable<T>(this IObservable<T> source, CancellationToken token)
        {
            source.Subscribe(token);
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
            source.Subscribe(onNext, token);
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
            source.Subscribe(onNext, onError, token);
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
            source.Subscribe(onNext, onCompleted, token);
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
            source.Subscribe(onNext, onError, onCompleted, token);
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
            return source.SubscribeSafe(observer);
        }

        /// <summary>
        /// Converts to AvaloniaList.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <returns>AvaloniaList&lt;T&gt;.</returns>
        public static AvaloniaList<T> ToAvaloniaList<T>(this IEnumerable<T> col)
        {
            return new AvaloniaList<T>(col);
        }

        /// <summary>
        /// Converts to LocalizedPercentage.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string ToLocalizedPercentage(this int number)
        {
            return ToLocalizedPercentage(Convert.ToDouble(number));
        }

        /// <summary>
        /// Converts to LocalizedPercentage.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>System.String.</returns>
        public static string ToLocalizedPercentage(this double number)
        {
            return (number / 100).ToString("P", Helpers.GetFormatProvider());
        }

        /// <summary>
        /// Converts to ObservableCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <returns>ObservableCollection&lt;T&gt;.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }

        /// <summary>
        /// Converts to SourceList.
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
