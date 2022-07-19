// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-29-2021
//
// Last Modified By : Mario
// Last Modified On : 07-10-2022
// ***********************************************************************
// <copyright file="IronyFileTarget.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronyModManager.IO.Common;
using NLog;
using NLog.Targets;

namespace IronyModManager.Log
{
    /// <summary>
    /// Class IronyFileTarget.
    /// Implements the <see cref="NLog.Targets.FileTarget" />
    /// </summary>
    /// <seealso cref="NLog.Targets.FileTarget" />
    [Target("IronyFile")]
    public class IronyFileTarget : FileTarget
    {
        #region Fields

        /// <summary>
        /// The last fatal exception file name
        /// </summary>
        private const string LastFatalExceptionFileName = "last-exception.log";

        /// <summary>
        /// The output directory
        /// </summary>
        private static string outputDirectory;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the last fatal exception.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetLastFatalException()
        {
            var fileName = GetLastExceptionFileName(new LogEventInfo(LogLevel.Info, string.Empty, string.Empty));
            if (File.Exists(fileName))
            {
                var data = File.ReadAllText(fileName);
                DiskOperations.DeleteFile(fileName);
                return data;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the last name of the exception file.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>System.String.</returns>
        protected string GetLastExceptionFileName(LogEventInfo logEvent)
        {
            return Path.Combine(GetOutputDirectory(logEvent), LastFatalExceptionFileName);
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>System.String.</returns>
        protected string GetOutputDirectory(LogEventInfo logEvent)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                var layout = new FilePathLayout(FileName, CleanupFileName, FileNameKind);
                var logFileName = layout.Render(logEvent);
                outputDirectory = Path.GetDirectoryName(logFileName);
            }
            return outputDirectory;
        }

        /// <summary>
        /// Logs the last fatal error.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected void LogLastFatalError(LogEventInfo logEvent)
        {
            var output = GetOutputDirectory(logEvent);
            if (logEvent.Level == LogLevel.Fatal && !StaticResources.CommandLineOptions.ShowFatalErrorNotification && Directory.Exists(output))
            {
                try
                {
                    var sbOut = new StringBuilder();
                    RenderFormattedMessage(logEvent, sbOut);
                    File.WriteAllText(GetLastExceptionFileName(logEvent), sbOut.ToString());
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Writes the specified logging event to a file specified in the FileName
        /// parameter.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            base.Write(logEvent);
            LogLastFatalError(logEvent);
        }

        #endregion Methods
    }
}
