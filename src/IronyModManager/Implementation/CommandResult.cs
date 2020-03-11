// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-08-2020
//
// Last Modified By : Mario
// Last Modified On : 03-11-2020
// ***********************************************************************
// <copyright file="CommandResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Implementation
{
    /// <summary>
    /// Class CommandResult.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandResult<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult{T}" /> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="state">The state.</param>
        public CommandResult(T result, CommandState state)
        {
            Result = result;
            State = state;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public T Result { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public CommandState State { get; set; }

        #endregion Properties
    }
}
