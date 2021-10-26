// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
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
    /// <typeparam name="T"></typeparam>
    public interface ITypeConverter<out T> where T : class
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this instance can convert the specified locale; otherwise, <c>false</c>.</returns>
        bool CanConvert(string locale, string key);

        /// <summary>
        /// Converts the specified locale.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        T Convert(string locale, string value);

        #endregion Methods
    }
}
