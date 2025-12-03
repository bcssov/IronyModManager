// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 01-28-2022
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
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
        JsonMetadata,

        /// <summary>
        /// The json metadata v2
        /// </summary>
        JsonMetadataV2
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
