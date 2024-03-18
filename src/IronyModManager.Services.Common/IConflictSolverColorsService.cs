
// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-18-2024
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="IConflictSolverColorsService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{

    /// <summary>
    /// An conflict solver colors service interface.
    /// </summary>
    public interface IConflictSolverColorsService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Get.
        /// </summary>
        /// <returns>An IConflictSolverColors.<see cref="IConflictSolverColors" /></returns>
        IConflictSolverColors Get();

        /// <summary>
        /// Has any.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>A bool.</returns>
        bool HasAny(IConflictSolverColors color);

        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="color">The color.</param>
        void Save(IConflictSolverColors color);

        #endregion Methods
    }
}
