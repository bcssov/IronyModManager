﻿// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-10-2022
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="Extensions.SmartFormat.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
#nullable enable

    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Formats the irony smart.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string FormatIronySmart(this string format, params object?[] args)
        {
            return IronyFormatter.Format(format, args);
        }

        /// <summary>
        /// Formats the irony smart.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string FormatIronySmart(this string format, IFormatProvider formatProvider, params object?[] args)
        {
            return IronyFormatter.Format(formatProvider, format, args);
        }

        #endregion Methods

#nullable disable
    }
}
