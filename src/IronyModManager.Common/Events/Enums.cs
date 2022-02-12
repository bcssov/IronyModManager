// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Enum NavigationState
    /// </summary>
    public enum NavigationState
    {
        /// <summary>
        /// The main
        /// </summary>
        Main,

        /// <summary>
        /// The conflict solver
        /// </summary>
        ConflictSolver,

        /// <summary>
        /// The read only conflict solver
        /// </summary>
        ReadOnlyConflictSolver
    }
}
