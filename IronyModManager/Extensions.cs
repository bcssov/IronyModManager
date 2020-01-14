// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-14-2020
//
// Last Modified By : Mario
// Last Modified On : 01-14-2020
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
using Avalonia.Collections;

namespace IronyModManager
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

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
        /// Converts to observablecollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <returns>ObservableCollection&lt;T&gt;.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }

        #endregion Methods
    }
}
