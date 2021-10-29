// ***********************************************************************
// Assembly         : IronyModManager.Tests.Common
// Author           : Mario
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2021
// ***********************************************************************
// <copyright file="Logger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Shared;
using NLog;

namespace IronyModManager.Tests.Common
{
    /// <summary>
    /// Class Logger.
    /// Implements the <see cref="IronyModManager.Shared.ILogger" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.ILogger" />
    [ExcludeFromCoverage("Exclude logger.")]
    public class Logger : Shared.ILogger
    {
        /// <summary>
        /// The log
        /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible

        #region Fields

        protected static NLog.Logger log = LogManager.GetCurrentClassLogger();

        #endregion Fields

#pragma warning restore CA2211 // Non-constant fields should not be visible

        #region Methods

        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public void Error(Exception ex, string message = Constants.EmptyParam)
        {
            if (ex != null)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    log.Error(ex, message);
                }
                else
                {
                    log.Error(ex);
                }
            }
        }

        /// <summary>
        /// Fatals the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public void Fatal(Exception ex, string message = Constants.EmptyParam)
        {
            if (ex != null)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    log.Fatal(ex, message);
                }
                else
                {
                    log.Fatal(ex);
                }
            }
        }

        /// <summary>
        /// Gets the last fatal exception message.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetLastFatalExceptionMessage()
        {
            return string.Empty;
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                log.Info(message);
            }
        }

        /// <summary>
        /// Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Trace(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                log.Trace(message);
            }
        }

        #endregion Methods
    }
}
