// ***********************************************************************
// Assembly         : IronyModManager.GameHandler
// Author           : Mario
// Created          : 10-26-2022
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
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
using NLog;

namespace IronyModManager.GameHandler
{
    /// <summary>
    /// Class Logger.
    /// Implements the <see cref="IronyModManager.Shared.ILogger" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.ILogger" />
    internal class Logger : Shared.ILogger
    {
        #region Fields

        /// <summary>
        /// The log
        /// </summary>
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
            return string.Empty;
        }

        /// <summary>
        /// Informations the specified message.
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
        /// Formats the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
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
