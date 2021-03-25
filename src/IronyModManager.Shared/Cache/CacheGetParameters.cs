// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-25-2021
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="CacheGetParameters.cs" company="Mario">
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
    /// Class CacheGetParameters.
    /// Implements the <see cref="IronyModManager.Shared.Cache.CacheParameters" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Cache.CacheParameters" />
    public class CacheGetParameters : CacheParameters
    {
        #region Fields

        /// <summary>
        /// The key
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

        #region Properties

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

        #endregion Properties
    }
}
