// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-10-2022
//
// Last Modified By : Mario
// Last Modified On : 07-10-2022
// ***********************************************************************
// <copyright file="IronyFormatter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using SmartFormat;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class IronyFormatter.
    /// </summary>
    public static class IronyFormatter
    {
        #region Fields

        /// <summary>
        /// The smart formatter
        /// </summary>
        private static readonly SmartFormatter smartFormatter = InitFormatter();

        #endregion Fields

        /// <summary>
        /// Formats the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
#nullable enable

        #region Methods

        public static string Format(string format, params object?[] args) => smartFormatter.Format(format, args);

#nullable disable

        /// <summary>
        /// Initializes the formatter.
        /// </summary>
        /// <returns>SmartFormatter.</returns>
        private static SmartFormatter InitFormatter()
        {
            // Sure why not screw over backwards compatibility. I don't mind the improvements but at least allow StringFormatCompatibility to be a global option as well.
            var formatter = Smart.CreateDefaultSmartFormat();
            formatter.Settings.StringFormatCompatibility = true;
            return formatter;
        }

        #endregion Methods
    }
}
