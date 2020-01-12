// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="Program.cs" company="IronyModManager">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Logging.Serilog;
using IronyModManager.DI;

namespace IronyModManager
{
    /// <summary>
    /// Class Program.
    /// </summary>
    internal class Program
    {
        #region Methods

        // Avalonia configuration, don't remove; also used by visual designer.
        /// <summary>
        /// Builds the avalonia application.
        /// </summary>
        /// <returns>AppBuilder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug(); //.UseReactiveUI(); // Doesn't follow proper DI conventions according to SimpleInjector.

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            InitCulture();
            InitAppConfig();
            InitDI();

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                LogError((Exception)e.ExceptionObject);
            }
        }

        /// <summary>
        /// Initializes the application configuration.
        /// </summary>
        private static void InitAppConfig()
        {
            // Do not catch exceptions and log them if debugger is attached
            if (!Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            }
        }

        /// <summary>
        /// Initializes the culture.
        /// </summary>
        private static void InitCulture()
        {
            var culture = new CultureInfo(Constants.Culture);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Initializes the di.
        /// </summary>
        private static void InitDI()
        {
            Bootstrap.Init(Constants.PluginsPathAndName);

            Bootstrap.Finish();
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="e">The e.</param>
        private static void LogError(Exception e)
        {
            if (e != null)
            {
                // TODO: Add logger
            }
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs"/> instance containing the event data.</param>
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogError(e.Exception);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class Constants.
        /// </summary>
        private class Constants
        {
            #region Fields

            /// <summary>
            /// The culture
            /// </summary>
            public const string Culture = "en-US";

            /// <summary>
            /// The error message
            /// </summary>
            public const string ErrorMessage = "Unhandled error occurred";

            /// <summary>
            /// The error title
            /// </summary>
            public const string ErrorTitle = "Error";

            /// <summary>
            /// The plugins path and name
            /// </summary>
            public const string PluginsPathAndName = "Plugins";

            #endregion Fields
        }

        #endregion Classes
    }
}
