// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-28-2022
// ***********************************************************************
// <copyright file="Logger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
                log.Error(ex, FormatMessage(message));
            }
        }

        /// <summary>
        /// Fatals the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public void Fatal(Exception ex, string message = Shared.Constants.EmptyParam)
        {
            if (ex != null)
            {
                log.Fatal(ex, FormatMessage(message));
            }
        }

        /// <summary>
        /// Gets the last fatal exception message.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetLastFatalExceptionMessage()
        {
            var fileTarget = log.Factory.Configuration.AllTargets.FirstOrDefault(p => p is IronyFileTarget);
            if (fileTarget != null)
            {
                return ((IronyFileTarget)fileTarget).GetLastFatalException();
            }
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
                log.Info(FormatMessage(message));
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
                log.Trace(FormatMessage(message));
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

        /// <summary>
        /// Formats the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>string.</returns>
        private string FormatMessage(string message)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(message))
            {
                sb.AppendLine(message);
            }
            sb.AppendLine($"Version: {FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).ProductVersion}");
            sb.AppendLine($"OS Description: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"Runtime Identifier: {RuntimeInformation.RuntimeIdentifier}");
            return sb.ToString();
        }

        #endregion Methods
    }
}
