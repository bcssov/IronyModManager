// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 01-28-2022
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common
{
    /// <summary>
    /// Enum DescriptorModType
    /// </summary>
    public enum DescriptorModType
    {
        /// <summary>
        /// The descriptor mod
        /// </summary>
        DescriptorMod,

        /// <summary>
        /// The json metadata
        /// </summary>
        JsonMetadata
    }

    /// <summary>
    /// Enum ValidationType
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// The full
        /// </summary>
        Full,

        /// <summary>
        /// The simple only
        /// </summary>
        SimpleOnly,

        /// <summary>
        /// The skip all
        /// </summary>
        SkipAll
    }
}
