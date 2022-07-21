// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-10-2022
//
// Last Modified By : Mario
// Last Modified On : 07-21-2022
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
#nullable enable

        #region Fields

        /// <summary>
        /// The smart formatter
        /// </summary>
        [ThreadStatic]
        private static SmartFormatter? smartFormatter;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the formatter.
        /// </summary>
        /// <value>The formatter.</value>
        private static SmartFormatter Formatter
        {
            get
            {
                smartFormatter ??= InitFormatter();
                return smartFormatter;
            }
        }

        #endregion Properties

        /// <summary>
        /// Formats the specified format provider.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>

        #region Methods

        public static string Format(IFormatProvider formatProvider, string format, params object?[] args) => Formatter.Format(formatProvider, format, args);

        /// <summary>
        /// Formats the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string Format(string format, params object?[] args) => Formatter.Format(format, args);

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
