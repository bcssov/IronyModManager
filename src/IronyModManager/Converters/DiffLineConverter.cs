// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-21-2020
//
// Last Modified By : Mario
// Last Modified On : 03-23-2020
// ***********************************************************************
// <copyright file="DiffLineConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using DiffPlex.DiffBuilder.Model;
using IronyModManager.Shared;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class DiffLineConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IValueConverter" />
    public class DiffLineConverter : IValueConverter
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
            if (value is DiffPiece)
            {
                var diff = value as DiffPiece;
                return diff.Type switch
                {
                    ChangeType.Deleted => "DiffDeletedLine",
                    ChangeType.Inserted => "DiffInsertedLine",
                    ChangeType.Imaginary => "DiffImaginaryLine",
                    _ => string.Empty,
                };
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        [ExcludeFromCoverage("Not being used.")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion Methods
    }
}
