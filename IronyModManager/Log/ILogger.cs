// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
// ***********************************************************************
// <copyright file="ILogger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace IronyModManager.Log
{
    /// <summary>
    /// Interface ILogger
    /// </summary>
    public interface ILogger
    {
        #region Methods

        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        void Error(Exception ex, string message = "");

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        #endregion Methods
    }
}
