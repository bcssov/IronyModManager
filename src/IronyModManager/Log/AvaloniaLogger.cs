// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-30-2021
// ***********************************************************************
// <copyright file="AvaloniaLogger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Logging;
using IronyModManager.DI;
using IronyModManager.Shared;
using Microsoft.Extensions.Configuration;

namespace IronyModManager.Log
{
    /// <summary>
    /// Class AvaloniaLogger.
    /// Implements the <see cref="IronyModManager.Log.Logger" />
    /// Implements the <see cref="Avalonia.Logging.ILogSink" />
    /// </summary>
    /// <seealso cref="IronyModManager.Log.Logger" />
    /// <seealso cref="Avalonia.Logging.ILogSink" />
    [ExcludeFromCoverage("Exclude logger.")]
    public class AvaloniaLogger : Logger, ILogSink
    {
        #region Fields

        /// <summary>
        /// The logging allowed
        /// </summary>
        private bool? loggingAllowed;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determines whether the specified level is enabled.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns><c>true</c> if the specified level is enabled; otherwise, <c>false</c>.</returns>
        public bool IsEnabled(LogEventLevel level)
        {
            if (!loggingAllowed.HasValue)
            {
                var configuration = DIResolver.Get<IConfigurationRoot>();
                loggingAllowed = configuration.GetSection("Logging").GetSection("EnableAvaloniaLogger").Get<bool?>().GetValueOrDefault();
            }
            if (loggingAllowed.GetValueOrDefault())
            {
                return level switch
                {
                    LogEventLevel.Verbose => IsTraceEnabled(),
                    LogEventLevel.Debug => IsDebugEnabled(),
                    LogEventLevel.Information => IsInfoEnabled(),
                    LogEventLevel.Warning => IsWarnEnabled(),
                    LogEventLevel.Error => IsErrorEnabled(),
                    LogEventLevel.Fatal => IsFatalEnabled(),
                    _ => false,
                };
            }
            return false;
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="area">The area.</param>
        /// <param name="source">The source.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Log(LogEventLevel level, string area, object source, string messageTemplate)
        {
            log.Log(MapLogLevel(level), messageTemplate);
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <typeparam name="T0">The type of the t0.</typeparam>
        /// <param name="level">The level.</param>
        /// <param name="area">The area.</param>
        /// <param name="source">The source.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="propertyValue0">The property value0.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Log<T0>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0)
        {
            log.Log(MapLogLevel(level), messageTemplate, propertyValue0);
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <typeparam name="T0">The type of the t0.</typeparam>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <param name="level">The level.</param>
        /// <param name="area">The area.</param>
        /// <param name="source">The source.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="propertyValue0">The property value0.</param>
        /// <param name="propertyValue1">The property value1.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Log<T0, T1>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            log.Log(MapLogLevel(level), messageTemplate, propertyValue0, propertyValue1);
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <typeparam name="T0">The type of the t0.</typeparam>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="level">The level.</param>
        /// <param name="area">The area.</param>
        /// <param name="source">The source.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="propertyValue0">The property value0.</param>
        /// <param name="propertyValue1">The property value1.</param>
        /// <param name="propertyValue2">The property value2.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Log<T0, T1, T2>(LogEventLevel level, string area, object source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            log.Log(MapLogLevel(level), messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        /// <summary>
        /// Logs the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="area">The area.</param>
        /// <param name="source">The source.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Log(LogEventLevel level, string area, object source, string messageTemplate, params object[] propertyValues)
        {
            log.Log(MapLogLevel(level), messageTemplate, propertyValues);
        }

        /// <summary>
        /// Maps the log level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>NLog.LogLevel.</returns>
        private static NLog.LogLevel MapLogLevel(LogEventLevel level)
        {
            var mappedLevel = NLog.LogLevel.Debug;
            switch (level)
            {
                case LogEventLevel.Verbose:
                    mappedLevel = NLog.LogLevel.Trace;
                    break;

                case LogEventLevel.Debug:
                    mappedLevel = NLog.LogLevel.Debug;
                    break;

                case LogEventLevel.Information:
                    mappedLevel = NLog.LogLevel.Info;
                    break;

                case LogEventLevel.Warning:
                    mappedLevel = NLog.LogLevel.Warn;
                    break;

                case LogEventLevel.Error:
                    mappedLevel = NLog.LogLevel.Error;
                    break;

                case LogEventLevel.Fatal:
                    mappedLevel = NLog.LogLevel.Fatal;
                    break;

                default:
                    break;
            }
            return mappedLevel;
        }

        #endregion Methods
    }
}
