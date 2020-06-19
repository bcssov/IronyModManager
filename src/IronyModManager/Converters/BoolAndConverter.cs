// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-15-2020
//
// Last Modified By : Mario
// Last Modified On : 06-15-2020
// ***********************************************************************
// <copyright file="BoolAndConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class InstalledModsSelectionConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IMultiValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IMultiValueConverter" />
    public class BoolAndConverter : IMultiValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Count > 0)
            {
                var bools = new List<bool>();
                foreach (var item in values)
                {
                    if (item is bool boolValue)
                    {
                        bools.Add(boolValue);
                    }
                }
                return bools.All(p => p);
            }
            return false;
        }

        #endregion Methods
    }
}
