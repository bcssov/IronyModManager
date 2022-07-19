// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 07-12-2022
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="IModCollectionSourceInfo.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IModCollectionSourceInfo
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IModCollectionSourceInfo : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the paradox identifier.
        /// </summary>
        /// <value>The paradox identifier.</value>
        long? ParadoxId { get; set; }

        /// <summary>
        /// Gets or sets the steam identifier.
        /// </summary>
        /// <value>The steam identifier.</value>
        long? SteamId { get; set; }

        #endregion Properties
    }
}
