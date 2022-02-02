// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-02-2022
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Enum ResetType
    /// </summary>
    public enum ResetType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The resolved
        /// </summary>
        Resolved,

        /// <summary>
        /// The ignored
        /// </summary>
        Ignored,

        /// <summary>
        /// Any
        /// </summary>
        Any
    }

    /// <summary>
    /// Enum ValueType
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// The whole text file
        /// </summary>
        WholeTextFile,

        /// <summary>
        /// The variable
        /// </summary>
        Variable,

        /// <summary>
        /// The special variable
        /// </summary>
        SpecialVariable,

        /// <summary>
        /// The namespace
        /// </summary>
        Namespace,

        /// <summary>
        /// The object
        /// </summary>
        Object,

        /// <summary>
        /// The binary
        /// </summary>
        Binary,

        /// <summary>
        /// The invalid
        /// </summary>
        Invalid,

        /// <summary>
        /// The overwritten object
        /// </summary>
        OverwrittenObject,

        /// <summary>
        /// The empty file
        /// </summary>
        EmptyFile,

        /// <summary>
        /// The overwritten object single file
        /// </summary>
        OverwrittenObjectSingleFile
    }
}
