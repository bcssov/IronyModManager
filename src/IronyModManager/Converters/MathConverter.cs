// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-13-2020
// ***********************************************************************
// <copyright file="MathConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using info.lundin.math;
using IronyModManager.Localization;

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()) && parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
            {
                if (double.TryParse(value.ToString(), out var x))
                {
                    var parser = new ExpressionParser
                    {
                        Culture = CurrentLocale.CurrentCulture
                    };
                    parser.Values.Add("x", x);
                    var result = parser.Parse(parameter.ToString().Replace(".", CurrentLocale.CurrentCulture.NumberFormat.NumberDecimalSeparator));
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion Methods
    }
}
