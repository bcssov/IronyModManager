// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-25-2021
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="CacheParameters.cs" company="Mario">
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
    /// Class CacheParameters.
    /// </summary>
    public abstract class CacheParameters
    {
        #region Fields

        /// <summary>
        /// The prefix
        /// </summary>
        private string prefix = string.Empty;

        /// <summary>
        /// The region
        /// </summary>
        private string region = string.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix
        {
            get
            {
                return prefix ?? string.Empty;
            }
            set
            {
                prefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        public string Region
        {
            get
            {
                return region ?? string.Empty;
            }
            set
            {
                region = value;
            }
        }

        #endregion Properties
    }
}
