// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2020
// ***********************************************************************
// <copyright file="ICache.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace IronyModManager.Shared.Cache
{
    /// <summary>
    /// Interface ICache
    /// </summary>
    public interface ICache
    {
        #region Methods

        /// <summary>
        /// Gets the specified prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        T Get<T>(string prefix, string key) where T : class;

        /// <summary>
        /// Sets the specified prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Set<T>(string prefix, string key, T value) where T : class;

        /// <summary>
        /// Sets the specified prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The expiration.</param>
        void Set<T>(string prefix, string key, T value, TimeSpan? expiration) where T : class;

        #endregion Methods
    }
}
