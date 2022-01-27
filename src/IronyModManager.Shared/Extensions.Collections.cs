// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="Extensions.Collections.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Conditionals the filter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">The col.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> ConditionalFilter<T>(this IEnumerable<T> col, bool condition, Func<IEnumerable<T>, IEnumerable<T>> filter)
        {
            return condition ? filter(col) : col;
        }

        /// <summary>
        /// Listses the same.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>bool.</returns>
        public static bool ListsSame<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null || second == null)
            {
                return false;
            }
            return first.SequenceEqual(second);
        }

        #endregion Methods
    }
}
