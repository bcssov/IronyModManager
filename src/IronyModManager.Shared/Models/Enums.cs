// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 10-17-2024
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
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
    /// Enum AddToMapAllowedType
    /// </summary>
    public enum AddToMapAllowedType
    {
        /// <summary>
        /// All
        /// </summary>
        All,

        /// <summary>
        /// The invalid and special
        /// </summary>
        InvalidAndSpecial
    }

    /// <summary>
    /// Enum MergeType
    /// </summary>
    public enum MergeType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The flat merge
        /// </summary>
        FlatMerge
    }

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
