// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 12-08-2025
//
// Last Modified By : Mario
// Last Modified On : 12-08-2025
// ***********************************************************************
// <copyright file="ReactiveExtensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    /// <summary>
    /// Class ReactiveExtensions.
    /// </summary>
    public static class ReactiveExtensions
    {
        #region Methods

        // Why is this here? ReactiveUI decided to remove it just cause...
        /// <summary>
        /// Ensures the provided disposable is disposed with the specified System.Reactive.Disposables.CompositeDisposable.
        /// </summary>
        /// <typeparam name="T">The type of the disposable.</typeparam>
        /// <param name="item">The disposable we are going to want to be disposed by the CompositeDisposable.</param>
        /// <param name="compositeDisposable">The composite disposable.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.ArgumentNullException">compositeDisposable</exception>
        public static T DisposeWith<T>(this T item, CompositeDisposable compositeDisposable) where T : IDisposable
        {
            switch (compositeDisposable)
            {
                case null:
                    throw new ArgumentNullException(nameof(compositeDisposable));
                default:
                    compositeDisposable.Add(item);
                    return item;
            }
        }

        #endregion Methods
    }
}
