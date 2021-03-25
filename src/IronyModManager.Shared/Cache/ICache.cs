// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-23-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
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
        /// Gets the specified parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T.</returns>
        T Get<T>(CacheGetParameters parameters) where T : class;

        /// <summary>
        /// Invalidates the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        void Invalidate(CacheInvalidateParameters parameters);

        /// <summary>
        /// Sets the specified parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        void Set<T>(CacheAddParameters<T> parameters) where T : class;

        #endregion Methods
    }
}
