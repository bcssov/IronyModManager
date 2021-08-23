// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 08-23-2021
//
// Last Modified By : Mario
// Last Modified On : 08-23-2021
// ***********************************************************************
// <copyright file="IQueryableModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Interface IQueryable
    /// </summary>
    public interface IQueryableModel
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified term is match.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns><c>true</c> if the specified term is match; otherwise, <c>false</c>.</returns>
        bool IsMatch(string term);

        #endregion Methods
    }
}
