// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ResourceSelectorConverter.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ResourceSelectorConverter. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using IronyModManager.Shared;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class ResourceSelectorConverter.
    /// Implements the <see cref="Avalonia.Controls.ResourceDictionary" />
    /// Implements the <see cref="Avalonia.Data.Converters.IValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ResourceDictionary" />
    /// <seealso cref="Avalonia.Data.Converters.IValueConverter" />
    [ExcludeFromCoverage("External logic.")]
    public class ResourceSelectorConverter : ResourceDictionary, IValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object key, Type targetType, object parameter, CultureInfo culture)
        {
            TryGetResource((string)key, out var value);
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
