// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 08-11-2020
// ***********************************************************************
// <copyright file="ILogger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

/// <summary>
/// The Shared namespace.
/// </summary>
namespace IronyModManager.Shared
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
        /// <param name="message">The message.</param>
        void Error(Exception ex, string message = Constants.EmptyParam);

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Trace(string message);

        #endregion Methods
    }
}
