// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-24-2020
// ***********************************************************************
// <copyright file="Program.cs" company="IronyModManager">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;

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
                .UsePlatformDetect().AfterSetup((s) =>
                {
# if DEBUG
                    if (!Design.IsDesignMode)
                    {
                        DIContainer.Verify();
                    }
#endif
                }); // You gotta be kidding me?!? Avalonia has a logging reference to Serilog which cannot be removed?!?

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
            InitDefaultCulture();
            InitAppEvents();
            InitDI();

            try
            {
                var app = BuildAvaloniaApp();
                app.StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
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
        /// Initializes the application events.
        /// </summary>
        private static void InitAppEvents()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// Initializes the default culture.
        /// </summary>
        private static void InitDefaultCulture()
        {
            CurrentLocale.SetCurrent(Shared.Constants.DefaultAppCulture);
        }

        /// <summary>
        /// Initializes the di.
        /// </summary>
        private static void InitDI()
        {
            Bootstrap.Setup(
                new DIOptions()
                {
                    PluginPathAndName = Shared.Constants.PluginsPathAndName,
                    ModuleTypes = new List<Type>() { typeof(IModule) },
                    PluginTypes = new List<Type>() { typeof(IPlugin) }
                });
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="e">The e.</param>
        private static void LogError(Exception e)
        {
            if (e != null)
            {
                var logger = DIResolver.Get<ILogger>();
                logger.Error(e);

                var title = Constants.UnhandledErrorTitle;
                var message = Constants.UnhandledErrorMessage;
                try
                {
                    var locManager = DIResolver.Get<ILocalizationManager>();
                    title = locManager.GetResource(LocalizationResources.FatalError.Title);
                    message = locManager.GetResource(LocalizationResources.FatalError.Message);
                }
                catch
                {
                }
                var messageBox = MessageBoxes.GetFatalErrorWindow(title, message);
                // We're deadlocking the thread, so kill the task after x amount of seconds.
                messageBox.Show().Wait(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs" /> instance containing the event data.</param>
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogError(e.Exception);
            e.SetObserved();
        }

        #endregion Methods
    }
}
