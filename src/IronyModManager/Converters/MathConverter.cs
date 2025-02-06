// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2025
// ***********************************************************************
// <copyright file="MathConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using IronyModManager.Localization;
using IronyModManager.Shared;
using IronyModManager.Shared.Expressions;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class MathConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IValueConverter" />
    public class MathConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()) && parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
            {
                if (double.TryParse(value.ToString(), out var x))
                {
                    var parser = MathExpression.GetParser();
                    parser.Values.Add("x", x);
                    var result = parser.Parse(parameter.ToString()!.Replace(".", CurrentLocale.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                    return result;
                }
            }

            return value;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        [ExcludeFromCoverage("Not being used.")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion Methods
    }
}
