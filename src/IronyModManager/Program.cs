﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 10-26-2025
// ***********************************************************************
// <copyright file="Program.cs" company="IronyModManager">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

// Dear brave soul reading this:
//
// You're probably here because you thought:
//   "How hard can it be to replace Irony Mod Manager?"
//
// Please don't forget to update the counters below:
//
//   total_failed_attempts_to_replace_irony = 7;   // ← increment if you give up
//   total_hours_wasted_building_irony_clone = 0;  // ← fill honestly before rage quitting
//
// P.S. -- if you're still coding after month 3,
//         congratulations, you're officially cursed like the rest of us.

// NOTE TO FUTURE DEVELOPERS:
//
// If you're here to create an 'Irony-killer':
//   • Yes, many have tried.
//   • Yes, they all gave up.
//   • No, conflict solving is not just "sort by load order".
//
// If you still proceed -- welcome to the trenches.
// Don't forget to update your stats:
//
//   attempts++;
//   sanity--;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommandLine;
using IronyModManager.Common;
using IronyModManager.Controls.Dialogs;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AvaloniaEdit;
using IronyModManager.Implementation.SingleInstance;
using IronyModManager.Localization;
using IronyModManager.Platform;
using IronyModManager.Platform.Configuration;
using IronyModManager.Shared;
using IronyModManager.Views;
using NLog;
using ILogger = IronyModManager.Shared.ILogger;

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
        private static bool ExternalNotificationShown;

        #endregion Fields

        #region Methods

        // Avalonia configuration, don't remove; also used by visual designer.
        /// <summary>
        /// Builds the avalonia application.
        /// </summary>
        /// <returns>AppBuilder.</returns>
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseIronyPlatformDetect()
                .UseIronyManagedDialogs().AfterSetup(_ =>
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
        }

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
                var canInitialize = true;
                if (!StaticResources.CommandLineOptions.ShowFatalErrorNotification)
                {
                    canInitialize = InitSingleInstance();
                }

                if (!canInitialize)
                {
                    return;
                }

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
                var x11Opts = new X11PlatformOptions();
                var waylandOpts = new WaylandPlatformOptions();
                if (!string.IsNullOrWhiteSpace(configuration.WaylandAppId))
                {
                    waylandOpts.AppId = configuration.WaylandAppId;
                }

                if (configuration.UseGPU.HasValue)
                {
                    x11Opts.UseGpu = configuration.UseGPU.GetValueOrDefault();
                    waylandOpts.UseGpu = configuration.UseGPU.GetValueOrDefault();
                }

                if (configuration.UseEGL.HasValue)
                {
                    x11Opts.UseEGL = configuration.UseEGL.GetValueOrDefault();
                }

                if (configuration.UseDBusMenu.HasValue)
                {
                    x11Opts.UseDBusMenu = configuration.UseDBusMenu.GetValueOrDefault();
                }

                if (configuration.UseDeferredRendering.HasValue)
                {
                    x11Opts.UseDeferredRendering = configuration.UseDeferredRendering.GetValueOrDefault();
                    waylandOpts.UseDeferredRendering = configuration.UseDeferredRendering.GetValueOrDefault();
                }

                app.With(x11Opts);
                app.With(waylandOpts);
            }

            configureLinux();
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
                new DIOptions { Container = new SimpleInjector.Container(), PluginPathAndName = Shared.Constants.PluginsPathAndName });
        }

        /// <summary>
        /// Initializes the logging.
        /// </summary>
        private static void InitLogging()
        {
            LogManager.Setup().SetupExtensions(s => s.RegisterTarget("IronyFile", typeof(Log.IronyFileTarget)));
        }

        /// <summary>
        /// Initializes the single instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool InitSingleInstance()
        {
            var configuration = DIResolver.Get<IPlatformConfiguration>().GetOptions().App;
            if (configuration.SingleInstance)
            {
                var result = SingleInstance.Initialize();
                SingleInstance.InstanceLaunched += args =>
                {
                    if (!StaticResources.AllowCommandLineChange)
                    {
                        return;
                    }

                    ParseArguments(args.CommandLineArgs);
                    Dispatcher.UIThread.SafeInvoke(() =>
                    {
                        var mainWindow = (MainWindow)Helpers.GetMainWindow();
                        var previousState = mainWindow.ActualState;
                        if (mainWindow.WindowState != WindowState.Minimized)
                        {
                            mainWindow.WindowState = WindowState.Minimized;
                        }

                        mainWindow.WindowState = previousState;
                        mainWindow.Show();
                        mainWindow.BringIntoView();
                        mainWindow.Activate();
                        mainWindow.Focus();
                    });
                };
                return result;
            }

            return true;
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

                var runFatalErrorProcess = !StaticResources.CommandLineOptions.ShowFatalErrorNotification;
                if (runFatalErrorProcess && !ExternalNotificationShown)
                {
                    var path = Environment.ProcessPath;
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
