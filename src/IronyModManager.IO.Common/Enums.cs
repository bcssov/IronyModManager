// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 06-06-2020
//
// Last Modified By : Mario
// Last Modified On : 10-27-2022
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Common
{
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
        Advanced,

        /// <summary>
        /// The default without localization
        /// </summary>
        DefaultWithoutLocalization,

        /// <summary>
        /// The advanced without localization
        /// </summary>
        AdvancedWithoutLocalization,
    }
}
