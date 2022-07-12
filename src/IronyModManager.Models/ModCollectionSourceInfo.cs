// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 07-12-2022
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="ModCollectionSourceInfo.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class ModCollectionSourceInfo.
    /// Implements the <see cref="BaseModel" />
    /// Implements the <see cref="IModCollectionSourceInfo" />
    /// </summary>
    /// <seealso cref="BaseModel" />
    /// <seealso cref="IModCollectionSourceInfo" />
    public class ModCollectionSourceInfo : BaseModel, IModCollectionSourceInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the paradox identifier.
        /// </summary>
        /// <value>The paradox identifier.</value>
        public virtual long? ParadoxId { get; set; }

        /// <summary>
        /// Gets or sets the steam identifier.
        /// </summary>
        /// <value>The steam identifier.</value>
        public virtual long? SteamId { get; set; }

        #endregion Properties
    }
}
