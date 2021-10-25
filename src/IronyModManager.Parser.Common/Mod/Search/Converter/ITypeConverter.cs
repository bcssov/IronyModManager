// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-25-2021
// ***********************************************************************
// <copyright file="ITypeConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Mod.Search.Converter
{
    /// <summary>
    /// Interface ITypeConverter
    /// </summary>
    public interface ITypeConverter
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this instance can convert the specified locale; otherwise, <c>false</c>.</returns>
        bool CanConvert(string locale, string key);

#nullable enable

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
        object? Convert(string locale, string value);

        #endregion Methods

#nullable disable
    }
}
