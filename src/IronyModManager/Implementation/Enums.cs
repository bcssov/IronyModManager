// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-08-2020
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Implementation
{
    /// <summary>
    /// Enum CommandState
    /// </summary>
    public enum CommandState
    {
        /// <summary>
        /// The success
        /// </summary>
        Success,

        /// <summary>
        /// The failed
        /// </summary>
        Failed,

        /// <summary>
        /// The exists
        /// </summary>
        Exists,

        /// <summary>
        /// The not executed
        /// </summary>
        NotExecuted
    }

    /// <summary>
    /// Enum SortOrder
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The asc
        /// </summary>
        Asc,

        /// <summary>
        /// The desc
        /// </summary>
        Desc
    }
}
