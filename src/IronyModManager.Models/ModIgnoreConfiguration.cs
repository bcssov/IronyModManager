// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 06-28-2023
//
// Last Modified By : Mario
// Last Modified On : 07-16-2023
// ***********************************************************************
// <copyright file="ModIgnoreConfiguration.cs" company="Mario">
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
    /// Class ModIgnoreConfiguration.
    /// Implements the <see cref="BaseModel" />
    /// Implements the <see cref="IModIgnoreConfiguration" />
    /// </summary>
    /// <seealso cref="BaseModel" />
    /// <seealso cref="IModIgnoreConfiguration" />
    public class ModIgnoreConfiguration : BaseModel, IModIgnoreConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        public string ModName { get; set; }

        #endregion Properties
    }
}
