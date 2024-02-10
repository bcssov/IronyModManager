
// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-10-2024
// ***********************************************************************
// <copyright file="Program.cs" company="IronyModManager">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
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
                if (!StaticResources.CommandLineOptions.ShowFatalErrorNotification)
                {
                    InitSingleInstance();
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
            LogManager.Setup().SetupExtensions(s => s.RegisterTarget("IronyFile", typeof(Log.IronyFileTarget)));
        }

        /// <summary>
        /// Initializes the single instance.
        /// </summary>
        private static void InitSingleInstance()
        {
            var configuration = DIResolver.Get<IPlatformConfiguration>().GetOptions().App;
            if (configuration.SingleInstance)
            {
                SingleInstance.Initialize();
                SingleInstance.InstanceLaunched += (args) =>
                {
                    ParseArguments(args.CommandLineArgs);
                    Dispatcher.UIThread.SafeInvoke(() =>
                    {
                        App.MainWindow.Show();
                        App.MainWindow.Activate();
                        var previousState = App.MainWindow.WindowState;
                        App.MainWindow.WindowState = WindowState.Minimized;
                        App.MainWindow.WindowState = previousState;
                    });
                };
            }
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
