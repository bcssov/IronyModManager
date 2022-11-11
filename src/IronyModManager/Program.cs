// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 11-06-2022
// ***********************************************************************
// <copyright file="Program.cs" company="IronyModManager">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using CommandLine;
using IronyModManager.Controls.Dialogs;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AvaloniaEdit;
using IronyModManager.Localization;
using IronyModManager.Platform;
using IronyModManager.Platform.Configuration;
using IronyModManager.Shared;

namespace IronyModManager
{
    /// <summary>
    /// Class Program.
    /// </summary>
    [ExcludeFromCoverage("Program entry point.")]
    internal class Program
    {
        #region Fields

        /// <summary>
        /// The external notification shown
        /// </summary>
        private static bool ExternalNotificationShown = false;

        #endregion Fields

        #region Methods

        // Avalonia configuration, don't remove; also used by visual designer.
        /// <summary>
        /// Builds the avalonia application.
        /// </summary>
        /// <returns>AppBuilder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UseIronyPlatformDetect()
                .UseIronyManagedDialogs().AfterSetup((s) =>
                {
                    if (!Design.IsDesignMode)
                    {
                        StaticResources.IsVerifyingContainer = true;
                        DIContainer.Finish();
                        StaticResources.IsVerifyingContainer = false;
                    }
                    else
                    {
                        DIContainer.Finish(true);
                    }
                });

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
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            InitDefaultCulture();
            InitAppEvents();
            InitDI();
            InitLogging();

            try
            {
                ParseArguments(args);
                var app = BuildAvaloniaApp();
                InitAvaloniaOptions(app);
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
            if (e.ExceptionObject is Exception exception)
            {
                LogError(exception);
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
        /// Initializes the avalonia options.
        /// </summary>
        /// <param name="app">The application.</param>
        private static void InitAvaloniaOptions(AppBuilder app)
        {
            void configureLinux()
            {
                var configuration = DIResolver.Get<IPlatformConfiguration>().GetOptions().LinuxOptions;
                if (configuration.UseGPU.HasValue || configuration.UseEGL.HasValue || configuration.UseDBusMenu.HasValue || configuration.UseDeferredRendering.HasValue)
                {
                    var opts = new X11PlatformOptions();
                    if (configuration.UseGPU.HasValue)
                    {
                        opts.UseGpu = configuration.UseGPU.GetValueOrDefault();
                    }
                    if (configuration.UseEGL.HasValue)
                    {
                        opts.UseEGL = configuration.UseEGL.GetValueOrDefault();
                    }
                    if (configuration.UseDBusMenu.HasValue)
                    {
                        opts.UseDBusMenu = configuration.UseDBusMenu.GetValueOrDefault();
                    }
                    if (configuration.UseDeferredRendering.HasValue)
                    {
                        opts.UseDeferredRendering = configuration.UseDeferredRendering.GetValueOrDefault();
                    }
                    app.With(opts);
                }
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configureLinux();
            }
        }

        /// <summary>
        /// Initializes the default culture.
        /// </summary>
        private static void InitDefaultCulture()
        {
            CurrentLocale.SetCurrent(Shared.Constants.DefaultAppCulture);
            ResourceManager.Init();
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
        /// Initializes the logging.
        /// </summary>
        private static void InitLogging()
        {
            NLog.Config.ConfigurationItemFactory.Default.Targets.RegisterDefinition("IronyFile", typeof(Log.IronyFileTarget));
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="e">The e.</param>
        private static async void LogError(Exception e)
        {
            if (e != null)
            {
                var logger = DIResolver.Get<ILogger>();
                logger.Fatal(e);

                var runFatalErrorProcess = StaticResources.CommandLineOptions == null || !StaticResources.CommandLineOptions.ShowFatalErrorNotification;
                if (runFatalErrorProcess && !ExternalNotificationShown)
                {
                    var path = Process.GetCurrentProcess().MainModule.FileName;
                    var appAction = DIResolver.Get<IAppAction>();
                    await appAction.RunAsync(path, "--fatal-error").ConfigureAwait(false);
                    ExternalNotificationShown = true;
                }
                // Force exit as it seems that sometimes the app doesn't quit on such an error
                Thread.Sleep(10000);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void ParseArguments(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<CommandLineArgs>(args).WithParsed(a =>
            {
                StaticResources.CommandLineOptions = a;
            });
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
