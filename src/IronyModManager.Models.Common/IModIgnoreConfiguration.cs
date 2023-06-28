
// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 06-28-2023
//
// Last Modified By : Mario
// Last Modified On : 06-28-2023
// ***********************************************************************
// <copyright file="IModIgnoreConfiguration.cs" company="Mario">
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
    /// Interface IModIgnoreConfiguration
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IModIgnoreConfiguration : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        string ModName { get; set; }

        #endregion Properties
    }
}
