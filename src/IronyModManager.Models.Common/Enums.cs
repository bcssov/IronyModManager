// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 06-06-2020
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Enum DefinitionPriorityType
    /// </summary>
    public enum DefinitionPriorityType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The mod order
        /// </summary>
        ModOrder,

        /// <summary>
        /// The fios
        /// </summary>
        FIOS,

        /// <summary>
        /// The lios
        /// </summary>
        LIOS,

        /// <summary>
        /// The mod override
        /// </summary>
        ModOverride
    }

    /// <summary>
    /// Enum ModSource
    /// </summary>
    public enum ModSource
    {
        /// <summary>
        /// The local
        /// </summary>
        Local,

        /// <summary>
        /// The steam
        /// </summary>
        Steam,

        /// <summary>
        /// The paradox
        /// </summary>
        Paradox
    }

    /// <summary>
    /// Enum PatchStateMode
    /// </summary>
    public enum PatchStateMode
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The default
        /// </summary>
        Default,

        /// <summary>
        /// The advanced
        /// </summary>
        Advanced
    }
}
