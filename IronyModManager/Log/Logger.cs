// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="Logger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using NLog;

namespace IronyModManager.Log
{
    /// <summary>
    /// Class Logger.
    /// Implements the <see cref="IronyModManager.Shared.ILogger" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.ILogger" />
    public class Logger : Shared.ILogger
    {
        #region Fields

        /// <summary>
        /// The log
        /// </summary>
        protected static NLog.Logger log = LogManager.GetCurrentClassLogger();

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

        #endregion Methods
    }
}
