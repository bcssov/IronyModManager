// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
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
using Avalonia.Threading;
using IronyModManager.Controls.Dialogs;
using IronyModManager.DI;
using IronyModManager.Implementation;
using IronyModManager.Localization;
using IronyModManager.Shared;

namespace IronyModManager
{
    /// <summary>
    /// Class Program.
    /// </summary>
    [ExcludeFromCoverage("Program entry point.")]
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
                .UseManagedDialogs().AfterSetup((s) =>
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
                Bootstrap.PostStartup();
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
                    Container = new SimpleInjector.Container(),
                    PluginPathAndName = Shared.Constants.PluginsPathAndName
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
                var header = Constants.UnhandledErrorHeader;
                try
                {
                    var locManager = DIResolver.Get<ILocalizationManager>();
                    title = locManager.GetResource(LocalizationResources.FatalError.Title);
                    message = locManager.GetResource(LocalizationResources.FatalError.Message);
                    header = locManager.GetResource(LocalizationResources.FatalError.Header);
                }
                catch
                {
                }
                var messageBox = MessageBoxes.GetFatalErrorWindow(title, header, message);
                // We're deadlocking the thread, so kill the task after x amount of seconds.
                messageBox.ShowAsync().Wait(TimeSpan.FromSeconds(10));
                Dispatcher.UIThread.InvokeAsync(() => Environment.Exit(0));
            }
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs" /> instance containing the event data.</param>
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            LogError(e.Exception);
        }

        #endregion Methods
    }
}
