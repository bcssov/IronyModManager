// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="LocalizationConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class LocalizationConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IValueConverter" />
    public class LocalizationConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var locManager = DIResolver.Get<ILocalizationManager>();
                string prefix = string.Empty;
                if (parameter != null && !string.IsNullOrWhiteSpace(parameter.ToString()))
                {
                    prefix = parameter.ToString();
                }
                var resKey = $"{prefix}{value}";
                var translation = locManager.GetResource(resKey);
                return translation;
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
