// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 05-14-2023
//
// Last Modified By : Mario
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="IObjectClone.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.Models;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Interface IObjectClone
    /// </summary>
    public interface IObjectClone
    {
        #region Methods

        /// <summary>
        /// Clones the definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="includeCode">if set to <c>true</c> [include code].</param>
        /// <returns>IDefinition.</returns>
        public IDefinition CloneDefinition(IDefinition definition, bool includeCode);

        #endregion Methods
    }
}
