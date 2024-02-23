// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-20-2024
//
// Last Modified By : Mario
// Last Modified On : 02-23-2024
// ***********************************************************************
// <copyright file="Extensions.Double.cs" company="Mario">
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
        #region Fields

        /// <summary>
        /// A private const double named defaultTolerance.
        /// </summary>
        private const double defaultTolerance = 0.01;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Is nearly equal.
        /// </summary>
        /// <param name="a">The A.</param>
        /// <param name="b">The B.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>A bool.</returns>
        public static bool IsNearlyEqual(this double a, double b, double tolerance)
        {
            return Math.Abs(a - b) < tolerance;
        }

        /// <summary>
        /// Is nearly equal.
        /// </summary>
        /// <param name="a">The A.</param>
        /// <param name="b">The B.</param>
        /// <returns>A bool.</returns>
        public static bool IsNearlyEqual(this double a, double b)
        {
            return IsNearlyEqual(a, b, defaultTolerance);
        }

        /// <summary>
        /// Is not nearly equal.
        /// </summary>
        /// <param name="a">The A.</param>
        /// <param name="b">The B.</param>
        /// <returns>A bool.</returns>
        public static bool IsNotNearlyEqual(this double a, double b)
        {
            return !IsNearlyEqual(a, b);
        }

        /// <summary>
        /// Is not nearly equal.
        /// </summary>
        /// <param name="a">The A.</param>
        /// <param name="b">The B.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>A bool.</returns>
        public static bool IsNotNearlyEqual(this double a, double b, double tolerance)
        {
            return !IsNearlyEqual(a, b, tolerance);
        }

        #endregion Methods
    }
}
