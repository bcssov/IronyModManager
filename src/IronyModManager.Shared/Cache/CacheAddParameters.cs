// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-25-2021
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="CacheAddParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Cache
{
    /// <summary>
    /// Class CacheAddParameters.
    /// Implements the <see cref="IronyModManager.Shared.Cache.CacheParameters" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IronyModManager.Shared.Cache.CacheParameters" />
    public class CacheAddParameters<T> : CacheParameters where T : class
    {
        #region Fields

        /// <summary>
        /// The key
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public TimeSpan? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get
            {
                return key ?? string.Empty;
            }
            set
            {
                key = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum items.
        /// </summary>
        /// <value>The maximum items.</value>
        public virtual int? MaxItems { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value { get; set; }

        #endregion Properties
    }
}
