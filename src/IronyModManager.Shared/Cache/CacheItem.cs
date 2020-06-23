// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2020
// ***********************************************************************
// <copyright file="CacheItem.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.Cache
{
    /// <summary>
    /// Class CacheItem.
    /// </summary>
    public class CacheItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItem" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The expiration.</param>
        public CacheItem(object value, TimeSpan? expiration)
        {
            Value = value;
            Expiration = expiration;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTimeOffset Created { get; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public TimeSpan? Expiration { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        #endregion Properties
    }
}
