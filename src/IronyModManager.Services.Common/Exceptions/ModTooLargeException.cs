
// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-10-2023
//
// Last Modified By : Mario
// Last Modified On : 06-10-2023
// ***********************************************************************
// <copyright file="ModTooLargeException.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Services.Common.Exceptions
{

    /// <summary>
    /// Class ModTooLargeException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class ModTooLargeException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ModTooLargeException(string message) : base(message) { }

        #endregion Constructors
    }
}
