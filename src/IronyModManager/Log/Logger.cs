// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2021
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

namespace IronyModManager.Log
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

        #region Fields

        protected static readonly NLog.Logger log = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public void Error(Exception ex, string message = Shared.Constants.EmptyParam)
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

        /// <summary>
        /// Determines whether [is debug enabled].
        /// </summary>
        /// <returns><c>true</c> if [is debug enabled]; otherwise, <c>false</c>.</returns>
        protected static bool IsDebugEnabled()
        {
            return log.IsDebugEnabled;
        }

        /// <summary>
        /// Determines whether [is error enabled].
        /// </summary>
        /// <returns><c>true</c> if [is error enabled]; otherwise, <c>false</c>.</returns>
        protected static bool IsErrorEnabled()
        {
            return log.IsErrorEnabled;
        }

        /// <summary>
        /// Determines whether [is fatal enabled].
        /// </summary>
        /// <returns><c>true</c> if [is fatal enabled]; otherwise, <c>false</c>.</returns>
        protected static bool IsFatalEnabled()
        {
            return log.IsFatalEnabled;
        }

        /// <summary>
        /// Determines whether [is information enabled].
        /// </summary>
        /// <returns><c>true</c> if [is information enabled]; otherwise, <c>false</c>.</returns>
        protected static bool IsInfoEnabled()
        {
            return log.IsInfoEnabled;
        }

        /// <summary>
        /// Determines whether [is trace enabled].
        /// </summary>
        /// <returns><c>true</c> if [is trace enabled]; otherwise, <c>false</c>.</returns>
        protected static bool IsTraceEnabled()
        {
            return log.IsTraceEnabled;
        }

        /// <summary>
        /// Determines whether [is warn enabled].
        /// </summary>
        /// <returns><c>true</c> if [is warn enabled]; otherwise, <c>false</c>.</returns>
        protected static bool IsWarnEnabled()
        {
            return log.IsWarnEnabled;
        }

        #endregion Methods
    }
}
