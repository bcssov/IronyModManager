// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-25-2021
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="CacheInvalidateParameters.cs" company="Mario">
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
    /// Class CacheInvalidateParameters.
    /// Implements the <see cref="IronyModManager.Shared.Cache.CacheParameters" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Cache.CacheParameters" />
    public class CacheInvalidateParameters : CacheParameters
    {
        #region Fields

        /// <summary>
        /// The keys
        /// </summary>
        private IEnumerable<string> keys = new List<string>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public IEnumerable<string> Keys
        {
            get
            {
                return keys ?? new List<string>();
            }
            set
            {
                keys = value;
            }
        }

        #endregion Properties
    }
}
