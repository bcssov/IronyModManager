// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common
{
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
        Binary
    }
}
