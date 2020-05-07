// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ByteSizeHelper.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ByteSizeHelper. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ByteSizeHelper.
    /// </summary>
    [ExcludeFromCoverage("External logic.")]
    public class ByteSizeHelper
    {
        #region Fields

        /// <summary>
        /// The format template
        /// </summary>
        private const string FormatTemplate = "{0}{1:0.#} {2}";

        /// <summary>
        /// The prefixes
        /// </summary>
        private static readonly string[] Prefixes =
        {
            "B",
            "KB",
            "MB",
            "GB",
            "TB",
            "PB",
            "EB",
            "ZB",
            "YB"
        };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(ulong bytes)
        {
            if (bytes == 0)
            {
                return string.Format(FormatTemplate, null, 0, Prefixes[0]);
            }

            var absSize = Math.Abs((double)bytes);
            var fpPower = Math.Log(absSize, 1000);
            var intPower = (int)fpPower;
            var iUnit = intPower >= Prefixes.Length
                ? Prefixes.Length - 1
                : intPower;
            var normSize = absSize / Math.Pow(1000, iUnit);

            return string.Format(FormatTemplate, bytes < 0 ? "-" : null, normSize, Prefixes[iUnit]);
        }

        #endregion Methods
    }
}
